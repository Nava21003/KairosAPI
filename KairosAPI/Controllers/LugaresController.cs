using KairosAPI.Data;
using KairosAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KairosAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LugaresController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LugaresController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Lugares
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Lugare>>> GetLugares()
        {
            // CORRECCIÓN: Quitamos el Where(Estatus == true) para que el admin vea todo.
            // Usamos Select para un mapeo limpio.
            var lugares = await _context.Lugares
                .Include(l => l.IdCategoriaNavigation)
                .Select(l => new Lugare
                {
                    IdLugar = l.IdLugar,
                    Nombre = l.Nombre,
                    Descripcion = l.Descripcion,
                    IdCategoria = l.IdCategoria,
                    Latitud = l.Latitud,
                    Longitud = l.Longitud,
                    Direccion = l.Direccion,
                    Horario = l.Horario,
                    Imagen = l.Imagen,
                    EsPatrocinado = l.EsPatrocinado,
                    Estatus = l.Estatus,
                    // Mapeamos solo el nombre de la categoría para mostrarlo en la tabla
                    IdCategoriaNavigation = l.IdCategoriaNavigation == null ? null : new Categoria
                    {
                        IdCategoria = l.IdCategoriaNavigation.IdCategoria,
                        Nombre = l.IdCategoriaNavigation.Nombre
                    }
                })
                .ToListAsync();

            return Ok(lugares);
        }

        // GET: api/Lugares/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Lugare>> GetLugar(int id)
        {
            var lugar = await _context.Lugares
                .Include(l => l.IdCategoriaNavigation)
                .FirstOrDefaultAsync(l => l.IdLugar == id);

            if (lugar == null) return NotFound();

            return lugar;
        }

        // POST: api/Lugares
        [HttpPost]
        public async Task<ActionResult<Lugare>> PostLugar(Lugare lugar)
        {
            // Limpiamos la navegación para evitar validaciones erróneas
            ModelState.Remove(nameof(lugar.IdCategoriaNavigation));
            ModelState.Remove(nameof(lugar.Actividades));
            ModelState.Remove(nameof(lugar.Promociones));
            ModelState.Remove(nameof(lugar.RutasLugares));

            if (!ModelState.IsValid) return BadRequest(ModelState);

            _context.Lugares.Add(lugar);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLugar), new { id = lugar.IdLugar }, lugar);
        }

        // PUT: api/Lugares/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLugar(int id, Lugare lugar)
        {
            if (id != lugar.IdLugar) return BadRequest();

            _context.Entry(lugar).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LugarExists(id)) return NotFound();
                else throw;
            }

            return NoContent();
        }

        // DELETE: api/Lugares/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLugar(int id)
        {
            var lugar = await _context.Lugares.FindAsync(id);
            if (lugar == null) return NotFound();

            // Eliminación física o lógica según prefieras. 
            // Aquí usamos eliminación física para limpiar BD, 
            // pero si prefieres lógica usa: lugar.Estatus = false; y SaveChanges.
            _context.Lugares.Remove(lugar);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LugarExists(int id)
        {
            return _context.Lugares.Any(e => e.IdLugar == id);
        }
    }
}