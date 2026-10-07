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
    group.MapGet("/", ObtenerTodos).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("actores-get"));
    group.MapGet("/{id:int}", ObtenerPorId);
    return group;
  }

  static async Task<Ok<List<ActorDTO>>> ObtenerTodos(IRepositorioActores repositorio, IMapper mapper)
  {
    var actores = await repositorio.ObtenerTodos();
    var actoresDTO = mapper.Map<List<ActorDTO>>(actores);
    return TypedResults.Ok(actoresDTO);
  }

  static async Task<Results<Ok<ActorDTO>, NotFound>> ObtenerPorId(int id, IRepositorioActores repositorio, IMapper mapper)
  {
    var actor = await repositorio.ObtenerPorId(id);

    if (actor is null)
    {
      return TypedResults.NotFound();
    }

    var actorDTO = mapper.Map<ActorDTO>(actor);
    return TypedResults.Ok(actorDTO);
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
