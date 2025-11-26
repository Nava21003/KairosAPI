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

        // 🧠 LÓGICA DE IA: Genera mensajes personalizados y conversacionales
        private string GenerarMensajeInteligente(int pasosHoy, int tiempoMinutos, double pasosPromedio, double tiempoPromedio)
        {
            // CASO 1: Excelente balance (buenos pasos, bajo tiempo digital)
            if (pasosHoy >= 8000 && tiempoMinutos <= 60)
            {
                var frases = new[] {
                    $"🌟 ¡WOW! {pasosHoy} pasos y solo {tiempoMinutos}min en pantalla. Estás dominando tu día como un campeón. ¿Te atreves a explorar un lugar nuevo mañana?",
                    $"🔥 ¡Eres imparable! Con {pasosHoy} pasos hoy, estás en la cima de tu juego. Tu yo del futuro te agradecerá este esfuerzo.",
                    $"💎 ¡Perfección! {pasosHoy} pasos + control digital = fórmula ganadora. Así se construye una vida épica, paso a paso."
                };
                return frases[new Random().Next(frases.Length)];
            }

            // CASO 2: Buenos pasos pero mucho tiempo digital
            if (pasosHoy >= 5000 && tiempoMinutos > 120)
            {
                return $"💪 ¡Genial con los {pasosHoy} pasos! Pero... {tiempoMinutos}min en pantalla es mucho. ¿Y si apagas el celular 1 hora antes de dormir? Tu sueño (y tu cerebro) lo amarán.";
            }

            // CASO 3: Pocos pasos y mucho tiempo digital (alerta crítica)
            if (pasosHoy < 2000 && tiempoMinutos > 180)
            {
                return $"⚠️ Ey, solo {pasosHoy} pasos pero {tiempoMinutos}min de pantalla. Tu cuerpo está pidiendo movimiento. ¿Caminata de 20 minutos? Prometo que después te sentirás increíble.";
            }

            // CASO 4: Mejorando respecto al promedio (motivación específica)
            if (pasosHoy > pasosPromedio * 1.2)
            {
                return $"📈 ¡IMPRESIONANTE! Estás {(int)((pasosHoy / pasosPromedio - 1) * 100)}% arriba de tu promedio ({(int)pasosPromedio} pasos). Sigue así y esta semana será legendaria. 🚀";
            }

            // CASO 5: Reduciendo tiempo digital exitosamente
            if (tiempoMinutos < tiempoPromedio * 0.8 && tiempoPromedio > 0)
            {
                int reduccion = (int)(tiempoPromedio - tiempoMinutos);
                return $"🎯 ¡Bravo! Has reducido {reduccion}min de pantalla hoy. Eso es más tiempo para vivir el mundo real. ¿Qué harás con esos minutos extra? 😊";
            }

            // CASO 6: Día promedio pero con potencial
            if (pasosHoy >= 3000 && pasosHoy < 8000)
            {
                int faltantes = 8000 - pasosHoy;
                return $"👍 Llevas {pasosHoy} pasos sólidos. Te faltan {faltantes} para alcanzar los 8K. Una caminata de 15min al atardecer y lo logras. ¿Te animas?";
            }

            // CASO 7: Inicio del día (motivación proactiva)
            if (pasosHoy < 500 && tiempoMinutos < 30)
            {
                var horaActual = DateTime.Now.Hour;
                if (horaActual < 12)
                    return "☀️ Buenos días, explorador. Hoy es un lienzo en blanco. ¿Qué tal empezar con una caminata matutina? El aire fresco despierta la creatividad.";
                else if (horaActual < 18)
                    return "🌤️ Buenas tardes. Aún tienes tiempo para hacer de hoy un gran día. Una caminata de 20 minutos puede cambiar tu energía por completo.";
                else
                    return "🌙 Buenas noches. Aunque es tarde, nunca está de más una caminata nocturna de 10min. Relaja la mente y prepara el sueño.";
            }

            // CASO 8: Pantalla moderada pero pocos pasos
            if (pasosHoy < 3000 && tiempoMinutos >= 60 && tiempoMinutos <= 120)
            {
                return $"🤔 Solo {pasosHoy} pasos hoy. Tu cuerpo está diseñado para moverse, no para estar quieto. ¿Qué tal visitar ese café que tenías pendiente? Caminando, claro.";
            }

            // CASO 9: Sedentarismo detectado (crítico)
            if (pasosHoy < 1000 && tiempoMinutos > 60)
            {
                return $"🚨 Solo {pasosHoy} pasos en {tiempoMinutos}min de pantalla. Tu cuerpo necesita movimiento urgente. Aunque sea 5min de caminar, ¡hazlo ya!";
            }

            // CASO 10: Usuario constante (motivación de mantenimiento)
            if (Math.Abs(pasosHoy - pasosPromedio) < 500 && pasosPromedio > 3000)
            {
                return $"⚡ Eres super constante con tus {pasosHoy} pasos diarios. La consistencia es la clave del éxito. ¿Qué tal subir el nivel a 6K esta semana?";
            }

            // CASO DEFAULT: Motivación general conversacional
            var frasesFallback = new[] {
                "💙 Cada paso que das es una victoria. Cada minuto sin pantalla es libertad. Sigue adelante, explorador.",
                "🌍 El mundo está lleno de lugares increíbles esperándote. ¿Cuál será tu próxima aventura?",
                "✨ Tu bienestar no es un destino, es un viaje. Y hoy ya diste el primer paso al abrir esta app."
            };
            return frasesFallback[new Random().Next(frasesFallback.Length)];
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
