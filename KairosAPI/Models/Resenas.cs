using System;
using System.Collections.Generic;

namespace KairosAPI.Models;

public partial class Resenas
{
    public int IdResena { get; set; }

    public string UsuarioNombre { get; set; } = null!;

    public string? Rol { get; set; }

    public string Comentario { get; set; } = null!;

    public int? Estrellas { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public bool? Estatus { get; set; }
}