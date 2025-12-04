using KairosAPI.Data;
using KairosAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System; // Agregado para DateTime
using System.Collections.Generic; // Agregado para IEnumerable
using System.Linq;
using System.Threading.Tasks;

namespace KairosAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RutasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RutasController(AppDbContext context)
        {
            _context = context;
        }

        // 1. OBTENER TODAS LAS RUTAS
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Ruta>>> GetRutas() 
        {
            return await _context.Rutas
        .Include(r => r.IdUsuarioNavigation)
        .Include(r => r.IdLugarInicioNavigation)
        .Include(r => r.IdLugarFinNavigation)
        .Include(r => r.RutasLugares.OrderBy(rl => rl.Orden)) 
                    .ThenInclude(rl => rl.IdLugarNavigation)
        .ToListAsync();
        }

        // 2. OBTENER RUTA POR ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Ruta>> GetRuta(int id)
        {
            var ruta = await _context.Rutas
              .Include(r => r.IdUsuarioNavigation)
              .Include(r => r.IdLugarInicioNavigation)
              .Include(r => r.IdLugarFinNavigation)
              .Include(r => r.RutasLugares.OrderBy(rl => rl.Orden))
                .ThenInclude(rl => rl.IdLugarNavigation)
              .FirstOrDefaultAsync(r => r.IdRuta == id);

            if (ruta == null)
            {
                return NotFound();
            }

            return ruta;
        }

        // 3. CREAR NUEVA RUTA (POST)
        [HttpPost]
        public async Task<ActionResult<Ruta>> PostRuta(Ruta ruta)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                ruta.IdRuta = 0;
                ruta.FechaCreacion = DateTime.Now;


                var rutasLugares = ruta.RutasLugares?.ToList();
                ruta.RutasLugares = null; 

                _context.Rutas.Add(ruta);
                await _context.SaveChangesAsync();

                if (rutasLugares != null && rutasLugares.Count > 0)
                {
                    foreach (var parada in rutasLugares)
                    {
                        parada.IdRuta = ruta.IdRuta; 
                        _context.RutasLugares.Add(parada);
                    }
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();

                var rutaCreada = await GetRuta(ruta.IdRuta);
                return CreatedAtAction(nameof(GetRuta), new { id = rutaCreada.Value.IdRuta }, rutaCreada.Value);
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                var innerMessage = ex.InnerException?.Message ?? ex.Message;
                return BadRequest($"Error al crear la ruta. Verifique los IDs de Usuario/Lugares, o duplicidad de paradas intermedias. Detalle: {innerMessage}");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // 4. ACTUALIZAR RUTA (PUT)
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRuta(int id, Ruta ruta)
        {
            if (id != ruta.IdRuta)
            {
                return BadRequest("El ID de la ruta en la URL no coincide con el ID del cuerpo.");
            }

            var rutaExistente = await _context.Rutas
        .Include(r => r.RutasLugares)
        .FirstOrDefaultAsync(r => r.IdRuta == id);

            if (rutaExistente == null)
            {
                return NotFound();
            }

            _context.Entry(rutaExistente).CurrentValues.SetValues(ruta);
            _context.RutasLugares.RemoveRange(rutaExistente.RutasLugares);

            if (ruta.RutasLugares != null)
            {
                foreach (var parada in ruta.RutasLugares)
                {
                    parada.IdRuta = id;
                    _context.Entry(parada).State = EntityState.Added;
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RutaExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (DbUpdateException ex)
            {
                var innerMessage = ex.InnerException?.Message ?? ex.Message;
                return BadRequest($"Error al actualizar la ruta. Verifique los IDs de Lugares. Detalle: {innerMessage}");
            }

            return NoContent();
        }

        // 5. ELIMINAR RUTA (DELETE)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRuta(int id)
        {
            var ruta = await _context.Rutas
                      .Include(r => r.RutasLugares)
              .FirstOrDefaultAsync(r => r.IdRuta == id);

            if (ruta == null)
            {
                return NotFound();
            }

            _context.RutasLugares.RemoveRange(ruta.RutasLugares);

            _context.Rutas.Remove(ruta);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // 6. MÉTODO AUXILIAR
        private bool RutaExists(int id)
        {
            return _context.Rutas.Any(e => e.IdRuta == id);
        }
    }
}