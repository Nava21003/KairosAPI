using System;
using System.Collections.Generic;

namespace KairosAPI.Models
{
    public partial class Usuario
    {
        public int IdUsuario { get; set; }
        public int IdRol { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Apellido { get; set; }
        public string Correo { get; set; } = null!;
        public string Contrasena { get; set; } = null!;
        public DateTime? FechaRegistro { get; set; }
        public string? FotoPerfil { get; set; }
        public bool Estatus { get; set; }

        // 🔗 Relaciones
        public virtual Role? IdRolNavigation { get; set; }
        public virtual ICollection<Actividades> Actividades { get; set; } = new List<Actividades>();
        public virtual ICollection<Notificacione> Notificaciones { get; set; } = new List<Notificacione>();
        public virtual ICollection<Ruta> Ruta { get; set; } = new List<Ruta>();
        public virtual ICollection<Token> Tokens { get; set; } = new List<Token>();
        public virtual ICollection<UsoDigital> UsoDigitals { get; set; } = new List<UsoDigital>();
        public virtual ICollection<RegistroClic> RegistroClics { get; set; } = new List<RegistroClic>();

        // 🔗 Relación muchos a muchos con Intereses
        public virtual ICollection<Interese> IdInteres { get; set; } = new List<Interese>();
    }
}
