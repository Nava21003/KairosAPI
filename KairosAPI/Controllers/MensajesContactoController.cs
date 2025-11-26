using KairosAPI.Data;
using KairosAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KairosAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MensajesContactoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MensajesContactoController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/MensajesContacto
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MensajesContacto>>> GetMensajes()
        {
            return await _context.MensajesContactos
                .OrderByDescending(m => m.FechaEnvio)
                .ToListAsync();
        }

        // GET: api/MensajesContacto/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MensajesContacto>> GetMensaje(int id)
        {
            var mensaje = await _context.MensajesContactos.FindAsync(id);

            if (mensaje == null) return NotFound();

            return mensaje;
        }

        // POST: api/MensajesContacto
        [HttpPost]
        public async Task<ActionResult<MensajesContacto>> PostMensaje(MensajesContacto mensaje)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (mensaje.FechaEnvio == null) mensaje.FechaEnvio = DateTime.Now;
            if (string.IsNullOrEmpty(mensaje.Estatus)) mensaje.Estatus = "Pendiente";

            _context.MensajesContactos.Add(mensaje);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMensaje), new { id = mensaje.IdMensaje }, mensaje);
        }

        // PUT: api/MensajesContacto/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMensaje(int id, MensajesContacto mensaje)
        {
            if (id != mensaje.IdMensaje) return BadRequest();

            _context.Entry(mensaje).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MensajeExists(id)) return NotFound();
                else throw;
            }

            return NoContent();
        }

        // DELETE: api/MensajesContacto/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMensaje(int id)
        {
            var mensaje = await _context.MensajesContactos.FindAsync(id);
            if (mensaje == null) return NotFound();

            _context.MensajesContactos.Remove(mensaje);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MensajeExists(int id)
        {
            return _context.MensajesContactos.Any(e => e.IdMensaje == id);
        }
    }
}