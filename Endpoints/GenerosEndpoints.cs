using System;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
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
  static async Task<Ok<List<Genero>>> ObtenerGeneros(IRepositorioGeneros repositorio)
  {
    var generos = await repositorio.ObtenerTodos();
    return TypedResults.Ok(generos);
  }

  static async Task<Results<Ok<Genero>, NotFound>> ObtenerGeneroPorId(IRepositorioGeneros repositorio, int id)
  {
    var genero = await repositorio.ObtenerPorId(id);
    if (genero is null)
    {
      return TypedResults.NotFound();
    }
    return TypedResults.Ok(genero);
  }

  static async Task<Created<Genero>> CrearGenero(Genero genero, IRepositorioGeneros repositorioGeneros, IOutputCacheStore outputCacheStore)
  {
    var id = await repositorioGeneros.CrearGenero(genero);
    await outputCacheStore.EvictByTagAsync("generos-get", default); //limpiamos cache con tag cuando creamos un nuevo recurso
    return TypedResults.Created($"/generos/{id}", genero); //recurso creado en ese URI
  }

  static async Task<Results<NoContent, NotFound>> ActualizarGenero(int id, Genero genero, IRepositorioGeneros repositorio, IOutputCacheStore outputCacheStore)
  {
    var existe = await repositorio.Existe(id);
    if (!existe)
    {
      return TypedResults.NotFound();
    }
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
