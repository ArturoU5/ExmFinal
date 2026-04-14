using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Gimnasio.Data;
using Gimnasio.Models;

namespace Gimnasio.Controllers
{
    [ApiController]
    [Route("api/rutina-ejercicio")]
    public class RutinaEjercicioController : ControllerBase
    {
        private readonly AppDbContext _context;
        public RutinaEjercicioController(AppDbContext context)
        {
            _context = context;
        }

        //GET api/rutina-ejercicio/{rutinaId}
        [HttpGet("{rutinaId}")]
        [Authorize(Roles = "ADMIN,ENTRENADOR,SOCIO")]
        public async Task<IActionResult> GetEjerciciosByRutina(int rutinaId)
        {
            var rutina = await _context.Rutinas.FindAsync(rutinaId);
            if (rutina == null)
            {
                return NotFound("Rutina no encontrada.");
            }

            var ejercicios = await _context.RutinaEjercicios
                .Where(re => re.RutinaId == rutinaId)
                .OrderBy(re => re.Orden)
                .ToListAsync();

            return Ok(ejercicios);
        }

        //POST api/rutina-ejercicio
        [HttpPost]
        [Authorize(Roles = "ADMIN,ENTRENADOR")]
        public async Task<IActionResult> Post([FromBody] RutinaEjercicios rutinaEjercicio)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validar que la rutina existe
            var rutinaExists = await _context.Rutinas.AnyAsync(r => r.RutinaId == rutinaEjercicio.RutinaId);
            if (!rutinaExists)
            {
                return BadRequest(new
                {
                    mensaje = "Error de validación",
                    detalle = $"La rutina con ID {rutinaEjercicio.RutinaId} no existe."
                });
            }

            // Validar que el ejercicio existe
            var ejercicioExists = await _context.Ejercicios.AnyAsync(e => e.EjercicioId == rutinaEjercicio.EjercicioId);
            if (!ejercicioExists)
            {
                return BadRequest(new
                {
                    mensaje = "Error de validación",
                    detalle = $"El ejercicio con ID {rutinaEjercicio.EjercicioId} no existe."
                });
            }

            try
            {
                _context.RutinaEjercicios.Add(rutinaEjercicio);
                await _context.SaveChangesAsync();
                return Created("", rutinaEjercicio);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    mensaje = "Error al agregar ejercicio a la rutina",
                    detalle = ex.InnerException?.Message ?? ex.Message
                });
            }
        }

        //PUT api/rutina-ejercicio/{rutinaId}/{ejercicioId}
        [HttpPut("{rutinaId}/{ejercicioId}")]
        [Authorize(Roles = "ADMIN,ENTRENADOR")]
        public async Task<IActionResult> Put(int rutinaId, int ejercicioId, [FromBody] RutinaEjercicios rutinaEjercicio)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existing = await _context.RutinaEjercicios
                .FirstOrDefaultAsync(re => re.RutinaId == rutinaId && re.EjercicioId == ejercicioId);

            if (existing == null)
            {
                return NotFound("Ejercicio en rutina no encontrado.");
            }

            existing.Orden = rutinaEjercicio.Orden;
            existing.Series = rutinaEjercicio.Series;
            existing.Repeticiones = rutinaEjercicio.Repeticiones;
            existing.PesoObjetivoKg = rutinaEjercicio.PesoObjetivoKg;
            existing.DuracionSegundos = rutinaEjercicio.DuracionSegundos;
            existing.DescansoSegundos = rutinaEjercicio.DescansoSegundos;
            existing.Notas = rutinaEjercicio.Notas;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    mensaje = "Ejercicio en rutina actualizado correctamente",
                    rutinaEjercicio = existing
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    mensaje = "Error al actualizar ejercicio en rutina",
                    detalle = ex.InnerException?.Message ?? ex.Message
                });
            }
        }

        //DELETE api/rutina-ejercicio/{rutinaId}/{ejercicioId}
        [HttpDelete("{rutinaId}/{ejercicioId}")]
        [Authorize(Roles = "ADMIN,ENTRENADOR")]
        public async Task<IActionResult> Delete(int rutinaId, int ejercicioId)
        {
            var rutinaEjercicio = await _context.RutinaEjercicios
                .FirstOrDefaultAsync(re => re.RutinaId == rutinaId && re.EjercicioId == ejercicioId);

            if (rutinaEjercicio == null)
            {
                return NotFound("Ejercicio en rutina no encontrado.");
            }

            _context.RutinaEjercicios.Remove(rutinaEjercicio);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
