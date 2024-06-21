using System.Collections.Generic;

namespace BlazorServidorPeliculas.DTOs
{
    public class RespuestaPaginadaDTO<T>
    {
        public int TotalPaginas { get; set; }
        public List<T> Registros { get; set; }
    }
}
