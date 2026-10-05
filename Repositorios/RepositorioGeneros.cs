using System;
using System.Data.Common;
using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using MinimalApiPeliculas.Entidades;

namespace MinimalApiPeliculas.Repositorios;

public class RepositorioGeneros : IRepositorioGeneros
{

  private readonly string? connectionString;

  public RepositorioGeneros(IConfiguration configuration)
  {
    connectionString = configuration.GetConnectionString("DefaultConnection");
  }

  public async Task<List<Genero>> ObtenerTodos()
  {
    using (var connection = new SqlConnection(connectionString))
    {
      var generos = await connection.QueryAsync<Genero>("Generos_ObtenerTodos", commandType: CommandType.StoredProcedure);
      return generos.ToList();
    }
  }

  public async Task<Genero?> ObtenerPorId(int id)
  {
    using (var connection = new SqlConnection(connectionString))
    {
      var genero = await connection.QueryFirstOrDefaultAsync<Genero>("Generos_ObtenerPorId", new { id }, commandType: CommandType.StoredProcedure);
      return genero;
    }
  }
  public async Task<int> CrearGenero(Genero genero)
  {
    using (var connection = new SqlConnection(connectionString))
    {
      var id = await connection.QuerySingleAsync<int>("Generos_CrearGenero", new { genero.Nombre }, commandType: CommandType.StoredProcedure);
      genero.Id = id;
      return id;
    }
  }

  public async Task<bool> Existe(int id)
  {
    using (var connection = new SqlConnection(connectionString))
    {
      var existe = await connection.QuerySingleAsync<bool>("Generos_Existe", new { id }, commandType: CommandType.StoredProcedure);
      return existe;
    }
  }

  public async Task Actualizar(Genero genero)
  {
    using (var connection = new SqlConnection(connectionString))
    {
      await connection.ExecuteAsync("Generos_Actualizar", genero, commandType: CommandType.StoredProcedure);
    }
  }

  public async Task Borrar(int id)
  {
    using (var connection = new SqlConnection(connectionString))
    {
      await connection.ExecuteAsync("Generos_Borrar", new { id }, commandType: CommandType.StoredProcedure);
    }
  }

}
