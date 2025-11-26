using System;
using System.Collections.Generic;

namespace KairosAPI.Models;

public partial class PuntoInteres
{
    public int IdPunto { get; set; }

    public int IdLugar { get; set; }

    public string Etiqueta { get; set; } = null!;

    public string? Descripcion { get; set; }

    public int? Prioridad { get; set; }

    public bool? Estatus { get; set; }

    public virtual Lugare? IdLugarNavigation { get; set; }
}