using Microsoft.JSInterop;
using System.Runtime.CompilerServices;

namespace PeliculasBlazor.Client.Helpers
{
    public static class IJSRuntimeExtensionMethods
    {
        /* Encapsular metodos de js en clase */
        public static async ValueTask<bool> Confirm(this IJSRuntime js, string message)
        {
            await js.InvokeVoidAsync("console.log", "PRUEBA DE JS CONSOLA");
            return await js.InvokeAsync<bool>("confirm", message);
        }
    }
}
