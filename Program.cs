using Microsoft.AspNetCore.Cors;
using MinimalApiPeliculas.Entidades;

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

app.MapGet("/generos", () =>
{
  var generos = new List<Genero>
  {
    new Genero
    {
      Id = 1,
      Nombre = "Drama"
    },
    new Genero
    {
      Id = 2,
      Nombre = "Acción"
    },
    new Genero
    {
      Id = 3,
      Nombre = "Comedia"
    }
  };
  return generos;
}).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(15))); // usamos el output cache service aqui

// FIN DE AREA DE MIDDLEWARES

app.Run();
