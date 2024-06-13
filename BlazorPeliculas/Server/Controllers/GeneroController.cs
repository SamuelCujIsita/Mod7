using BlazorPeliculas.Shared.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorPeliculas.Server.Controllers
{
    [Route("api/generos")]
    [ApiController]
    public class GeneroController : ControllerBase
    {
        public readonly ApplicationDbContext _context;
        public GeneroController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Genero>>> Get()
        {
            return await _context.Generos.ToListAsync();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Genero>> Get(int id)
        {
            var respuesta = await _context.Generos.FirstOrDefaultAsync(x => x.Id == id);

            if (respuesta is null)
            {
                return NotFound();
            }

            return respuesta;
        }

        [HttpPost]
        public async Task<ActionResult<int>> Post(Genero genero)
        {
            _context.Add(genero);
            await _context.SaveChangesAsync();
            return genero.Id;
        }

        [HttpPut]
        public async Task<ActionResult> Put(Genero genero)
        {
            _context.Update(genero);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var filasAfectadas = await _context.Generos.Where(x=> x.Id == id).ExecuteDeleteAsync();
            
            if(filasAfectadas == 0)
            {
                return NotFound();
            }

            return NoContent();

        }

    }
}
