using System;
using System.Collections.Generic;

namespace KairosAPI.Models;

public partial class MensajesContacto
{
    public int IdMensaje { get; set; }

    public string Nombre { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public string? Asunto { get; set; }

    public string? Mensaje { get; set; }

    public DateTime? FechaEnvio { get; set; }

    public string? Estatus { get; set; }
}