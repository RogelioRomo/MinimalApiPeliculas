using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.OutputCaching;
using MinimalApiPeliculas.Entidades;
using MinimalApiPeliculas.Repositorios;

var builder = WebApplication.CreateBuilder(args);
var workingEnvironment = builder.Configuration.GetValue<string>("workingEnvironment");
var allowedOrigins = builder.Configuration.GetValue<string>("allowedOrigins")!;

//INICIO DE AREA DE LOS SERVICIOS

builder.Services.AddCors(options =>
{
  //solo para los origenes permitidos en el archivo de appSettings
  options.AddDefaultPolicy(configuration =>
  {
    configuration.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod();
  });

  //para cualquier origen
  options.AddPolicy("anyOrigin", configuration =>
  {
    configuration.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
  });
});

// output cache service
builder.Services.AddOutputCache();

//SWAGGER service
//http://localhost:5282/swagger/index.html
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IRepositorioGeneros, RepositorioGeneros>();

// FIN DE AREA DE LOS SERVICIOS

var app = builder.Build();

// INICIO DE AREA DE MIDDLEWARES

if (builder.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseCors();

app.UseOutputCache();

app.MapGet("/", [EnableCors(policyName: "anyOrigin")] () => workingEnvironment);

app.MapGet("/generos", async (IRepositorioGeneros repositorio) =>
{
  return await repositorio.ObtenerTodos();
}).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("generos-get")); // usamos el output cache service aqui y usamos un tag para limpiar el cache donde se ejecute use el tag

app.MapGet("/generos/{id:int}", async (int id, IRepositorioGeneros repositorio) =>
{
  var genero = await repositorio.ObtenerPorId(id);
  if (genero is null)
  {
    return Results.NotFound();
  }
  return Results.Ok(genero);
});

app.MapPost("/generos", async (Genero genero, IRepositorioGeneros repositorioGeneros, IOutputCacheStore outputCacheStore) =>
{
  var id = await repositorioGeneros.CrearGenero(genero);
  await outputCacheStore.EvictByTagAsync("generos-get", default); //limpiamos cache con tag cuando creamos un nuevo recurso
  return TypedResults.Created($"/generos/{id}", genero); //recurso creado en ese URI
});

// FIN DE AREA DE MIDDLEWARES

app.Run();
