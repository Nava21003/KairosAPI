using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KairosAPI.Data;
using KairosAPI.Models;

namespace KairosAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActividadesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ActividadesController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/Actividades
        [HttpPost]
        public async Task<ActionResult<Actividades>> PostActividadFisica(ActividadFisicaRequest request)
        {
            var nuevaActividad = new Actividades
            {
                IdUsuario = request.IdUsuario,
                Pasos = request.Pasos,
                // Asumimos que en este modelo SÍ usas DateTime (por el SQL que me mostraste)
                FechaInicio = DateTime.Parse(request.Fecha),
                FechaFin = DateTime.Parse(request.Fecha),
                IdLugar = null
            };

            _context.Actividades.Add(nuevaActividad);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }

    public class ActividadFisicaRequest
    {
        public int IdUsuario { get; set; }
        public int Pasos { get; set; }
        public string Fecha { get; set; }
    }
}
