using System;
using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using MinimalApiPeliculas.DTOs;
using MinimalApiPeliculas.Entidades;

namespace MinimalApiPeliculas.Repositorios;

public class RepositorioPeliculas : IRepositorioPeliculas
{
  private readonly string connectionString;
  private readonly HttpContext httpContext;

  public RepositorioPeliculas(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
  {
    connectionString = configuration.GetConnectionString("DefaultConnection")!;
    httpContext = httpContextAccessor.HttpContext!;
  }

  public async Task<List<Pelicula>> ObtenerTodos(PaginacionDTO paginacionDTO)
  {
    using (var connection = new SqlConnection(connectionString))
    {
      var peliculas = await connection.QueryAsync<Pelicula>("Peliculas_ObtenerTodos", new { paginacionDTO.Pagina, paginacionDTO.RecordsPorPagina }, commandType: CommandType.StoredProcedure);

      var cantidadPeliculas = await connection.QuerySingleAsync<int>("Peliculas_Cantidad", commandType: CommandType.StoredProcedure);

      httpContext.Response.Headers.Append("cantidadTotalRegistros", cantidadPeliculas.ToString());

      return peliculas.ToList();
    }
  }

  public async Task<Pelicula?> ObtenerPorId(int id)
  {
    using (var connection = new SqlConnection(connectionString))
    {
      using (var multi = await connection.QueryMultipleAsync("Peliculas_ObtenerPorId", new { id }, commandType: CommandType.StoredProcedure))
      {
        var pelicula = await multi.ReadFirstAsync<Pelicula>();
        var comentarios = await multi.ReadAsync<Comentario>();
        pelicula.Comentarios = comentarios.ToList();
        return pelicula;
      }
    }
  }

  public async Task<int> Crear(Pelicula pelicula)
  {
    using (var connection = new SqlConnection(connectionString))
    {
      var id = await connection.QuerySingleAsync<int>("Peliculas_Crear", new
      {
        pelicula.Titulo,
        pelicula.Poster,
        pelicula.FechaLanzamiento,
        pelicula.EnCines
      }, commandType: CommandType.StoredProcedure);

      pelicula.Id = id;
      return id;
    }
  }

  public async Task<bool> Existe(int id)
  {
    using (var connection = new SqlConnection(connectionString))
    {
      var existe = await connection.QuerySingleAsync<bool>("Peliculas_ExistePorId", new { id }, commandType: CommandType.StoredProcedure);

      return existe;
    }
  }

  public async Task Actualizar(Pelicula pelicula)
  {
    using (var connection = new SqlConnection(connectionString))
    {
      await connection.ExecuteAsync("Peliculas_Actualizar", pelicula, commandType: CommandType.StoredProcedure);
    }
  }

  public async Task Borrar(int id)
  {
    using (var connection = new SqlConnection(connectionString))
    {
      await connection.ExecuteAsync("Peliculas_Borrar", new { id }, commandType: CommandType.StoredProcedure);
    }
  }

  public async Task AsignarGeneros(int id, List<int> generosIds)
  {
    var dt = new DataTable();
    dt.Columns.Add("Id", typeof(int));

    foreach (var generoId in generosIds)
    {
      dt.Rows.Add(generoId);
    }

    using (var connection = new SqlConnection(connectionString))
    {
      await connection.ExecuteAsync("Peliculas_AsignarGeneros", new { peliculaId = id, generosIds = dt }, commandType: CommandType.StoredProcedure);
    }
  }
}
