using System.ComponentModel.DataAnnotations;

public class PerfilUpdateRequest
{

    [MaxLength(100)]
    public string? Nombre { get; set; }

    [MaxLength(100)]
    public string? Apellido { get; set; }

    [EmailAddress]
    [MaxLength(150)]
    public string? Correo { get; set; }

    // Sin MaxLength porque las imágenes Base64 pueden ser muy largas
    public string? FotoPerfil { get; set; }
}