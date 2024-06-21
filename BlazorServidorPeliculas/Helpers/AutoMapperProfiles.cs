using AutoMapper;
using BlazorServidorPeliculas.DTOs;
using BlazorServidorPeliculas.Entidades;

namespace BlazorServidorPeliculas.Helpers;

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
