using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Gimnasio.Data;
using Gimnasio.Models;

namespace Gimnasio.Controllers
{

    [ApiController]
    [Route("api/asistencia")]
    public class AsistenciaController : ControllerBase
    {
        private readonly AppDbContext _context;
        public AsistenciaController(AppDbContext context)
        {
            _context = context;
        }



        //GET api/asistencia
        [HttpGet]
        [Authorize(Roles = "ADMIN,ENTRENADOR,SOCIO")]
        public async Task<IActionResult> Get()
        {
            var asistencias = await _context.Asistencias.ToListAsync();
            return Ok(asistencias);
        }



        //GET api/asistencia/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "ADMIN,ENTRENADOR,SOCIO")]
        public async Task<IActionResult> GetOnly(int id)
        {
            var asistencias = await _context.Asistencias.FindAsync(id);

            if (asistencias == null)
            {
                return NotFound();
            }

            return Ok(asistencias);
        }



        //POST api/asistencia
        [HttpPost]
        [Authorize(Roles = "ADMIN,ENTRENADOR")]
        public async Task<IActionResult> Post([FromBody] Asistencias asistencia)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validar que la fecha de salida sea posterior a la de entrada
            if (asistencia.FechaHoraSalida <= asistencia.FechaHoraEntrada)
            {
                return BadRequest(new
                {
                    mensaje = "Error de validación",
                    detalle = "La fecha y hora de salida debe ser posterior a la de entrada."
                });
            }

            // Validar que el socio existe
            var socioExists = await _context.Socios.AnyAsync(s => s.SocioId == asistencia.SocioId);
            if (!socioExists)
            {
                return BadRequest(new
                {
                    mensaje = "Error de validación",
                    detalle = $"El socio con ID {asistencia.SocioId} no existe."
                });
            }

            // Validar que el usuario existe
            var userExists = await _context.Users.AnyAsync(u => u.UserId == asistencia.RegistradaPorUserId);
            if (!userExists)
            {
                return BadRequest(new
                {
                    mensaje = "Error de validación",
                    detalle = $"El usuario con ID {asistencia.RegistradaPorUserId} no existe."
                });
            }

            try
            {
                _context.Asistencias.Add(asistencia);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetOnly), new { id = asistencia.AsistenciaId }, asistencia);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    mensaje = "Error al registrar la asistencia",
                    detalle = ex.InnerException?.Message ?? ex.Message
                });
            }
        }



        //PUT api/asistencia/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN,ENTRENADOR")]
        public async Task<IActionResult> Put(int id, [FromBody] Asistencias asistencia)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingUser = await _context.Asistencias.FindAsync(id);
            if (existingUser == null)
            {
                return NotFound("Error, registro de asistencia no encontrado.");
            }

            existingUser.SocioId = asistencia.SocioId;
            existingUser.FechaHoraEntrada = asistencia.FechaHoraEntrada;
            existingUser.FechaHoraSalida = asistencia.FechaHoraSalida;
            existingUser.Observaciones = asistencia.Observaciones;
            existingUser.RegistradaPorUserId = asistencia.RegistradaPorUserId;


            try
            {
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    mensaje = "Asistencia actualizada correctamente",
                    asistencia = existingUser
                });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Asistencias.AnyAsync(e => e.AsistenciaId == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }



        //DELETE api/asistencia/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Delete(int id)
        {
            var asistencias = await _context.Asistencias.FindAsync(id);
            if (asistencias == null)
            {
                return NotFound();
            }

            _context.Asistencias.Remove(asistencias);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}