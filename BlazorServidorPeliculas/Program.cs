using BlazorServidorPeliculas.Areas.Identity;
using BlazorServidorPeliculas.Data;
using BlazorServidorPeliculas.Helpers;
using BlazorServidorPeliculas.Repositorios;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
//optionsBuilder.EnableSensitiveDataLogging();
optionsBuilder.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found."));

builder.Services.AddTransient(_ => new ApplicationDbContext(optionsBuilder.Options));
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddTransient<RepositorioUsuarios>();
builder.Services.AddTransient<RepositorioGenero>();
builder.Services.AddTransient<RepositorioActores>();
builder.Services.AddTransient<RepositorioPeliculas>();
builder.Services.AddTransient<AuthenticationStateService>();
builder.Services.AddTransient<RepositorioVotos>();
builder.Services.AddScoped<IAlmacenadorArchivos, AlmacenadorArchivosLocales>();
builder.Services.AddScoped<IMostrarMensajes, MostrarMensajes>();
builder.Services.AddAutoMapper(typeof(Program));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
