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

public static class PeliculasEndpoints
{
  private static readonly string contenedor = "peliculas";

  public static RouteGroupBuilder MapPeliculas(this RouteGroupBuilder group)
  {
    group.MapPost("/", Crear).DisableAntiforgery();
    group.MapGet("/", ObtenerTodos).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("peliculas-get"));
    group.MapGet("/{id:int}", ObtenerPorId);
    group.MapPut("/{id:int}", Actualizar).DisableAntiforgery();
    group.MapDelete("/{id:int}", Borrar);
    return group;
  }

  static async Task<Created<PeliculaDTO>> Crear([FromForm] CrearPeliculaDTO crearPeliculaDTO, IRepositorioPeliculas repositorio, IAlmacenadorArchivos almacenadorArchivos, IOutputCacheStore outputCacheStore, IMapper mapper)
  {
    var pelicula = mapper.Map<Pelicula>(crearPeliculaDTO);

    if (crearPeliculaDTO.Poster is not null)
    {
      var url = await almacenadorArchivos.Almacenar(contenedor, crearPeliculaDTO.Poster);
      pelicula.Poster = url;
    }

    var id = await repositorio.Crear(pelicula);
    await outputCacheStore.EvictByTagAsync("peliculas-get", default);
    var peliculaDTO = mapper.Map<PeliculaDTO>(pelicula);
    return TypedResults.Created($"/peliculas/{id}", peliculaDTO);
  }

  static async Task<Ok<List<PeliculaDTO>>> ObtenerTodos(IRepositorioPeliculas repositorio, IMapper mapper, int pagina = 1, int recordsPorPagina = 10)
  {
    var paginacion = new PaginacionDTO { Pagina = pagina, RecordsPorPagina = recordsPorPagina };
    var peliculas = await repositorio.ObtenerTodos(paginacion);
    var peliculasDTO = mapper.Map<List<PeliculaDTO>>(peliculas);
    return TypedResults.Ok(peliculasDTO);
  }

  static async Task<Results<Ok<PeliculaDTO>, NotFound>> ObtenerPorId(int id, IRepositorioPeliculas repositorio, IMapper mapper)
  {
    var pelicula = await repositorio.ObtenerPorId(id);

    if (pelicula is null)
    {
      return TypedResults.NotFound();
    }

    var peliculaDTO = mapper.Map<PeliculaDTO>(pelicula);
    return TypedResults.Ok(peliculaDTO);
  }

  static async Task<Results<NoContent, NotFound>> Actualizar(int id, [FromForm] CrearPeliculaDTO crearPeliculaDTO, IRepositorioPeliculas repositorio, IAlmacenadorArchivos almacenadorArchivos, IOutputCacheStore outputCacheStore, IMapper mapper)
  {
    var peliculaDB = await repositorio.ObtenerPorId(id);

    if (peliculaDB is null)
    {
      return TypedResults.NotFound();
    }

    var peliculaParaActualizar = mapper.Map<Pelicula>(crearPeliculaDTO);
    peliculaParaActualizar.Id = id;
    peliculaParaActualizar.Poster = peliculaDB.Poster;

    if (crearPeliculaDTO.Poster is not null)
    {
      var url = await almacenadorArchivos.Editar(peliculaParaActualizar.Poster, contenedor, crearPeliculaDTO.Poster);
      peliculaParaActualizar.Poster = url;
    }

    await repositorio.Actualizar(peliculaParaActualizar);
    await outputCacheStore.EvictByTagAsync("peliculas-get", default);
    return TypedResults.NoContent();
  }

  static async Task<Results<NoContent, NotFound>> Borrar(int id, IRepositorioPeliculas repositorio, IAlmacenadorArchivos almacenadorArchivos, IOutputCacheStore outputCacheStore)
  {
    var peliculaDB = await repositorio.ObtenerPorId(id);

    if (peliculaDB is null)
    {
      return TypedResults.NotFound();
    }

    await repositorio.Borrar(id);
    await almacenadorArchivos.Borrar(peliculaDB.Poster, contenedor);
    await outputCacheStore.EvictByTagAsync("peliculas-get", default);
    return TypedResults.NoContent();
  }
}
