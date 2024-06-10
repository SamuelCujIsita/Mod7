using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PeliculasBlazor.Client;
using PeliculasBlazor.Client.Repositorio;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
ConfigureServices(builder.Services);

await builder.Build().RunAsync();

void ConfigureServices(IServiceCollection services)
{
    services.AddSingleton<ServiciosSingleton>();
    services.AddTransient<ServiciosTransit>();
    services.AddSingleton<IRepositorio, Repositorio>();
}