using KairosAPI.Data;
using KairosAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KairosAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PreguntasFrecuentesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PreguntasFrecuentesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/PreguntasFrecuentes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PreguntasFrecuentes>>> GetPreguntas()
        {
            return await _context.PreguntasFrecuentes
                .OrderBy(f => f.Orden)
                .ToListAsync();
        }

        // GET: api/PreguntasFrecuentes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PreguntasFrecuentes>> GetPregunta(int id)
        {
            var pregunta = await _context.PreguntasFrecuentes.FindAsync(id);

            if (pregunta == null) return NotFound();

            return pregunta;
        }

        // POST: api/PreguntasFrecuentes
        [HttpPost]
        public async Task<ActionResult<PreguntasFrecuentes>> PostPregunta(PreguntasFrecuentes pregunta)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _context.PreguntasFrecuentes.Add(pregunta);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPregunta), new { id = pregunta.IdPregunta }, pregunta);
        }

        // PUT: api/PreguntasFrecuentes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPregunta(int id, PreguntasFrecuentes pregunta)
        {
            if (id != pregunta.IdPregunta) return BadRequest();

            _context.Entry(pregunta).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PreguntaExists(id)) return NotFound();
                else throw;
            }

            return NoContent();
        }

        // DELETE: api/PreguntasFrecuentes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePregunta(int id)
        {
            var pregunta = await _context.PreguntasFrecuentes.FindAsync(id);
            if (pregunta == null) return NotFound();

            _context.PreguntasFrecuentes.Remove(pregunta);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PreguntaExists(int id)
        {
            return _context.PreguntasFrecuentes.Any(e => e.IdPregunta == id);
        }
    }
}