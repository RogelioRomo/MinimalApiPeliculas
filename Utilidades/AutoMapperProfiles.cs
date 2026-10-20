using System;
using AutoMapper;
using MinimalApiPeliculas.DTOs;
using MinimalApiPeliculas.Entidades;

namespace MinimalApiPeliculas.Utilidades;

public class AutoMapperProfiles : Profile
{
  public AutoMapperProfiles()
  {
    //Generos
    CreateMap<CrearGeneroDTO, Genero>();
    CreateMap<Genero, GeneroDTO>();

    //Actores
    CreateMap<CrearActorDTO, Actor>()
    .ForMember(x => x.Foto, opciones => opciones.Ignore());
    CreateMap<Actor, ActorDTO>();

    //Peliculas
    CreateMap<CrearPeliculaDTO, Pelicula>()
    .ForMember(x => x.Poster, opciones => opciones.Ignore());
    CreateMap<Pelicula, PeliculaDTO>();
  }
}
