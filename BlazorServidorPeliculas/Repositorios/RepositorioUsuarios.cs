using BlazorServidorPeliculas.Data;
using BlazorServidorPeliculas.DTOs;
using BlazorServidorPeliculas.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BlazorServidorPeliculas.Repositorios
{
    public class RepositorioUsuarios
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<IdentityUser> userManager;

        public RepositorioUsuarios(ApplicationDbContext context, 
            UserManager<IdentityUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }


        public async Task<RespuestaPaginadaDTO<UsuarioDTO>> Get(PaginacionDTO paginacion)
        {
            var queryable = context.Users.AsQueryable();
            var respuesta = new RespuestaPaginadaDTO<UsuarioDTO>();
            respuesta.TotalPaginas = await queryable.CalcularTotalPaginas(paginacion.CantidadRegistro);
            respuesta.Registros = await queryable.Paginar(paginacion).Select(x => new UsuarioDTO { Id = x.Id, Email = x.Email! }).ToListAsync();
            return respuesta;
        }

        public async Task<List<RolDTO>> GetRoles()
        {
            return await context.Roles.Select(x => new RolDTO { Nombre = x.Name! }).ToListAsync();

        }

        public async Task AsignarRolUsuario(EditarRolDTO editarRolDTO)
        {
            var usuario = await userManager.FindByIdAsync(editarRolDTO.UsuarioId);
            await userManager.AddClaimAsync(usuario, new Claim(ClaimTypes.Role, editarRolDTO.Rol));
            await userManager.AddToRoleAsync(usuario, editarRolDTO.Rol);

        }

        public async Task RemoverRolUsuario(EditarRolDTO editarRolDTO)
        {
            var usuario = await userManager.FindByIdAsync(editarRolDTO.UsuarioId);
            await userManager.RemoveClaimAsync(usuario, new Claim(ClaimTypes.Role, editarRolDTO.Rol));
            await userManager.RemoveFromRoleAsync(usuario, editarRolDTO.Rol);
        }
    }
}
