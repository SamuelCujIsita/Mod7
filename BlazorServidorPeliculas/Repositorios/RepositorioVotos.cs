using AutoMapper;
using BlazorServidorPeliculas.Data;
using BlazorServidorPeliculas.DTOs;
using BlazorServidorPeliculas.Entidades;
using BlazorServidorPeliculas.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorServidorPeliculas.Repositorios
{
    public class RepositorioVotos
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IMapper mapper;
        private readonly AuthenticationStateService authenticationService;

        public RepositorioVotos(ApplicationDbContext context, UserManager<IdentityUser> userManager, IMapper mapper, AuthenticationStateService authenticationService)
        {
            this.context = context;
            this.userManager = userManager;
            this.mapper = mapper;
            this.authenticationService = authenticationService;
        }

        public async Task Votar(VotoPeliculaDTO votoPeliculaDTO)
        {
            var userid = await authenticationService.GetCurrentUserId();
            if (userid == null)
            {
                return;
            }

            var votoActual = await context.VotosPeliculas.FirstOrDefaultAsync(x => x.PeliculaId
            == votoPeliculaDTO.PeliculaId && x.UsuarioId == userid);

            if (votoActual is null)
            {
                var votopelicula = mapper.Map<VotoPelicula>(votoPeliculaDTO);
                votopelicula.UsuarioId = userid;
                votopelicula.FechaVoto = DateTime.Now;
                context.Add(votopelicula);
            }
            else
            {
                votoActual.FechaVoto = DateTime.Now;
                votoActual.Voto = votoPeliculaDTO.Voto;
            }

            await context.SaveChangesAsync();
        }

    }
}
