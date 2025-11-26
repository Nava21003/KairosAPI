using KairosAPI.Data;
using KairosAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KairosAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InteresesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public InteresesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Intereses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Interese>>> GetIntereses()
        {
            var intereses = await _context.Intereses
                .Select(i => new Interese
                {
                    IdInteres = i.IdInteres,
                    Nombre = i.Nombre,
                    Descripcion = i.Descripcion,
                    IdUsuarios = new List<Usuario>()
                })
                .ToListAsync();

            return Ok(intereses);
        }

        // GET: api/Intereses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Interese>> GetInterese(int id)
        {
            var interese = await _context.Intereses
                .FirstOrDefaultAsync(i => i.IdInteres == id);

            if (interese == null) return NotFound();

            return interese;
        }

        // POST: api/Intereses
        [HttpPost]
        public async Task<ActionResult<Interese>> PostInterese(Interese interese)
        {
            ModelState.Remove(nameof(interese.IdUsuarios));

            if (!ModelState.IsValid) return BadRequest(ModelState);

            _context.Intereses.Add(interese);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetInterese), new { id = interese.IdInteres }, interese);
        }

        // PUT: api/Intereses/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInterese(int id, Interese interese)
        {
            if (id != interese.IdInteres) return BadRequest();

            _context.Entry(interese).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IntereseExists(id)) return NotFound();
                else throw;
            }

            return NoContent();
        }

        // DELETE: api/Intereses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInterese(int id)
        {
            var interese = await _context.Intereses.FindAsync(id);
            if (interese == null) return NotFound();

            _context.Intereses.Remove(interese);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool IntereseExists(int id)
        {
            return _context.Intereses.Any(e => e.IdInteres == id);
        }
    }
}