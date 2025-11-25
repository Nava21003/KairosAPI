using KairosAPI.Data;
using KairosAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KairosAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SociosAfiliadosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SociosAfiliadosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/SociosAfiliados
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SociosAfiliado>>> GetSociosAfiliados()
        {
            // Usamos proyección (.Select) para evitar ciclos infinitos con las Promociones
            var socios = await _context.SociosAfiliados
                .Select(s => new SociosAfiliado
                {
                    IdSocio = s.IdSocio,
                    NombreSocio = s.NombreSocio,
                    TarifaCpc = s.TarifaCpc,

                    // --- CORRECCIÓN IMPORTANTE: Mapear el estatus ---
                    Estatus = s.Estatus,
                    // -----------------------------------------------

                    // Opcional: Si quieres ver las promociones de este socio
                    Promociones = s.Promociones.Select(p => new Promocione
                    {
                        IdPromocion = p.IdPromocion,
                        Titulo = p.Titulo,
                        Descripcion = p.Descripcion,
                        FechaInicio = p.FechaInicio,
                        FechaFin = p.FechaFin,
                        Estatus = p.Estatus
                        // IMPORTANTE: NO incluimos 'IdSocioNavigation' aquí para romper el ciclo
                    }).ToList()
                })
                .ToListAsync();

            return Ok(socios);
        }

        // GET: api/SociosAfiliados/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SociosAfiliado>> GetSociosAfiliado(int id)
        {
            var sociosAfiliado = await _context.SociosAfiliados
                .Where(s => s.IdSocio == id)
                .Select(s => new SociosAfiliado
                {
                    IdSocio = s.IdSocio,
                    NombreSocio = s.NombreSocio,
                    TarifaCpc = s.TarifaCpc,

                    // --- CORRECCIÓN IMPORTANTE: Mapear el estatus ---
                    Estatus = s.Estatus,
                    // -----------------------------------------------

                    Promociones = s.Promociones.Select(p => new Promocione
                    {
                        IdPromocion = p.IdPromocion,
                        Titulo = p.Titulo,
                        Descripcion = p.Descripcion,
                        FechaInicio = p.FechaInicio,
                        FechaFin = p.FechaFin,
                        Estatus = p.Estatus
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (sociosAfiliado == null)
            {
                return NotFound();
            }

            return sociosAfiliado;
        }

        // PUT: api/SociosAfiliados/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSociosAfiliado(int id, SociosAfiliado sociosAfiliado)
        {
            if (id != sociosAfiliado.IdSocio)
            {
                return BadRequest("El ID de la URL no coincide con el del cuerpo de la petición.");
            }

            _context.Entry(sociosAfiliado).State = EntityState.Modified;

            // Evitamos que intente modificar las promociones en esta petición si vienen nulas o vacías
            _context.Entry(sociosAfiliado).Collection(s => s.Promociones).IsModified = false;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SociosAfiliadoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/SociosAfiliados
        [HttpPost]
        public async Task<ActionResult<SociosAfiliado>> PostSociosAfiliado(SociosAfiliado sociosAfiliado)
        {
            // Limpiamos validaciones de la lista de promociones para evitar errores de validación
            ModelState.Remove(nameof(sociosAfiliado.Promociones));

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.SociosAfiliados.Add(sociosAfiliado);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSociosAfiliado", new { id = sociosAfiliado.IdSocio }, sociosAfiliado);
        }

        // DELETE: api/SociosAfiliados/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSociosAfiliado(int id)
        {
            var sociosAfiliado = await _context.SociosAfiliados.FindAsync(id);
            if (sociosAfiliado == null)
            {
                return NotFound();
            }

            _context.SociosAfiliados.Remove(sociosAfiliado);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SociosAfiliadoExists(int id)
        {
            return _context.SociosAfiliados.Any(e => e.IdSocio == id);
        }
    }
}