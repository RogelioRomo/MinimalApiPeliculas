using System;
using MinimalApiPeliculas.Entidades;

namespace MinimalApiPeliculas.Repositorios;

public interface IRepositorioActores
{
  Task Actualizar(Actor actor);
  Task Borrar(int id);
  Task<int> Crear(Actor actor);
  Task<bool> Existe(int id);
  Task<Actor?> ObtenerPorId(int id);
  Task<List<Actor>> ObtenerTodos();
}
