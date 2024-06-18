using AutoMapper;
using BlazorPeliculas.Shared.DTOs;
using BlazorPeliculas.Shared.Entidades;

namespace BlazorPeliculas.Server.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Actor, Actor>()//configurar el mapeo en este caso es de actor a actor
                .ForMember(x => x.Foto, option => option.Ignore());//ignoramos el campo foto


            CreateMap<Pelicula, Pelicula>()//configurar el mapeo en este caso es de actor a actor
                .ForMember(x => x.Poster, option => option.Ignore());//ignoramos el campo foto

            CreateMap<VotoPeliculaDTO, VotoPelicula>();
        }

    }
}
