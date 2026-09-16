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
      var generos = await connection.QueryAsync<Genero>(@"
      SELECT Id, Nombre
      FROM Generos ORDER BY Nombre
      ");
      return generos.ToList();
    }
  }

  public async Task<Genero?> ObtenerPorId(int id)
  {
    using (var connection = new SqlConnection(connectionString))
    {
      var genero = await connection.QueryFirstOrDefaultAsync<Genero>(@"
      SELECT Id, Nombre FROM Generos
      WHERE Id = @Id
      ", new { id });
      return genero;
    }
  }
  public async Task<int> CrearGenero(Genero genero)
  {
    using (var connection = new SqlConnection(connectionString))
    {
      var id = await connection.QuerySingleAsync<int>(@"
      INSERT INTO Generos (Nombre) VALUES (@Nombre);
      SELECT SCOPE_IDENTITY();
      ", genero);
      genero.Id = id;
      return id;
    }
  }

  public async Task<bool> Existe(int id)
  {
    using (var connection = new SqlConnection(connectionString))
    {
      var existe = await connection.QuerySingleAsync<bool>(@"
      IF EXISTS (SELECT 1 FROM Generos WHERE Id = @Id)
          SELECT 1
      ELSE
          SELECT 0
      ", new { id });
      return existe;
    }
  }

  public async Task Actualizar(Genero genero)
  {
    using (var connection = new SqlConnection(connectionString))
    {
      await connection.ExecuteAsync(@"
      UPDATE Generos
      SET Nombre = @Nombre
      WHERE Id = @Id
      ", genero);
    }
  }

  public async Task Borrar(int id)
  {
    using (var connection = new SqlConnection(connectionString))
    {
      await connection.ExecuteAsync(@"
      DELETE Generos
      WHERE Id = @Id
      ", new { id });
    }
  }

}
