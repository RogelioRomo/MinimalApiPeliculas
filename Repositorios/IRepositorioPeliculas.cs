using System;
using MinimalApiPeliculas.DTOs;
using MinimalApiPeliculas.Entidades;

namespace MinimalApiPeliculas.Repositorios;

public interface IRepositorioPeliculas
{
  Task Actualizar(Pelicula pelicula);
  Task AsignarGeneros(int id, List<int> generosIds);
  Task Borrar(int id);
  Task<int> Crear(Pelicula pelicula);
  Task<bool> Existe(int id);
  Task<Pelicula?> ObtenerPorId(int id);
  Task<List<Pelicula>> ObtenerTodos(PaginacionDTO paginacionDTO);
}
