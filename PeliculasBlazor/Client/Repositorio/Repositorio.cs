using PeliculasBlazor.Shared.Entidades;

namespace PeliculasBlazor.Client.Repositorio
{
    public class Repositorio : IRepositorio
    {
        public List<Pelicula> ObtenerPeliculas()
        {
            return new List<Pelicula>()
        {
            new Pelicula{Titulo="Volver al futuro", FechaLanzamiento=new DateTime(1985, 4, 20), Poster="https://upload.wikimedia.org/wikipedia/en/d/d2/Back_to_the_Future.jpg"  },
            new Pelicula{Titulo="Avengers", FechaLanzamiento=new DateTime(2020, 9, 4), Poster = "https://upload.wikimedia.org/wikipedia/en/8/8a/The_Avengers_%282012_film%29_poster.jpg" },
            new Pelicula{Titulo="Harry Potter y la camara secreta", FechaLanzamiento=new DateTime(2001, 11, 4), Poster = "https://upload.wikimedia.org/wikipedia/en/c/c0/Harry_Potter_and_the_Chamber_of_Secrets_movie.jpg" }
        };
        }
    }
}
