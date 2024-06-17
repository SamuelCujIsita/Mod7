using BlazorPeliculas.Server;
using BlazorPeliculas.Server.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews()
    .AddJsonOptions(op => op.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
//Corregir error de referencias ciclicas
builder.Services.AddRazorPages();



builder.Services.AddDbContext<ApplicationDbContext>
    (options => options.UseSqlServer("name = DefaultConnection"));

builder.Services.AddTransient<IAlmacenadorArchivos, AlmacenadorArchivosLocales>();
builder.Services.AddHttpContextAccessor();//ACCEDER AL CONTEXTO HTTP Y CONTENER EL NOMBRE DEL HOST Y SABER SI ES HTTP O HTTPS

builder.Services.AddAutoMapper(typeof(Program));//config automapper

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(option =>
    option.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false, //validar emisor
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["jwtkey"])
            ),
        ClockSkew = TimeSpan.Zero
    }
    );

//jsonwebtoken -> string que contien cifrado los claims(info del user que puedo confiar) del usuario 

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();//descargar archivos estaticos del wwwwroot

app.UseRouting();

app.UseAuthorization();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
