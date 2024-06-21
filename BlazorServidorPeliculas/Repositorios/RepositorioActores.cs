using AutoMapper;
using BlazorServidorPeliculas.Entidades;
using BlazorServidorPeliculas.Data;
using BlazorServidorPeliculas.DTOs;
using BlazorServidorPeliculas.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorServidorPeliculas.Repositorios
{
    public class RepositorioActores
    {
        private readonly ApplicationDbContext context;
        private readonly IAlmacenadorArchivos almacenadorArchivos;
        private readonly IMapper mapper;
        private readonly string contenedor = "personas";


        public RepositorioActores(ApplicationDbContext context, IAlmacenadorArchivos almacenadorArchivos, IMapper mapper)
        {
            this.context = context;
            this.almacenadorArchivos = almacenadorArchivos;
            this.mapper = mapper;
        }

        public async Task<RespuestaPaginadaDTO<Actor>> Get(
            PaginacionDTO paginacion)
        {
            var queryable = context.Actores.AsNoTracking().AsQueryable();
            var respuesta = new RespuestaPaginadaDTO<Actor>();
            respuesta.TotalPaginas = await queryable.CalcularTotalPaginas(paginacion.CantidadRegistro);
            respuesta.Registros = await queryable.Paginar(paginacion).ToListAsync();
            return respuesta;
        }

        public async Task<List<Actor>> Get(string textoBusqueda)
        {
            if (string.IsNullOrWhiteSpace(textoBusqueda))
            {
                return new List<Actor>();
            }

            textoBusqueda = textoBusqueda.ToLower();
            return await context.Actores.Where(x => x.Nombre.ToLower().Contains(textoBusqueda)).Take(5).AsNoTracking().ToListAsync();
        }

        public async Task<int> Post(Actor actor)
        {
            if (!string.IsNullOrWhiteSpace(actor.Foto))
            {
                var fotoActor = Convert.FromBase64String(actor.Foto);
                actor.Foto = await almacenadorArchivos.GuardarArchivo(fotoActor, ".jpg", contenedor);
            }
            context.Add(actor);
            await context.SaveChangesAsync();
            return actor.Id;
        }

        public async Task<Actor> Get(int id)
        {
            var respuesta = await context.Actores.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            return respuesta;
        }

        public async Task Put(Actor actor)//cambios nombre pedro, nuevo
        {
            var actorDB = await context.Actores.AsNoTracking().FirstOrDefaultAsync(a => a.Id == actor.Id);//nombre juan, antiguo
            if (actorDB is null)
            {
                return;
            }

            actorDB = mapper.Map(actor, actorDB);// cambia lo nuevo por lo viejo, actorDB va a actualizar el registro

            if (!string.IsNullOrWhiteSpace(actor.Foto))
            {
                var fotoActor = Convert.FromBase64String(actor.Foto);
                actorDB.Foto = await almacenadorArchivos.EditarArchivo(fotoActor, ".jpg", contenedor, actorDB.Foto);
            }

            await context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var actor = await context.Actores.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

            if (actor is null)
            {
                return;
            }
            context.Remove(actor);
            await context.SaveChangesAsync();
            await almacenadorArchivos.EliminarArchivo(actor.Foto!, contenedor);

        }

    }
}
