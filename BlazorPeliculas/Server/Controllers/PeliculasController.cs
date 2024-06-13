using AutoMapper;
using BlazorPeliculas.Server.Helpers;
using BlazorPeliculas.Shared.DTOs;
using BlazorPeliculas.Shared.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorPeliculas.Server.Controllers
{
    [ApiController]
    [Route("api/peliculas")]
    public class PeliculasController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IAlmacenadorArchivos almacenadorArchivos;
        private readonly IMapper mapper;
        private readonly string contenedor = "peliculas";

        public PeliculasController(ApplicationDbContext context, IAlmacenadorArchivos almacenadorArchivos, IMapper mapper)
        {
            this.context = context;
            this.almacenadorArchivos = almacenadorArchivos;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<HomePageDTO>> Get()
        {
            var limt = 6;
            var fechaActual = DateTime.Today;

            var peliculasEnCartelera = await context.Peliculas
                .Where(peli => peli.EnCartelera).Take(limt)
                .OrderByDescending(peli => peli.Lanzamiento)
                .ToListAsync();

            var proximosEstrenos = await context.Peliculas
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

        [HttpGet("{id:int}")]//por id obtiene info
        public async Task<ActionResult<PeliculaVisualizarDTO>> Get(int id)
        {
            var pelicula = await context.Peliculas.Where(pelicula => pelicula.Id == id)
                .Include(pelicula => pelicula.GenerosPelicula)
                    .ThenInclude(gp => gp.Genero)
                .Include(pelicula => pelicula.PeliculasActor.OrderBy(pa => pa.Orden))
                    .ThenInclude(pa => pa.Actor)
                .FirstOrDefaultAsync();

            if (pelicula is null)
            {
                return NotFound();
            }

            // TODO: Sistema de votación
            var promedioVoto = 4;
            var votoUsuario = 5;

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

        [HttpGet("filtrar")]
        public async Task<ActionResult<List<Pelicula>>> Get(
            [FromQuery] ParametrosBusquedaPeliculasDTO modelo)
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

            // TODO: Implementar votación

            await HttpContext.InsertarParametrosPaginacionEnRespuesta(peliculasQueryable,
                    modelo.CantidadRegistros);

            var peliculas = await peliculasQueryable.Paginar(modelo.PaginacionDTO).ToListAsync();
            return peliculas;
        }



        [HttpGet("actualizar/{id}")]
        public async Task<ActionResult<PeliculaActualizaconDTO>> PutGet(int id) //get para actualizar la pantalla para editar
        {

            var peliculaActionResult = await Get(id);
            if (peliculaActionResult.Result is NotFoundResult)
            {
                return NotFound();
            }

            var peliculaVisualizarDTO = peliculaActionResult.Value;
            var generosSeleccionadosIds = peliculaVisualizarDTO!.Generos.Select(x => x.Id).ToList();
            var generosNoSeleccionados = await context.Generos.Where(x => !generosSeleccionadosIds.Contains(x.Id)).ToListAsync();

            return new PeliculaActualizaconDTO()
            {
                Pelicula = peliculaVisualizarDTO.Pelicula,
                GeneroSeleccionados = peliculaVisualizarDTO.Generos,
                GeneroNoSeleccionados = generosNoSeleccionados,
                Actores = peliculaVisualizarDTO.Actores
            };

        }

        [HttpPost]
        public async Task<ActionResult<int>> Post(Pelicula pelicula)
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

        [HttpPut]
        public async Task<ActionResult> Put(Pelicula pelicula)
        {
            var peliculaDB = await context.Peliculas
                .Include(x => x.GenerosPelicula)
                .Include(x => x.PeliculasActor)
                .FirstOrDefaultAsync(x => x.Id == pelicula.Id);
            if (pelicula is null)
            {
                return NotFound();
            }

            peliculaDB = mapper.Map(pelicula, peliculaDB);

            if (!string.IsNullOrWhiteSpace(pelicula.Poster))
            {
                var posterImagen = Convert.FromBase64String(pelicula.Poster);
                peliculaDB!.Poster = await almacenadorArchivos.EditarArchivo(posterImagen, ".jpg", contenedor, peliculaDB.Poster!);


            }

            EscribirOrdenActores(peliculaDB!);

            await context.SaveChangesAsync();
            return NoContent();

        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var peli = await context.Peliculas.FirstOrDefaultAsync(x => x.Id == id);

            if (peli is null)
            {
                return NotFound();
            }

            context.Remove(peli);
            await context.SaveChangesAsync();
            await almacenadorArchivos.EliminarArchivo(peli.Poster!, contenedor);
            return NoContent();

        }

    }
}
