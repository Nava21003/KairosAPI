using KairosAPI.Data;
using KairosAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KairosAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResenasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ResenasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Resenas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Resenas>>> GetResenas()
        {
            return await _context.Resenas
                .Where(r => r.Estatus == true) 
                .OrderByDescending(r => r.FechaRegistro) 
                .ToListAsync();
        }

        // GET: api/Resenas/Admin
        [HttpGet("Admin")]
        public async Task<ActionResult<IEnumerable<Resenas>>> GetAllResenasAdmin()
        {
            return await _context.Resenas
                .OrderByDescending(r => r.FechaRegistro)
                .ToListAsync();
        }

        // GET: api/Resenas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Resenas>> GetResena(int id)
        {
            var resena = await _context.Resenas.FindAsync(id);

            if (resena == null) return NotFound();

            return resena;
        }

        // POST: api/Resenas
        [HttpPost]
        public async Task<ActionResult<Resenas>> PostResena(Resenas resena)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (resena.FechaRegistro == null) resena.FechaRegistro = DateTime.Now;
            if (resena.Estatus == null) resena.Estatus = true;

            _context.Resenas.Add(resena);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetResena), new { id = resena.IdResena }, resena);
        }

        // PUT: api/Resenas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutResena(int id, Resenas resena)
        {
            if (id != resena.IdResena) return BadRequest();

            _context.Entry(resena).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ResenaExists(id)) return NotFound();
                else throw;
            }

            return NoContent();
        }

        // DELETE: api/Resenas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteResena(int id)
        {
            var resena = await _context.Resenas.FindAsync(id);
            if (resena == null) return NotFound();

            _context.Resenas.Remove(resena);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ResenaExists(int id)
        {
            return _context.Resenas.Any(e => e.IdResena == id);
        }
    }
}