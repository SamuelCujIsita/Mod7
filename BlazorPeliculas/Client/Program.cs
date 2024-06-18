using BlazorPeliculas.Client;
using BlazorPeliculas.Client.Auth;
using BlazorPeliculas.Client.Repositorios;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddSingleton(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });//se modifico a singleton
ConfigureServices(builder.Services);

await builder.Build().RunAsync();

void ConfigureServices(IServiceCollection services)
{
    services.AddSweetAlert2();
    services.AddScoped<IRepositorio, Repositorio>();
    services.AddAuthorizationCore();

    services.AddScoped<ProveedorAutenticacionJWT>();//Configurar la dependecia del provedor de auth
    services.AddScoped<AuthenticationStateProvider, ProveedorAutenticacionJWT>(
        provedor => provedor.GetRequiredService<ProveedorAutenticacionJWT>());

    services.AddScoped<ILoginService, ProveedorAutenticacionJWT>(
        provedor => provedor.GetRequiredService<ProveedorAutenticacionJWT>());

    services.AddScoped<RenovadorToken>();
}