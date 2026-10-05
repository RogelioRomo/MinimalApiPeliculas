using System;
using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using MinimalApiPeliculas.Entidades;

namespace MinimalApiPeliculas.Repositorios;

public class RepositorioActores : IRepositorioActores
{
  private readonly string connectionString;
  public RepositorioActores(IConfiguration configuration)
  {
    connectionString = configuration.GetConnectionString("DefaultConnection")!;
  }

  public async Task<List<Actor>> ObtenerTodos()
  {
    using (var connection = new SqlConnection(connectionString))
    {
      var actores = await connection.QueryAsync<Actor>("Actores_ObtenerTodos", commandType: CommandType.StoredProcedure);
      return actores.ToList();
    }
  }

  public async Task<Actor?> ObtenerPorId(int id)
  {
    using (var connection = new SqlConnection(connectionString))
    {
      var actor = await connection.QueryFirstOrDefaultAsync<Actor>("Actores_ObtenerPorId", new { id }, commandType: CommandType.StoredProcedure);
      return actor;
    }
  }

  public async Task<int> Crear(Actor actor)
  {
    using (var connection = new SqlConnection(connectionString))
    {
      var id = await connection.QuerySingleAsync<int>("Actores_Crear", new { actor.Nombre, actor.FechaNacimiento, actor.Foto }, commandType: CommandType.StoredProcedure);
      actor.Id = id;
      return id;
    }
  }

  public async Task Actualizar(Actor actor)
  {
    using (var connection = new SqlConnection(connectionString))
    {
      await connection.ExecuteAsync("Actores_Actualizar", actor, commandType: CommandType.StoredProcedure);
    }
  }

  public async Task<bool> Existe(int id)
  {
    using (var connection = new SqlConnection(connectionString))
    {
      var existe = await connection.QuerySingleAsync("Actores_Existe", new { id }, commandType: CommandType.StoredProcedure);
      return existe;
    }
  }

  public async Task Borrar(int id)
  {
    using (var connection = new SqlConnection(connectionString))
    {
      //recordar que se usar ExecuteAsync porque no queremos retornar nada
      await connection.ExecuteAsync("Actores_Borrar", new { id }, commandType: CommandType.StoredProcedure);
    }
  }
}
