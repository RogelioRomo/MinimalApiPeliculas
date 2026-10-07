using System;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using MinimalApiPeliculas.DTOs;
using MinimalApiPeliculas.Entidades;
using MinimalApiPeliculas.Repositorios;
using MinimalApiPeliculas.Servicios;

namespace MinimalApiPeliculas.Endpoints;

public static class ActoresEndpoints
{
  private static readonly string contenedor = "actores";
  public static RouteGroupBuilder MapActores(this RouteGroupBuilder group)
  {
    //el antiforgery es un sistema de seguridad que para esta api necesitamos deshabilitar
    //para que nos permita subir fotografias
    group.MapPost("/", Crear).DisableAntiforgery();
    return group;
  }

  static async Task<Created<ActorDTO>> Crear([FromForm] CrearActorDTO crearActorDTO, IRepositorioActores repositorio, IOutputCacheStore outputCacheStore, IMapper mapper, IAlmacenadorArchivos almacenadorArchivos)
  {
    var actor = mapper.Map<Actor>(crearActorDTO);

    if (crearActorDTO.Foto is not null)
    {
      var url = await almacenadorArchivos.Almacenar(contenedor, crearActorDTO.Foto);
      actor.Foto = url;
    }

    var id = await repositorio.Crear(actor);
    await outputCacheStore.EvictByTagAsync("actores-get", default);
    var actorDTO = mapper.Map<ActorDTO>(actor);
    return TypedResults.Created($"/actores/{id}", actorDTO);
  }
}
