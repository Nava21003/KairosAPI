using System;
using System.Collections.Generic;

namespace KairosAPI.Models;

public partial class Lugare
{
    public int IdLugar { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public int? IdCategoria { get; set; }

    public decimal? Latitud { get; set; }

    public decimal? Longitud { get; set; }

    public string? Direccion { get; set; }

    public string? Horario { get; set; }

    public string? Imagen { get; set; }

    public bool? EsPatrocinado { get; set; }

    public bool? Estatus { get; set; }

    public virtual ICollection<Actividades> Actividades { get; set; } = new List<Actividades>();

    public virtual Categoria? IdCategoriaNavigation { get; set; }

    public virtual ICollection<Promocione> Promociones { get; set; } = new List<Promocione>();

    public virtual ICollection<RutasLugare> RutasLugares { get; set; } = new List<RutasLugare>();

    public virtual ICollection<PuntoInteres> PuntosInteres { get; set; } = new List<PuntoInteres>();
}