using AutoMapper;
using BlazorServidorPeliculas.Data;
using BlazorServidorPeliculas.DTOs;
using BlazorServidorPeliculas.Entidades;
using BlazorServidorPeliculas.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorServidorPeliculas.Repositorios
{
    public class RepositorioPeliculas
    {
        private readonly ApplicationDbContext context;
        private readonly IAlmacenadorArchivos almacenadorArchivos;
        private readonly IMapper mapper;
        private readonly UserManager<IdentityUser> userManager;
        private readonly AuthenticationStateService authentication;
        private readonly string contenedor = "peliculas";

        public RepositorioPeliculas(ApplicationDbContext context, IAlmacenadorArchivos almacenadorArchivos, IMapper mapper, UserManager<IdentityUser> userManager, AuthenticationStateService authentication)
        {
            this.context = context;
            this.almacenadorArchivos = almacenadorArchivos;
            this.mapper = mapper;
            this.userManager = userManager;
            this.authentication = authentication;
        }

        public async Task<HomePageDTO> Get()
        {
            var limt = 6;
            var fechaActual = DateTime.Today;

            var peliculasEnCartelera = await context.Peliculas
                .AsNoTracking()
                .Where(peli => peli.EnCartelera).Take(limt)
                .OrderByDescending(peli => peli.Lanzamiento)
                .ToListAsync();

            var proximosEstrenos = await context.Peliculas
                .AsNoTracking()
                .Where(x => x.Lanzamiento > fechaActual)
                .OrderBy(pel => pel.Lanzamiento).Take(limt)
                .ToListAsync();

            var resultado = new HomePageDTO
            {
                PeliculasEnCartelera = peliculasEnCartelera,
                ProximosEstrenos = proximosEstrenos
            };
            return resultado;
        }

        public async Task<PeliculaVisualizarDTO> Get(int id)
        {
            var pelicula = await context.Peliculas.Where(pelicula => pelicula.Id == id)
                .Include(pelicula => pelicula.GenerosPelicula)
                    .ThenInclude(gp => gp.Genero)
                .Include(pelicula => pelicula.PeliculasActor.OrderBy(pa => pa.Orden))
                    .ThenInclude(pa => pa.Actor)
                .FirstOrDefaultAsync();

            if (pelicula is null)
            {
                return null;
            }

            // TODO: Sistema de votación
            var promedioVoto = 0.0;
            var votoUsuario = 0;

            if (await context.VotosPeliculas.AnyAsync(x => x.PeliculaId == id))
            {
                promedioVoto = await context.VotosPeliculas.Where(x => x.PeliculaId == id)
                    .AverageAsync(x => x.Voto);

                var userid = await authentication.GetCurrentUserId();

                if (userid is null)
                {
                    var votoUsuarioDB = await context.VotosPeliculas
                   .FirstOrDefaultAsync(x => x.PeliculaId == id && x.UsuarioId == userid);
                    if (votoUsuarioDB is not null)
                    {
                        votoUsuario = votoUsuarioDB.Voto;
                    }
                }

            }

            var modelo = new PeliculaVisualizarDTO();
            modelo.Pelicula = pelicula;
            modelo.Generos = pelicula.GenerosPelicula.Select(gp => gp.Genero!).ToList();
            modelo.Actores = pelicula.PeliculasActor.Select(pa => new Actor
            {
                Nombre = pa.Actor!.Nombre,
                Foto = pa.Actor.Foto,
                Personaje = pa.Personaje,
                Id = pa.ActorId
            }).ToList();

            modelo.PromedioVotos = promedioVoto;
            modelo.VotoUsuario = votoUsuario;
            return modelo;
        }

        public async Task<RespuestaPaginadaDTO<Pelicula>> Get(
        ParametrosBusquedaPeliculasDTO modelo)
        {
            var peliculasQueryable = context.Peliculas.AsQueryable();

            if (!string.IsNullOrWhiteSpace(modelo.Titulo))
            {
                peliculasQueryable = peliculasQueryable
                    .Where(x => x.Titulo.Contains(modelo.Titulo));
            }

            if (modelo.EnCartelera)
            {
                peliculasQueryable = peliculasQueryable.Where(x => x.EnCartelera);
            }

            if (modelo.Estrenos)
            {
                var hoy = DateTime.Today;
                peliculasQueryable = peliculasQueryable.Where(x => x.Lanzamiento >= hoy);
            }

            if (modelo.GeneroId != 0)
            {
                peliculasQueryable = peliculasQueryable
                                        .Where(x => x.GenerosPelicula
                                            .Select(y => y.GeneroId)
                                            .Contains(modelo.GeneroId));
            }

            if (modelo.MasVotadas)
            {
                peliculasQueryable = peliculasQueryable.OrderByDescending(p =>
                p.VotosPeliculas.Average(vp => vp.Voto));
            }

            var respuesta = new RespuestaPaginadaDTO<Pelicula>();
            respuesta.TotalPaginas = await peliculasQueryable.CalcularTotalPaginas(modelo.CantidadRegistros);
            respuesta.Registros = await peliculasQueryable.AsNoTracking().Paginar(modelo.PaginacionDTO).ToListAsync();
            return respuesta;
        }

        public async Task<PeliculaActualizaconDTO> PutGet(int id) //get para actualizar la pantalla para editar
        {

            var peliculaActionResult = await Get(id);
            if (peliculaActionResult == null)
            {
                return null;
            }

            var peliculaVisualizarDTO = peliculaActionResult;
            var generosSeleccionadosIds = peliculaVisualizarDTO!.Generos.Select(x => x.Id).ToList();
            var generosNoSeleccionados = await context.Generos.AsNoTracking()
                    .Where(x => !generosSeleccionadosIds.Contains(x.Id))
                    .ToListAsync();

            return new PeliculaActualizaconDTO()
            {
                Pelicula = peliculaVisualizarDTO.Pelicula,
                GeneroSeleccionados = peliculaVisualizarDTO.Generos,
                GeneroNoSeleccionados = generosNoSeleccionados,
                Actores = peliculaVisualizarDTO.Actores
            };

        }

        public async Task<int> Post(Pelicula pelicula)
        {
            if (!string.IsNullOrWhiteSpace(pelicula.Poster))
            {
                var poster = Convert.FromBase64String(pelicula.Poster);
                pelicula.Poster = await almacenadorArchivos.GuardarArchivo(poster,
                    ".jpg", contenedor);
            }
            EscribirOrdenActores(pelicula);

            context.Add(pelicula);
            await context.SaveChangesAsync();
            return pelicula.Id;
        }

        private static void EscribirOrdenActores(Pelicula pelicula)
        {
            if (pelicula.PeliculasActor is not null)
            {
                for (int i = 0; i < pelicula.PeliculasActor.Count; i++)
                {
                    pelicula.PeliculasActor[i].Orden = i + 1;
                }
            }
        }

        public async Task Put(Pelicula pelicula)
        {
            var peliculaDB = await context.Peliculas
                .Include(x => x.GenerosPelicula)
                .Include(x => x.PeliculasActor)
                .FirstOrDefaultAsync(x => x.Id == pelicula.Id);

            if (pelicula is null)
            {
                throw new ApplicationException("No encontrada");
            }

            peliculaDB = mapper.Map(pelicula, peliculaDB);

            if (!string.IsNullOrWhiteSpace(pelicula.Poster))
            {
                var posterImagen = Convert.FromBase64String(pelicula.Poster);
                peliculaDB!.Poster = await almacenadorArchivos.EditarArchivo(posterImagen, ".jpg", contenedor, peliculaDB.Poster!);


            }

            EscribirOrdenActores(peliculaDB!);

            await context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var peli = await context.Peliculas.FirstOrDefaultAsync(x => x.Id == id);

            if (peli is null)
            {
                throw new ApplicationException("No encontrada");
            }

            context.Remove(peli);
            await context.SaveChangesAsync();
            await almacenadorArchivos.EliminarArchivo(peli.Poster!, contenedor);
        }

    }
}

