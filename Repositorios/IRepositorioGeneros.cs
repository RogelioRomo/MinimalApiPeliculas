using System;
using MinimalApiPeliculas.Entidades;

namespace MinimalApiPeliculas.Repositorios;

public interface IRepositorioGeneros
{
  Task<int> CrearGenero(Genero genero);
  Task<Genero?> ObtenerPorId(int id);
  Task<List<Genero>> ObtenerTodos();
}
