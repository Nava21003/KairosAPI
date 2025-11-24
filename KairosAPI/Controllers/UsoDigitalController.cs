using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KairosAPI.Data;
using KairosAPI.Models;

namespace KairosAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsoDigitalController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsoDigitalController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/UsoDigital
        [HttpPost]
        public async Task<ActionResult<UsoDigital>> PostUsoDigital(UsoDigitalRequest request)
        {
            // 1. Parseamos la fecha primero
            DateOnly fechaRegistro = DateOnly.Parse(request.Fecha);

            // 2. Buscamos si YA existe un registro para este usuario en esta fecha
            var registroExistente = await _context.UsoDigitals
                .FirstOrDefaultAsync(u => u.IdUsuario == request.IdUsuario && u.FechaRegistro == fechaRegistro);

            if (registroExistente != null)
            {
                // --- CASO A: YA EXISTE -> ACTUALIZAMOS (UPDATE) ---
                // Actualizamos los minutos con el nuevo valor que manda la app
                registroExistente.TiempoDigitalMinutos = request.TiempoMinutos;

                // EF Core detecta el cambio automáticamente, no hace falta .Update() explícito a veces,
                // pero guardar cambios basta.
            }
            else
            {
                // --- CASO B: NO EXISTE -> INSERTAMOS (INSERT) ---
                var nuevoRegistro = new UsoDigital
                {
                    IdUsuario = request.IdUsuario,
                    TiempoDigitalMinutos = request.TiempoMinutos,
                    FechaRegistro = fechaRegistro
                };
                _context.UsoDigitals.Add(nuevoRegistro);
            }

            // 3. Guardamos los cambios (sea Update o Insert)
            await _context.SaveChangesAsync();

            return Ok();
        }
    }

    public class UsoDigitalRequest
    {
        public int IdUsuario { get; set; }
        public int TiempoMinutos { get; set; }
        public string Fecha { get; set; }
    }
}