using KairosAPI.Data;
using KairosAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KairosAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PuntosInteresController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PuntosInteresController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/PuntosInteres
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PuntoInteres>>> GetPuntosInteres()
        {
            var puntos = await _context.PuntosInteres
                .Include(p => p.IdLugarNavigation) // Incluimos el Lugar para ver su nombre
                .Select(p => new PuntoInteres
                {
                    IdPunto = p.IdPunto,
                    IdLugar = p.IdLugar,
                    Etiqueta = p.Etiqueta,
                    Descripcion = p.Descripcion,
                    Prioridad = p.Prioridad,
                    Estatus = p.Estatus,
                    // Mapeamos solo lo necesario del Lugar para evitar ciclos
                    IdLugarNavigation = p.IdLugarNavigation == null ? null : new Lugare
                    {
                        IdLugar = p.IdLugarNavigation.IdLugar,
                        Nombre = p.IdLugarNavigation.Nombre,
                        Imagen = p.IdLugarNavigation.Imagen,
                        Direccion = p.IdLugarNavigation.Direccion
                    }
                })
                .OrderByDescending(p => p.Prioridad) // Ordenamos por prioridad
                .ToListAsync();

            return Ok(puntos);
        }

        // GET: api/PuntosInteres/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PuntoInteres>> GetPuntoInteres(int id)
        {
            var punto = await _context.PuntosInteres
                .Include(p => p.IdLugarNavigation)
                .FirstOrDefaultAsync(p => p.IdPunto == id);

            if (punto == null) return NotFound();

            return punto;
        }

        // POST: api/PuntosInteres
        [HttpPost]
        public async Task<ActionResult<PuntoInteres>> PostPuntoInteres(PuntoInteres punto)
        {
            ModelState.Remove(nameof(punto.IdLugarNavigation));

            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Validar que el lugar exista
            if (!_context.Lugares.Any(l => l.IdLugar == punto.IdLugar))
                return BadRequest("El lugar especificado no existe.");

            _context.PuntosInteres.Add(punto);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPuntoInteres), new { id = punto.IdPunto }, punto);
        }

        // PUT: api/PuntosInteres/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPuntoInteres(int id, PuntoInteres punto)
        {
            if (id != punto.IdPunto) return BadRequest();

            _context.Entry(punto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PuntoInteresExists(id)) return NotFound();
                else throw;
            }

            return NoContent();
        }

        // DELETE: api/PuntosInteres/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePuntoInteres(int id)
        {
            var punto = await _context.PuntosInteres.FindAsync(id);
            if (punto == null) return NotFound();

            _context.PuntosInteres.Remove(punto);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PuntoInteresExists(int id)
        {
            return _context.PuntosInteres.Any(e => e.IdPunto == id);
        }
    }
}