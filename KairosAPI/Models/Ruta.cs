using System;
using System.Collections.Generic;

namespace KairosAPI.Models;

public partial class Ruta
{
    public int IdRuta { get; set; }

    public int? IdUsuario { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public decimal? LatitudInicio { get; set; }

    public decimal? LongitudInicio { get; set; }

    public decimal? LatitudFin { get; set; }

    public decimal? LongitudFin { get; set; }

    public int? IdLugarInicio { get; set; }
    public int? IdLugarFin { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public string? Estatus { get; set; }

    public virtual Usuario? IdUsuarioNavigation { get; set; }
    public virtual Lugare? IdLugarInicioNavigation { get; set; }
    public virtual Lugare? IdLugarFinNavigation { get; set; }

    public virtual ICollection<RutasLugare> RutasLugares { get; set; } = new List<RutasLugare>();
}