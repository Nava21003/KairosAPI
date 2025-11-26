using KairosAPI.Data;
using KairosAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KairosAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromocionesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PromocionesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Promociones
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Promocione>>> GetPromociones()
        {
            return await _context.Promociones
                .Include(p => p.IdLugarNavigation)
                .ToListAsync();
        }

        // GET: api/Promociones/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Promocione>> GetPromocion(int id)
        {
            var promo = await _context.Promociones
                .Include(p => p.IdLugarNavigation)
                .FirstOrDefaultAsync(p => p.IdPromocion == id);

            if (promo == null) return NotFound();
            return promo;
        }

        // POST: api/Promociones
        [HttpPost]
        public async Task<ActionResult<Promocione>> PostPromocion(Promocione promocion)
        {
            _context.Promociones.Add(promocion);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetPromocion), new { id = promocion.IdPromocion }, promocion);
        }

        // PUT: api/Promociones/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPromocion(int id, Promocione promocion)
        {
            if (id != promocion.IdPromocion) return BadRequest();

            _context.Entry(promocion).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Promociones/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePromocion(int id)
        {
            var promo = await _context.Promociones.FindAsync(id);
            if (promo == null) return NotFound();
            _context.Promociones.Remove(promo);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}