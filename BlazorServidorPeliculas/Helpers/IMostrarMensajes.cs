namespace BlazorServidorPeliculas.Helpers;

public interface IMostrarMensajes
{
    Task MostrarMensajeError(string msg);
    Task MostrarMensajeExitoso(string msg);

}
