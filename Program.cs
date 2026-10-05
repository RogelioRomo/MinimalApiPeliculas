using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using MinimalApiPeliculas.Endpoints;
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
builder.Services.AddScoped<IRepositorioActores, RepositorioActores>();

builder.Services.AddAutoMapper(typeof(Program));

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

app.MapGroup("/generos").MapGeneros();


// FIN DE AREA DE MIDDLEWARES

app.Run();
