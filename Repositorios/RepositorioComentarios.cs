using System;
using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using MinimalApiPeliculas.Entidades;

namespace MinimalApiPeliculas.Repositorios;

public class RepositorioComentarios : IRepositorioComentarios
{
  private readonly string connectionString;
  public RepositorioComentarios(IConfiguration configuration)
  {
    connectionString = configuration.GetConnectionString("DefaultConnection")!;
  }

  public async Task<List<Comentario>> ObtenerTodos(int peliculaId)
  {
    using (var connection = new SqlConnection(connectionString))
    {
      var comentarios = await connection.QueryAsync<Comentario>("Comentarios_ObtenerTodos", new { peliculaId }, commandType: CommandType.StoredProcedure);
      return comentarios.ToList();
    }
  }

  public async Task<Comentario?> ObtenerPorId(int id)
  {
    using (var connection = new SqlConnection(connectionString))
    {
      var comentario = await connection.QueryFirstOrDefaultAsync<Comentario>("Comentarios_ObtenerPorId", new { id }, commandType: CommandType.StoredProcedure);
      return comentario;
    }
  }

  public async Task<int> Crear(Comentario comentario)
  {
    using (var connection = new SqlConnection(connectionString))
    {
      var id = await connection.QuerySingleAsync<int>("Comentarios_Crear", new { comentario.Cuerpo, comentario.PeliculaId }, commandType: CommandType.StoredProcedure);
      comentario.Id = id;
      return id;
    }
  }

  public async Task<bool> Existe(int id)
  {
    using (var connection = new SqlConnection(connectionString))
    {
      var existe = await connection.QuerySingleAsync<bool>("Comentarios_ExistePorId", new { id }, commandType: CommandType.StoredProcedure);
      return existe;
    }
  }

  public async Task Actualizar(Comentario comentario)
  {
    using (var connection = new SqlConnection(connectionString))
    {
      await connection.ExecuteAsync("Comentarios_Actualizar", comentario, commandType: CommandType.StoredProcedure);
    }
  }

  public async Task Borrar(int id)
  {
    using (var connection = new SqlConnection(connectionString))
    {
      await connection.ExecuteAsync("Comentarios_Borrar", new { id }, commandType: CommandType.StoredProcedure);
    }
  }
}
