using PeliculasBlazor.Shared.Entidades;

namespace PeliculasBlazor.Client.Repositorio
{
    public class Repositorio : IRepositorio
    {
        public List<Pelicula> ObtenerPeliculas()
        {
            return new List<Pelicula>()
        {
            new Pelicula{Titulo="Volver al futuro", FechaLanzamiento=new DateTime(1985, 4, 20)},
            new Pelicula{Titulo="Avengers", FechaLanzamiento=new DateTime(2020, 9, 4) },
            new Pelicula{Titulo="Harry Potter y la camara secreta", FechaLanzamiento=new DateTime(2001, 11, 4) }
        };
        }
    }
}
