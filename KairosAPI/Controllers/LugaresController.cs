using KairosAPI.Data;
using KairosAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient; 
using System.Data; 

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
                    PuntosOtorgados = l.PuntosOtorgados,

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
            ModelState.Remove(nameof(lugar.IdCategoriaNavigation));
            ModelState.Remove(nameof(lugar.Actividades));
            ModelState.Remove(nameof(lugar.Promociones));
            ModelState.Remove(nameof(lugar.RutasLugares));
            ModelState.Remove(nameof(lugar.PuntosInteres));

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
            _context.Lugares.Remove(lugar);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        //  RECLAMAR PUNTOS
        [HttpPost("reclamar-puntos")]
        public async Task<IActionResult> ReclamarPuntos([FromBody] ReclamarPuntosRequest request)
        {
            if (request == null || request.IdUsuario <= 0 || request.IdLugar <= 0)
            {
                return BadRequest("Datos inválidos.");
            }

            try
            {
                var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "sp_ReclamarPuntosLugar";
                    command.CommandType = CommandType.StoredProcedure;

                    var pUsuario = command.CreateParameter();
                    pUsuario.ParameterName = "@idUsuario";
                    pUsuario.Value = request.IdUsuario;
                    command.Parameters.Add(pUsuario);

                    var pLugar = command.CreateParameter();
                    pLugar.ParameterName = "@idLugar";
                    pLugar.Value = request.IdLugar;
                    command.Parameters.Add(pLugar);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            var resultado = new
                            {
                                Exito = reader.GetInt32(reader.GetOrdinal("Resultado")) == 1,
                                Mensaje = reader.GetString(reader.GetOrdinal("Mensaje")),
                                PuntosGanados = reader.IsDBNull(reader.GetOrdinal("PuntosGanados")) ? 0 : reader.GetInt32(reader.GetOrdinal("PuntosGanados"))
                            };

                            if (resultado.Exito)
                                return Ok(resultado); 
                            else
                                return BadRequest(resultado); 
                        }
                    }
                }
                return BadRequest("No se obtuvo respuesta del servidor.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
            finally
            {
                if (_context.Database.GetDbConnection().State == ConnectionState.Open)
                    _context.Database.CloseConnection();
            }
        }

        private bool LugarExists(int id)
        {
            return _context.Lugares.Any(e => e.IdLugar == id);
        }
    }

    // CLASE DTO PARA RECIBIR LA PETICIÓN DESDE EL FRONTEND
    public class ReclamarPuntosRequest
    {
        public int IdUsuario { get; set; }
        public int IdLugar { get; set; }
    }
}