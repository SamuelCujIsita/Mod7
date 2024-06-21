using BlazorServidorPeliculas.DTOs;
using BlazorServidorPeliculas.Shared;
using Microsoft.EntityFrameworkCore;


namespace BlazorServidorPeliculas.Helpers
{
    public static class IQueryableExtensions
    {
        public async static Task<int> CalcularTotalPaginas<T>(this IQueryable<T> queryable, int cantidadRegistrosAMostrar)
        {
            double conteo = await queryable.CountAsync();
            int totalPaginas = (int)Math.Ceiling(conteo / cantidadRegistrosAMostrar);
            return totalPaginas;
        }

        public static IQueryable<T> Paginar<T>(this IQueryable<T> queryable, PaginacionDTO paginacion)
        {
            return queryable
                    .Skip((paginacion.pagina - 1) * paginacion.CantidadRegistro)
                    .Take(paginacion.CantidadRegistro);
        }

    }
}
