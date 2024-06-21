using BlazorServidorPeliculas.Entidades;
using BlazorServidorPeliculas.Data;
using BlazorServidorPeliculas.DTOs;
using BlazorServidorPeliculas.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorServidorPeliculas.Repositorios
{
    public class RepositorioGenero
    {
        public readonly ApplicationDbContext _context;
        public RepositorioGenero(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Genero>> GetGeneros()
        {
            return await _context.Generos.AsNoTracking().ToListAsync();//no darle seguimiento a los generos que envia 
        }

        public async Task<Genero> Get(int id)
        {
            return await _context.Generos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<int> Post(Genero genero)
        {
            _context.Add(genero);
            await _context.SaveChangesAsync();
            return genero.Id;
        }

        public async Task Put(Genero genero)
        {
            _context.Attach(genero).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var existe = await _context.Generos.AnyAsync(x => x.Id == id);
            if(!existe)
            {
                throw new ApplicationException($"Genero {id} no encontrado");
            }
            _context.Remove(new Genero { Id = id });
            await _context.SaveChangesAsync();
        }

    }
}
