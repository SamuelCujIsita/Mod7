using BlazorPeliculas.Shared.Entidades;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlazorPeliculas.Server
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); //NO BORRAR NUNCA

            modelBuilder.Entity <GeneroPelicula>().HasKey(e => new {e.GeneroId, e.PeliculaId});
            modelBuilder.Entity <PeliculaActor>().HasKey(e => new {e.ActorId, e.PeliculaId});
        }
        public DbSet<Genero> Generos => Set<Genero>();
        public DbSet<Actor> Actores => Set<Actor>();
        public DbSet<Pelicula> Peliculas => Set<Pelicula>();
        public DbSet<VotoPelicula> VotosPeliculas => Set<VotoPelicula>();
        public DbSet<GeneroPelicula> GenerosPeliculas => Set<GeneroPelicula>();
        public DbSet<PeliculaActor> PeliculasActores => Set<PeliculaActor>();
    }
}
