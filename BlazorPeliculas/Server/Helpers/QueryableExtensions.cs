using BlazorPeliculas.Shared.DTOs;
using Microsoft.Identity.Client;

namespace BlazorPeliculas.Server.Helpers
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> Paginar<T>(this IQueryable<T> queryable,
            PaginacionDTO paginacion)
        {
            return queryable
                .Skip((paginacion.pagina - 1) * paginacion.CantidadRegistro)
                .Take(paginacion.CantidadRegistro);
        }
    }
}
