using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Gimnasio.Data;
using Gimnasio.Models;
using System.Security.Claims;

namespace Gimnasio.Controllers
{
    [ApiController]
    [Route("api/rutina")]
    public class RutinaController : ControllerBase
    {
        private readonly AppDbContext _context;
        public RutinaController(AppDbContext context)
        {
            _context = context;
        }

        //GET api/rutina
        [HttpGet]
        [Authorize(Roles = "ADMIN,ENTRENADOR")]
        public async Task<IActionResult> Get()
        {
            var rutinas = await _context.Rutinas.Where(r => r.IsActive).ToListAsync();
            return Ok(rutinas);
        }

        //GET api/rutina/{id} - SOCIO puede ver solo su rutina
        [HttpGet("{id}")]
        [Authorize(Roles = "ADMIN,ENTRENADOR,SOCIO")]
        public async Task<IActionResult> GetOnly(int id)
        {
            var rutina = await _context.Rutinas.FindAsync(id);

            if (rutina == null)
            {
                return NotFound("Rutina no encontrada.");
            }

            // SOCIO solo puede ver su propia rutina
            if (User.IsInRole("SOCIO"))
            {
                var socio = await _context.Socios.FirstOrDefaultAsync(s => s.SocioId == rutina.SocioId);
                var userIdClaim = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                
                if (socio?.UserId != userIdClaim)
                {
                    return Forbid("No tiene permiso para ver esta rutina.");
                }
            }

            return Ok(rutina);
        }

        //POST api/rutina
        [HttpPost]
        [Authorize(Roles = "ADMIN,ENTRENADOR")]
        public async Task<IActionResult> Post([FromBody] Rutinas rutina)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (rutina.FechaFin.HasValue && rutina.FechaFin <= rutina.FechaInicio)
            {
                return BadRequest(new
                {
                    mensaje = "Error de validación",
                    detalle = "La fecha de fin debe ser posterior a la fecha de inicio."
                });
            }

            // Validar que el socio existe
            var socioExists = await _context.Socios.AnyAsync(s => s.SocioId == rutina.SocioId);
            if (!socioExists)
            {
                return BadRequest(new
                {
                    mensaje = "Error de validación",
                    detalle = $"El socio con ID {rutina.SocioId} no existe."
                });
            }

            // Validar que el entrenador existe
            var entrenadorExists = await _context.Entrenadores.AnyAsync(e => e.EntrenadorId == rutina.EntrenadorId);
            if (!entrenadorExists)
            {
                return BadRequest(new
                {
                    mensaje = "Error de validación",
                    detalle = $"El entrenador con ID {rutina.EntrenadorId} no existe."
                });
            }

            try
            {
                _context.Rutinas.Add(rutina);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetOnly), new { id = rutina.RutinaId }, rutina);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    mensaje = "Error al registrar la rutina",
                    detalle = ex.InnerException?.Message ?? ex.Message
                });
            }
        }

        //PUT api/rutina/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN,ENTRENADOR")]
        public async Task<IActionResult> Put(int id, [FromBody] Rutinas rutina)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingRutina = await _context.Rutinas.FindAsync(id);
            if (existingRutina == null)
            {
                return NotFound("Rutina no encontrada.");
            }

            if (rutina.FechaFin.HasValue && rutina.FechaFin <= rutina.FechaInicio)
            {
                return BadRequest(new
                {
                    mensaje = "Error de validación",
                    detalle = "La fecha de fin debe ser posterior a la fecha de inicio."
                });
            }

            existingRutina.Nombre = rutina.Nombre;
            existingRutina.Objetivo = rutina.Objetivo;
            existingRutina.FechaInicio = rutina.FechaInicio;
            existingRutina.FechaFin = rutina.FechaFin;
            existingRutina.IsActive = rutina.IsActive;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    mensaje = "Rutina actualizada correctamente",
                    rutina = existingRutina
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    mensaje = "Error al actualizar la rutina",
                    detalle = ex.InnerException?.Message ?? ex.Message
                });
            }
        }

        //DELETE api/rutina/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN,ENTRENADOR")]
        public async Task<IActionResult> Delete(int id)
        {
            var rutina = await _context.Rutinas.FindAsync(id);
            if (rutina == null)
            {
                return NotFound("Rutina no encontrada.");
            }

            _context.Rutinas.Remove(rutina);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
