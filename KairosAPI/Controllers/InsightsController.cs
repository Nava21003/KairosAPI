using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KairosAPI.Data;
using System.Linq;

namespace KairosAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InsightsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public InsightsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Insights/{userId}
        [HttpGet("{userId}")]
        public async Task<ActionResult<InsightResponse>> GetInsight(int userId)
        {
            try
            {
                var hoy = DateOnly.FromDateTime(DateTime.Today);

                // 1. Obtener datos de ACTIVIDAD FÍSICA (pasos de hoy)
                var actividadHoy = await _context.Actividades
                    .Where(a => a.IdUsuario == userId
                               && a.FechaInicio.HasValue
                               && DateOnly.FromDateTime(a.FechaInicio.Value) == hoy)
                    .OrderByDescending(a => a.FechaInicio)
                    .FirstOrDefaultAsync();

                int pasosHoy = actividadHoy?.Pasos ?? 0;

                // 2. Obtener datos de USO DIGITAL (tiempo de hoy)
                var usoHoy = await _context.UsoDigitals
                    .Where(u => u.IdUsuario == userId && u.FechaRegistro == hoy)
                    .FirstOrDefaultAsync();

                int tiempoMinutos = usoHoy?.TiempoDigitalMinutos ?? 0;

                // 3. Calcular PROMEDIOS de los últimos 7 días (para comparar)
                var hace7Dias = hoy.AddDays(-7);

                var pasosPromedio = await _context.Actividades
                    .Where(a => a.IdUsuario == userId
                               && a.FechaInicio.HasValue
                               && DateOnly.FromDateTime(a.FechaInicio.Value) >= hace7Dias)
                    .AverageAsync(a => (int?)a.Pasos) ?? 0;

                var tiempoPromedio = await _context.UsoDigitals
                    .Where(u => u.IdUsuario == userId && u.FechaRegistro >= hace7Dias)
                    .AverageAsync(u => (int?)u.TiempoDigitalMinutos) ?? 0;

                // 4. GENERAR EL INSIGHT (LÓGICA DE IA SIMULADA)
                string mensaje = GenerarMensajeInteligente(pasosHoy, tiempoMinutos, pasosPromedio, tiempoPromedio);
                string tipo = DeterminarTipoMensaje(pasosHoy, tiempoMinutos);

                return Ok(new InsightResponse
                {
                    Mensaje = mensaje,
                    Tipo = tipo,
                    PasosHoy = pasosHoy,
                    TiempoDigitalHoy = tiempoMinutos,
                    PasosPromedio = (int)pasosPromedio,
                    TiempoDigitalPromedio = (int)tiempoPromedio
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // 🧠 LÓGICA DE IA: Genera mensajes personalizados
        private string GenerarMensajeInteligente(int pasosHoy, int tiempoMinutos, double pasosPromedio, double tiempoPromedio)
        {
            // CASO 1: Excelente balance (buenos pasos, bajo tiempo digital)
            if (pasosHoy >= 8000 && tiempoMinutos <= 60)
            {
                return "🌟 ¡Increíble! Estás en tu mejor momento. Sigue así, campeón.";
            }

            // CASO 2: Buenos pasos pero mucho tiempo digital
            if (pasosHoy >= 5000 && tiempoMinutos > 120)
            {
                return "💪 Excelente actividad física, pero reduce un poco el tiempo en pantalla. Tu mente te lo agradecerá.";
            }

            // CASO 3: Pocos pasos y mucho tiempo digital (alerta crítica)
            if (pasosHoy < 2000 && tiempoMinutos > 180)
            {
                return "⚠️ Hoy has estado mucho tiempo en el celular. ¿Qué tal una caminata de 15 minutos? Tu cuerpo lo necesita.";
            }

            // CASO 4: Mejorando respecto al promedio
            if (pasosHoy > pasosPromedio * 1.2)
            {
                return $"📈 ¡Vas mejor que tu promedio semanal! Llevas {pasosHoy} pasos hoy, tu promedio es {(int)pasosPromedio}.";
            }

            // CASO 5: Reduciendo tiempo digital exitosamente
            if (tiempoMinutos < tiempoPromedio * 0.8 && tiempoPromedio > 0)
            {
                return $"🎯 ¡Estás reduciendo tu tiempo digital! Hoy: {tiempoMinutos}min, promedio: {(int)tiempoPromedio}min.";
            }

            // CASO 6: Día promedio (motivación general)
            if (pasosHoy >= 3000 && pasosHoy < 8000)
            {
                return $"👍 Vas bien hoy con {pasosHoy} pasos. ¿Puedes llegar a 5000 antes de dormir?";
            }

            // CASO 7: Inicio del día (sin datos suficientes)
            if (pasosHoy < 500 && tiempoMinutos < 30)
            {
                return "🌅 ¡Buenos días! Sal a explorar. El mundo está ahí afuera esperándote.";
            }

            // CASO 8: Sedentarismo detectado
            if (pasosHoy < 1000)
            {
                return "🚶 Llevas pocos pasos hoy. Una caminata corta puede cambiar tu día por completo.";
            }

            // CASO DEFAULT: Motivación general
            return "💙 Recuerda: cada paso cuenta, cada minuto lejos de la pantalla es vida ganada.";
        }

        // 🎨 Determina el tipo de mensaje para la UI (success, warning, info)
        private string DeterminarTipoMensaje(int pasos, int tiempo)
        {
            if (pasos >= 8000 && tiempo <= 60) return "success";
            if (pasos < 2000 && tiempo > 180) return "warning";
            if (pasos >= 5000) return "success";
            return "info";
        }
    }

    // Modelo de respuesta
    public class InsightResponse
    {
        public string Mensaje { get; set; } = "";
        public string Tipo { get; set; } = "info"; // success, warning, info
        public int PasosHoy { get; set; }
        public int TiempoDigitalHoy { get; set; }
        public int PasosPromedio { get; set; }
        public int TiempoDigitalPromedio { get; set; }
    }
}
