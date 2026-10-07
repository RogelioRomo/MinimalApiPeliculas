using System;

namespace MinimalApiPeliculas.DTOs;

public class CrearActorDTO
{
  public string Nombre { get; set; } = null!;
  public DateTime FechaNacimiento { get; set; }
  // este field no es igual a la entidad de actor debido a que el usuario
  // tiene que proporcionar primero la foto como archivo para despues generar el URL
  public IFormFile? Foto { get; set; }
}
