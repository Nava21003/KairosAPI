using KairosAPI.Data;
using KairosAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KairosAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RolesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Role>>> GetRoles()
        {
            return await _context.Roles.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Role>> GetRol(int id)
        {
            var rol = await _context.Roles.FindAsync(id);
            if (rol == null) return NotFound();
            return rol;
        }

        [HttpPost]
        public async Task<ActionResult<Role>> PostRol(Role rol)
        {
            _context.Roles.Add(rol);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetRol), new { id = rol.IdRol }, rol);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutRol(int id, Role rol)
        {
            if (id != rol.IdRol) return BadRequest();
            _context.Entry(rol).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRol(int id)
        {
            var rol = await _context.Roles.FindAsync(id);
            if (rol == null)
            {
                return NotFound();
            }

            _context.Roles.Remove(rol);

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException ex) when (ex.InnerException is Microsoft.Data.SqlClient.SqlException sqlEx && sqlEx.Number == 547)
            {
                return Conflict("No se puede eliminar el rol porque está siendo utilizado por uno o más usuarios. Por favor, reasigne o elimine los usuarios asociados primero.");
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, "Ocurrió un error inesperado al intentar guardar los cambios.");
            }
        }

    }
}
