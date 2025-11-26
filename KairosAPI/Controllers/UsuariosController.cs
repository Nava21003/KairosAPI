using KairosAPI.Data;
using KairosAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KairosAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsuariosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Usuarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            var usuarios = await _context.Usuarios
                .Select(u => new Usuario
                {
                    IdUsuario = u.IdUsuario,
                    IdRol = u.IdRol,
                    Nombre = u.Nombre,
                    Apellido = u.Apellido,
                    Correo = u.Correo,
                    Contrasena = u.Contrasena,
                    FechaRegistro = u.FechaRegistro,
                    FotoPerfil = u.FotoPerfil,
                    Estatus = u.Estatus,

                    IdRolNavigation = u.IdRolNavigation == null ? null : new Role
                    {
                        IdRol = u.IdRolNavigation.IdRol,
                        NombreRol = u.IdRolNavigation.NombreRol
                    }
                })
                .ToListAsync();

            return Ok(usuarios);
        }

        // GET: api/Usuarios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios
                .Where(u => u.IdUsuario == id)
                .Select(u => new Usuario
                {
                    IdUsuario = u.IdUsuario,
                    IdRol = u.IdRol,
                    Nombre = u.Nombre,
                    Apellido = u.Apellido,
                    Correo = u.Correo,
                    Contrasena = u.Contrasena,
                    FechaRegistro = u.FechaRegistro,
                    FotoPerfil = u.FotoPerfil,
                    Estatus = u.Estatus,

                    IdRolNavigation = u.IdRolNavigation == null ? null : new Role
                    {
                        IdRol = u.IdRolNavigation.IdRol,
                        NombreRol = u.IdRolNavigation.NombreRol
                    }
                })
                .FirstOrDefaultAsync();

            if (usuario == null) return NotFound();
            return usuario;
        }

        // POST: api/Usuarios
        [HttpPost]
        public async Task<ActionResult<Usuario>> PostUsuario(Usuario usuario)
        {
            if (string.IsNullOrEmpty(usuario.Contrasena))
            {
                return BadRequest("La contraseña es obligatoria para nuevos usuarios.");
            }

            if (usuario.FechaRegistro == null)
            {
                usuario.FechaRegistro = DateTime.Now;
            }

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUsuario), new { id = usuario.IdUsuario }, usuario);
        }

        // PUT: api/Usuarios/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(int id, [FromBody] PerfilUpdateRequest request)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound(new { message = "Usuario no encontrado" });

            // Actualizar solo los campos que vienen en el request
            if (!string.IsNullOrEmpty(request.Nombre))
                usuario.Nombre = request.Nombre;

            if (!string.IsNullOrEmpty(request.Apellido))
                usuario.Apellido = request.Apellido;

            if (!string.IsNullOrEmpty(request.Correo))
                usuario.Correo = request.Correo;

            if (!string.IsNullOrEmpty(request.FotoPerfil))
                usuario.FotoPerfil = request.FotoPerfil;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Perfil actualizado correctamente",
                user = new
                {
                    idUsuario = usuario.IdUsuario,
                    nombre = usuario.Nombre,
                    apellido = usuario.Apellido,
                    correo = usuario.Correo,
                    fotoPerfil = usuario.FotoPerfil
                }
            });
        }

        // PATCH: api/Usuarios/5/estatus
        [HttpPatch("{id}/estatus")]
        public async Task<IActionResult> CambiarEstatus(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();

            usuario.Estatus = !usuario.Estatus;
            await _context.SaveChangesAsync();

            return Ok(usuario);
        }

        // DELETE: api/Usuarios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.IdUsuario == id);
        }
    }
}