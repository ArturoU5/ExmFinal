using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Gimnasio.Data;
using Gimnasio.Models;
using System.Security.Claims;

namespace Gimnasio.Controllers
{
    [ApiController]
    [Route("api/socio-membresia")]
    public class SocioMembresiaController : ControllerBase
    {
        private readonly AppDbContext _context;
        public SocioMembresiaController(AppDbContext context)
        {
            _context = context;
        }

        //GET api/socio-membresia/{socioId} - SOCIO puede ver solo su propia membresía
        [HttpGet("{socioId}")]
        [Authorize(Roles = "ADMIN,SOCIO")]
        public async Task<IActionResult> GetBySocio(int socioId)
        {
            var socio = await _context.Socios.FindAsync(socioId);
            if (socio == null)
            {
                return NotFound("Socio no encontrado.");
            }

            // SOCIO solo puede ver su propia membresía
            if (User.IsInRole("SOCIO"))
            {
                var userIdClaim = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                if (socio.UserId != userIdClaim)
                {
                    return Forbid("No tiene permiso para ver esta membresía.");
                }
            }

            var membresias = await _context.SocioMembresia
                .Where(sm => sm.SocioId == socioId)
                .ToListAsync();

            return Ok(membresias);
        }

        //GET api/socio-membresia/{socioId}/{socioMembresiaId}
        [HttpGet("{socioId}/{socioMembresiaId}")]
        [Authorize(Roles = "ADMIN,SOCIO")]
        public async Task<IActionResult> GetOnly(int socioId, int socioMembresiaId)
        {
            var socio = await _context.Socios.FindAsync(socioId);
            if (socio == null)
            {
                return NotFound("Socio no encontrado.");
            }

            var membresia = await _context.SocioMembresia.FindAsync(socioMembresiaId);
            if (membresia == null || membresia.SocioId != socioId)
            {
                return NotFound("Membresía del socio no encontrada.");
            }

            // SOCIO solo puede ver su propia membresía
            if (User.IsInRole("SOCIO"))
            {
                var userIdClaim = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                if (socio.UserId != userIdClaim)
                {
                    return Forbid("No tiene permiso para ver esta membresía.");
                }
            }

            return Ok(membresia);
        }

        //POST api/socio-membresia - SOLO ADMIN
        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Post([FromBody] SocioMembresia socioMembresia)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validar que el socio existe
            var socioExists = await _context.Socios.AnyAsync(s => s.SocioId == socioMembresia.SocioId);
            if (!socioExists)
            {
                return BadRequest(new
                {
                    mensaje = "Error de validación",
                    detalle = $"El socio con ID {socioMembresia.SocioId} no existe."
                });
            }

            // Validar que la membresía existe
            var membresiaExists = await _context.Membresias.AnyAsync(m => m.MembresiaId == socioMembresia.MembresiaId);
            if (!membresiaExists)
            {
                return BadRequest(new
                {
                    mensaje = "Error de validación",
                    detalle = $"La membresía con ID {socioMembresia.MembresiaId} no existe."
                });
            }

            // Validar que la fecha de fin sea mayor a la de inicio
            if (socioMembresia.FechaFin <= socioMembresia.FechaInicio)
            {
                return BadRequest(new
                {
                    mensaje = "Error de validación",
                    detalle = "La fecha de fin debe ser posterior a la fecha de inicio."
                });
            }

            // Validar que el monto pagado sea válido
            if (socioMembresia.MontoPagado < 0)
            {
                return BadRequest(new
                {
                    mensaje = "Error de validación",
                    detalle = "El monto pagado no puede ser negativo."
                });
            }

            try
            {
                _context.SocioMembresia.Add(socioMembresia);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetOnly), new { socioId = socioMembresia.SocioId, socioMembresiaId = socioMembresia.SocioMembresiaId }, socioMembresia);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    mensaje = "Error al asignar membresía al socio",
                    detalle = ex.InnerException?.Message ?? ex.Message
                });
            }
        }

        //PUT api/socio-membresia/{socioMembresiaId}
        [HttpPut("{socioMembresiaId}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Put(int socioMembresiaId, [FromBody] SocioMembresia socioMembresia)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existing = await _context.SocioMembresia.FindAsync(socioMembresiaId);
            if (existing == null)
            {
                return NotFound("Membresía del socio no encontrada.");
            }

            if (socioMembresia.FechaFin <= socioMembresia.FechaInicio)
            {
                return BadRequest(new
                {
                    mensaje = "Error de validación",
                    detalle = "La fecha de fin debe ser posterior a la fecha de inicio."
                });
            }

            if (socioMembresia.MontoPagado < 0)
            {
                return BadRequest(new
                {
                    mensaje = "Error de validación",
                    detalle = "El monto pagado no puede ser negativo."
                });
            }

            existing.FechaInicio = socioMembresia.FechaInicio;
            existing.FechaFin = socioMembresia.FechaFin;
            existing.Estado = socioMembresia.Estado;
            existing.MontoPagado = socioMembresia.MontoPagado;
            existing.Notas = socioMembresia.Notas;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    mensaje = "Membresía del socio actualizada correctamente",
                    socioMembresia = existing
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    mensaje = "Error al actualizar membresía del socio",
                    detalle = ex.InnerException?.Message ?? ex.Message
                });
            }
        }

        //DELETE api/socio-membresia/{socioMembresiaId}
        [HttpDelete("{socioMembresiaId}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Delete(int socioMembresiaId)
        {
            var socioMembresia = await _context.SocioMembresia.FindAsync(socioMembresiaId);
            if (socioMembresia == null)
            {
                return NotFound("Membresía del socio no encontrada.");
            }

            _context.SocioMembresia.Remove(socioMembresia);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
