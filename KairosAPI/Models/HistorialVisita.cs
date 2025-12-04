using System;
using System.Collections.Generic;
namespace KairosAPI.Models;

public partial class HistorialVisita
{
    public int IdVisita { get; set; }

    public int IdUsuario { get; set; }

    public int IdLugar { get; set; }

    public int PuntosGanados { get; set; }

    public DateTime? FechaVisita { get; set; }

    public virtual Lugare? IdLugarNavigation { get; set; }
    public virtual Usuario? IdUsuarioNavigation { get; set; }
}