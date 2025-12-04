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
                    // Mapeo del nuevo campo
                    PuntosAcumulados = u.PuntosAcumulados,
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
                    // Mapeo del nuevo campo
                    PuntosAcumulados = u.PuntosAcumulados,
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

            // Si no se envían puntos, inicializar en 0 (opcional, ya lo hace la BD por default)
            if (usuario.PuntosAcumulados == null)
            {
                usuario.PuntosAcumulados = 0;
            }

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUsuario), new { id = usuario.IdUsuario }, usuario);
        }

        // PUT: api/Usuarios/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(int id, Usuario usuarioEntrante)
        {
            if (id != usuarioEntrante.IdUsuario)
            {
                return BadRequest("El ID del usuario no coincide.");
            }

            var usuarioExistente = await _context.Usuarios.FindAsync(id);

            if (usuarioExistente == null)
            {
                return NotFound($"No se encontró el usuario con ID {id}");
            }

            usuarioExistente.Nombre = usuarioEntrante.Nombre;
            usuarioExistente.Apellido = usuarioEntrante.Apellido;
            usuarioExistente.Correo = usuarioEntrante.Correo;
            usuarioExistente.IdRol = usuarioEntrante.IdRol;
            usuarioExistente.Estatus = usuarioEntrante.Estatus;
            usuarioExistente.FotoPerfil = usuarioEntrante.FotoPerfil;

            // Actualización del nuevo campo
            usuarioExistente.PuntosAcumulados = usuarioEntrante.PuntosAcumulados;

            if (!string.IsNullOrEmpty(usuarioEntrante.Contrasena))
            {
                usuarioExistente.Contrasena = usuarioEntrante.Contrasena;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(usuarioExistente);
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