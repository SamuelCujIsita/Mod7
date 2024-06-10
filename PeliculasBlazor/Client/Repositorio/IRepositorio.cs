using PeliculasBlazor.Shared.Entidades;

namespace PeliculasBlazor.Client.Repositorio
{
    public interface IRepositorio
    {
        List<Pelicula> ObtenerPeliculas();
    }
}
