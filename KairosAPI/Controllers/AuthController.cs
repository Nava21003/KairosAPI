using KairosAPI.Data;
using KairosAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using BCrypt.Net;

namespace KairosAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // --------------------------
        // LOGIN
        // --------------------------
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _context.Usuarios
                .Include(u => u.IdRolNavigation)
                .FirstOrDefaultAsync(u => u.Correo == request.Correo);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Contrasena, user.Contrasena))
                return Unauthorized(new { success = false, message = "Credenciales inválidas" });

            var token = GenerateJwtToken(user);
            return Ok(new
            {
                success = true,
                message = "Inicio de sesión exitoso",
                token,
                user = new
                {
                    idUsuario = user.IdUsuario,  // ← CORREGIDO: Android espera minúscula
                    nombre = user.Nombre,
                    apellido = user.Apellido,
                    correo = user.Correo,
                    fotoPerfil = user.FotoPerfil,
                    rol = user.IdRolNavigation?.NombreRol
                }
            });
        }

        [HttpPost("register")]
        [IgnoreAntiforgeryToken]
        [Consumes("application/json")]
        public async Task<IActionResult> Register([FromBody] JsonElement rawUser)
        {
            if (rawUser.ValueKind == JsonValueKind.Undefined || rawUser.ValueKind == JsonValueKind.Null)
                return BadRequest(new { success = false, message = "El cuerpo de la solicitud está vacío." });

            string nombre = rawUser.GetProperty("nombre").GetString() ?? "";
            string apellido = rawUser.TryGetProperty("apellido", out var ap) ? ap.GetString() ?? "" : "";
            string correo = rawUser.GetProperty("correo").GetString() ?? "";
            string contrasena = rawUser.GetProperty("contrasena").GetString() ?? "";
            string fotoPerfil = rawUser.TryGetProperty("fotoPerfil", out var fp) ? fp.GetString() ?? "" : "";
            int idRol = rawUser.TryGetProperty("idRol", out var rolElement) ? rolElement.GetInt32() : 2;

            if (string.IsNullOrEmpty(correo) || string.IsNullOrEmpty(contrasena))
                return BadRequest(new { success = false, message = "Correo y contraseña son obligatorios." });

            if (await _context.Usuarios.AnyAsync(u => u.Correo == correo))
                return Conflict(new { success = false, message = "El correo ya está registrado" });

            var nuevo = new Usuario
            {
                Nombre = nombre,
                Apellido = apellido,
                Correo = correo,
                Contrasena = BCrypt.Net.BCrypt.HashPassword(contrasena),
                FotoPerfil = fotoPerfil,
                IdRol = idRol,

                FechaRegistro = DateTime.Now,
                Estatus = true
            };

            _context.Usuarios.Add(nuevo);
            await _context.SaveChangesAsync();

            var token = GenerateJwtToken(nuevo);

            return Ok(new
            {
                success = true,
                message = "Usuario registrado correctamente",
                token,
                user = new
                {
                    id = nuevo.IdUsuario,
                    nombre = nuevo.Nombre,
                    apellido = nuevo.Apellido,
                    correo = nuevo.Correo,
                    fotoPerfil = nuevo.FotoPerfil,
                    idRol = nuevo.IdRol
                }
            });
        }

        // --------------------------
        // TOKEN GENERATOR
        // --------------------------
        private string GenerateJwtToken(Usuario user)
        {
            var jwtSettings = _config.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.IdUsuario.ToString()),
                new Claim("correo", user.Correo),
                new Claim("rol", user.IdRolNavigation?.NombreRol ?? "Usuario")
            };

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(4),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class LoginRequest
    {
        public string? Correo { get; set; }
        public string? Contrasena { get; set; }
    }
}
