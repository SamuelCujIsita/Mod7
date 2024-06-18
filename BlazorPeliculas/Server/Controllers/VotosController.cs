using AutoMapper;
using BlazorPeliculas.Shared.DTOs;
using BlazorPeliculas.Shared.Entidades;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace BlazorPeliculas.Server.Controllers
{
    [ApiController]
    [Route("api/votos")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class VotosController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IMapper mapper;

        public VotosController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IMapper mapper)
        {
            this.context = context;
            this.userManager = userManager;
            this.mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult> Votar(VotoPeliculaDTO votoPeliculaDTO)
        {
            var user = await userManager.FindByEmailAsync(HttpContext.User.Identity!.Name);
            if (user == null)
            {
                return BadRequest("usuario no encontrado");
            }

            var usuarioid = user.Id;

            var votoActual = await context.VotosPeliculas
                .FirstOrDefaultAsync(x => x.PeliculaId == votoPeliculaDTO.PeliculaId
            && x.UsuarioId == usuarioid);

            if (votoActual is null)
            {
                var votopelicula = mapper.Map<VotoPelicula>(votoPeliculaDTO);
                votopelicula.UsuarioId = usuarioid;
                votopelicula.FechaVoto = DateTime.Now;
                context.Add(votopelicula);
            }
            else 
            { 
                votoActual.FechaVoto= DateTime.Now;
                votoActual.Voto = votoPeliculaDTO.Voto;
            }

            await context.SaveChangesAsync();
            return NoContent();
        }

    }
}
