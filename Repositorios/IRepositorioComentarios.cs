using System;
using MinimalApiPeliculas.Entidades;

namespace MinimalApiPeliculas.Repositorios;

public interface IRepositorioComentarios
{
  Task Actualizar(Comentario comentario);
  Task Borrar(int id);
  Task<int> Crear(Comentario comentario);
  Task<bool> Existe(int id);
  Task<Comentario?> ObtenerPorId(int id);
  Task<List<Comentario>> ObtenerTodos(int peliculaId);
}
