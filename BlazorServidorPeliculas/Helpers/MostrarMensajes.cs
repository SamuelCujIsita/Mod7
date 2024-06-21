using Microsoft.JSInterop;

namespace BlazorServidorPeliculas.Helpers
{
    public class MostrarMensajes : IMostrarMensajes
    {
        private readonly IJSRuntime js;

        public MostrarMensajes(IJSRuntime js)
        {
            this.js = js;
        }
        public async Task MostrarMensajeError(string msg)
        {
            await MostrarMensaje("Error", msg, "error");
        }

        public async Task MostrarMensajeExitoso(string msg)
        {
            await MostrarMensaje("Exitoso", msg, "success");
        }

        private async ValueTask MostrarMensaje(string titulo, string mensaje, string tipoMensaje)
        {
            await js.InvokeVoidAsync("Swal.fire", titulo, mensaje, tipoMensaje);
        }
    }
}
