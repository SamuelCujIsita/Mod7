using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace PeliculasBlazor.Client.Pages
{
    public partial class Counter
    {
        [Inject] ServiciosSingleton? singleton { get; set; }
        [Inject] ServiciosTransit? transient { get; set; }
        [Inject] IJSRuntime? js { get; set; }

        private int currentCount = 0;
        public static int CurrentCountStatic;
        private async Task IncrementCount()
        {
            currentCount++;
            CurrentCountStatic = currentCount;
            singleton.Valor = currentCount;
            transient.Valor = currentCount;
            await js.InvokeVoidAsync("pruebaPuntoNetStatic");
        }

        [JSInvokable]
        public static Task<int> ObtenerCurrentCount()
        {
            return Task.FromResult(CurrentCountStatic);
        }


    }
}
