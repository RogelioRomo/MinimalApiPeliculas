using System;
using AutoMapper;
using MinimalApiPeliculas.DTOs;
using MinimalApiPeliculas.Entidades;

namespace MinimalApiPeliculas.Utilidades;

public class AutoMapperProfiles : Profile
{
  public AutoMapperProfiles()
  {
    CreateMap<CrearGeneroDTO, Genero>();
    CreateMap<Genero, GeneroDTO>();
  }
}
