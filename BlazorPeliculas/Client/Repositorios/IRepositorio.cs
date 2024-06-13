using BlazorPeliculas.Shared.Entidades;

namespace BlazorPeliculas.Client.Repositorios
{
    public interface IRepositorio
    {
        //Borrar Registros
        Task<HttpResponseWrapper<object>> Delete(string url);
        //obtener todo
        Task<HttpResponseWrapper<T>> Get<T>(string url);
        //peliculas en memoria
        List<Pelicula> ObtenerPeliculas();
        //Guardar 
        Task<HttpResponseWrapper<object>> Post<T>(string url, T enviar);
        //Guardar
        Task<HttpResponseWrapper<TResponse>> Post<T, TResponse>(string url, T enviar);
        //Actualizar registros 
        Task<HttpResponseWrapper<object>> Put<T>(string url, T enviar);
    }
}
