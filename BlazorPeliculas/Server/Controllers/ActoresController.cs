using AutoMapper;
using BlazorPeliculas.Server.Helpers;
using BlazorPeliculas.Shared.DTOs;
using BlazorPeliculas.Shared.Entidades;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorPeliculas.Server.Controllers
{
    [ApiController]
    [Route("api/actores")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ActoresController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IAlmacenadorArchivos almacenadorArchivos;
        private readonly IMapper mapper;
        private readonly string contenedor = "personas";


        public ActoresController(ApplicationDbContext context, IAlmacenadorArchivos almacenadorArchivos, IMapper mapper)
        {
            this.context = context;
            this.almacenadorArchivos = almacenadorArchivos;
            this.mapper = mapper;
        }

        //Querly string -> url?pagina=1
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Actor>>> Get(
            [FromQuery] PaginacionDTO paginacion)
        {
            var queryable = context.Actores.AsQueryable();
            Console.WriteLine(queryable.ToQueryString());
            await HttpContext
                .InsertarParametrosPaginacionEnRespuesta(queryable, paginacion.CantidadRegistro);

            return await queryable.OrderBy(x => x.Nombre).Paginar(paginacion).ToListAsync();
        }

        [HttpGet("buscar/{textoBusqueda}")]//Variable de ruta
        public async Task<ActionResult<List<Actor>>> Get(string textoBusqueda)
        {
            if (string.IsNullOrWhiteSpace(textoBusqueda))
            {
                return new List<Actor>();
            }

            textoBusqueda = textoBusqueda.ToLower();
            return await context.Actores.Where(x => x.Nombre.ToLower().Contains(textoBusqueda)).Take(5).ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<int>> Post(Actor actor)
        {
            if(!string.IsNullOrWhiteSpace(actor.Foto))
            {
                var fotoActor = Convert.FromBase64String(actor.Foto);
                actor.Foto = await almacenadorArchivos.GuardarArchivo(fotoActor, ".jpg", contenedor);
            }
            context.Add(actor);
            await context.SaveChangesAsync();
            return actor.Id;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Actor>> Get(int id)
        {
            var respuesta = await context.Actores.FirstOrDefaultAsync(x => x.Id == id);

            if (respuesta is null)
            {
                return NotFound();
            }

            return respuesta;
        }

        [HttpPut]
        public async Task<ActionResult> Put(Actor actor)//cambios nombre pedro, nuevo
        {
            var actorDB = await context.Actores.FirstOrDefaultAsync(a => a.Id == actor.Id);//nombre juan, antiguo
            if (actorDB is null)
            {
                return NotFound();
            }

            actorDB = mapper.Map(actor, actorDB);// cambia lo nuevo por lo viejo, actorDB va a actualizar el registro

            if (!string.IsNullOrWhiteSpace(actor.Foto))
            {
                var fotoActor = Convert.FromBase64String(actor.Foto);
                actorDB.Foto = await almacenadorArchivos.EditarArchivo(fotoActor, ".jpg", contenedor, actorDB.Foto);
            }

            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var actor = await context.Actores.FirstOrDefaultAsync(x => x.Id == id);

            if (actor is null)
            {
                return NotFound();
            }

            context.Remove(actor);
            await context.SaveChangesAsync();
            await almacenadorArchivos.EliminarArchivo(actor.Foto!, contenedor);
            return NoContent();

        }

    }
}
