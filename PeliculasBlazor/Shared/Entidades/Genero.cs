using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeliculasBlazor.Shared.Entidades
{
    public class Genero
    {
        public int Id { get; set; }
        [Required]
        public string Nombre { get; set; }

    }
}
