using System;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.VisualBasic;
using MinimalApiPeliculas.DTOs;
using MinimalApiPeliculas.Entidades;
using MinimalApiPeliculas.Repositorios;

namespace MinimalApiPeliculas.Endpoints;

public static class GenerosEndpoints
{
  public static RouteGroupBuilder MapGeneros(this RouteGroupBuilder group)
  {
    group.MapGet("/", ObtenerGeneros).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("generos-get")); // usamos el output cache service aqui y usamos un tag para limpiar el cache donde se ejecute use el tag
    group.MapGet("/{id:int}", ObtenerGeneroPorId);
    group.MapPost("/", CrearGenero);
    group.MapPut("/{id:int}", ActualizarGenero);
    group.MapDelete("/{id:int}", BorrarGenero);
    return group;
  }

  static async Task<Ok<List<GeneroDTO>>> ObtenerGeneros(IRepositorioGeneros repositorio, IMapper mapper)
  {
    var generos = await repositorio.ObtenerTodos();

    var generosDTO = mapper.Map<List<GeneroDTO>>(generos);

    return TypedResults.Ok(generosDTO);
  }

  static async Task<Results<Ok<GeneroDTO>, NotFound>> ObtenerGeneroPorId(IRepositorioGeneros repositorio, int id, IMapper mapper)
  {
    var genero = await repositorio.ObtenerPorId(id);
    if (genero is null)
    {
      return TypedResults.NotFound();
    }

    var generoDTO = mapper.Map<GeneroDTO>(genero);
    return TypedResults.Ok(generoDTO);
  }

  static async Task<Created<GeneroDTO>> CrearGenero(CrearGeneroDTO crearGeneroDTO, IRepositorioGeneros repositorioGeneros, IOutputCacheStore outputCacheStore, IMapper mapper)
  {
    //mapeo con automapper
    var genero = mapper.Map<Genero>(crearGeneroDTO);
    var id = await repositorioGeneros.CrearGenero(genero);
    await outputCacheStore.EvictByTagAsync("generos-get", default); //limpiamos cache con tag cuando creamos un nuevo recurso

    var generoDTO = mapper.Map<GeneroDTO>(genero);
    return TypedResults.Created($"/generos/{id}", generoDTO); //recurso creado en ese URI
  }

  static async Task<Results<NoContent, NotFound>> ActualizarGenero(int id, CrearGeneroDTO crearGeneroDTO, IRepositorioGeneros repositorio, IOutputCacheStore outputCacheStore, IMapper mapper)
  {
    var existe = await repositorio.Existe(id);
    if (!existe)
    {
      return TypedResults.NotFound();
    }

    var genero = mapper.Map<Genero>(crearGeneroDTO);
    genero.Id = id;

    await repositorio.Actualizar(genero);
    await outputCacheStore.EvictByTagAsync("generos-get", default);
    return TypedResults.NoContent();
  }
  static async Task<Results<NoContent, NotFound>> BorrarGenero(int id, IRepositorioGeneros repositorio, IOutputCacheStore outputCacheStore)
  {
    var existe = await repositorio.Existe(id);
    if (!existe)
    {
      return TypedResults.NotFound();
    }
    await repositorio.Borrar(id);
    await outputCacheStore.EvictByTagAsync("generos-get", default);
    return TypedResults.NoContent();
  }

}
