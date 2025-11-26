using System.ComponentModel.DataAnnotations;

public class LoginRequest
{
    [Required]
    public string Correo { get; set; }

    [Required]
    public string Contrasena { get; set; }
}