using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace KairosAPI.Models;

public partial class RutasLugare
{
    public int IdRuta { get; set; }

    public int IdLugar { get; set; }

    public int? Orden { get; set; }

    [JsonIgnore]
    public virtual Lugare? IdLugarNavigation { get; set; }

    [JsonIgnore]
    public virtual Ruta? IdRutaNavigation { get; set; }
}