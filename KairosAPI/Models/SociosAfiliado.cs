using KairosAPI.Models;

public partial class SociosAfiliado
{
    public int IdSocio { get; set; }
    public string NombreSocio { get; set; } = null!;
    public decimal? TarifaCpc { get; set; }

    public bool? Estatus { get; set; }

    public virtual ICollection<Promocione> Promociones { get; set; } = new List<Promocione>();
}