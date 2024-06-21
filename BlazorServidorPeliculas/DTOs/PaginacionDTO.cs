using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorServidorPeliculas.DTOs
{
    public class PaginacionDTO
    {
        public int pagina { get; set; } = 1;
        public int CantidadRegistro { get; set; } = 5;
    }
}
