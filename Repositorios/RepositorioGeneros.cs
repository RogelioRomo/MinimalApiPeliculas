using System;
using System.Data.Common;
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
      var generos = await connection.QueryAsync<Genero>(@"SELECT Id, Nombre
      FROM Generos");

      return generos.ToList();
    }
  }

  public async Task<Genero?> ObtenerPorId(int id)
  {
    using (var connection = new SqlConnection(connectionString))
    {
      var genero = await connection.QueryFirstOrDefaultAsync<Genero>(@"SELECT Id, Nombre FROM Generos WHERE Id = @Id", new { id });
      return genero;
    }
  }

  public async Task<int> CrearGenero(Genero genero)
  {
    using (var connection = new SqlConnection(connectionString))
    {
      var id = await connection.QuerySingleAsync<int>(@"INSERT INTO Generos (Nombre) VALUES (@Nombre); SELECT SCOPE_IDENTITY();", genero);
      genero.Id = id;
      return id;
    }
  }
}
