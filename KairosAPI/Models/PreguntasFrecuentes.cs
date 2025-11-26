using System;
using System.Collections.Generic;

namespace KairosAPI.Models;

public partial class PreguntasFrecuentes
{
    public int IdPregunta { get; set; }

    public string Pregunta { get; set; } = null!;

    public string Respuesta { get; set; } = null!;

    public string? Categoria { get; set; }

    public int? Orden { get; set; }

    public bool? Estatus { get; set; }
}