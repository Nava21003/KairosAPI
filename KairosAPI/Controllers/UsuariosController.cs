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
            // CORRECCIÓN: Usamos .Select() para proyectar los datos.
            // Esto crea una copia limpia de los datos sin metadatos "$id" ni ciclos.
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

                    // Aquí "limpiamos" el Rol.
                    // Creamos un nuevo objeto Role SOLO con el ID y Nombre.
                    // Al no incluir la propiedad 'Usuarios' del rol, rompemos el ciclo infinito.
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

                    // Misma limpieza para el usuario individual
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

        [HttpPost]
        public async Task<ActionResult<Usuario>> PostUsuario(Usuario usuario)
        {
            // Si la fecha no viene, la asignamos
            if (usuario.FechaRegistro == null)
            {
                usuario.FechaRegistro = DateTime.Now;
            }

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            // Usamos CreatedAtAction para retornar el objeto creado
            return CreatedAtAction(nameof(GetUsuario), new { id = usuario.IdUsuario }, usuario);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(int id, Usuario usuario)
        {
            if (id != usuario.IdUsuario) return BadRequest();

            _context.Entry(usuario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(id)) return NotFound();
                else throw;
            }

            return NoContent();
        }

        [HttpPatch("{id}/estatus")]
        public async Task<IActionResult> CambiarEstatus(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();

            usuario.Estatus = !usuario.Estatus;
            await _context.SaveChangesAsync();

            // Retornamos el usuario actualizado para que el frontend pueda actualizar el estado localmente si es necesario
            return Ok(usuario);
        }

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