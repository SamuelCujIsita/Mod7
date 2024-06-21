using BlazorServidorPeliculas.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorServidorPeliculas.DTOs
{
    public class PeliculaActualizaconDTO
    {
        public Pelicula Pelicula { get; set; }
        public List<Actor> Actores { get; set; } = new List<Actor>();
        public List<Genero> GeneroSeleccionados { get; set; } = new List<Genero>();
        public List<Genero> GeneroNoSeleccionados { get; set; } = new List<Genero>();

    }
}
