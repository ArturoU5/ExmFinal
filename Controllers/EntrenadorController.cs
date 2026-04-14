using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Gimnasio.Data;
using Gimnasio.Models;
using System.Security.Claims;

namespace Gimnasio.Controllers
{
    [ApiController]
    [Route("api/entrenador")]
    public class EntrenadorController : ControllerBase
    {
        private readonly AppDbContext _context;
        public EntrenadorController(AppDbContext context)
        {
            _context = context;
        }

        //GET api/entrenador
        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Get()
        {
            var entrenadores = await _context.Entrenadores.Where(e => e.IsActive).ToListAsync();
            return Ok(entrenadores);
        }

        //GET api/entrenador/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "ADMIN,ENTRENADOR")]
        public async Task<IActionResult> GetOnly(int id)
        {
            var entrenador = await _context.Entrenadores.FindAsync(id);

            if (entrenador == null)
            {
                return NotFound("Entrenador no encontrado.");
            }

            // ENTRENADOR solo puede ver su propio perfil
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (User.IsInRole("ENTRENADOR") && entrenador.UserId.ToString() != userIdClaim)
            {
                return Forbid("No tiene permiso para ver este entrenador.");
            }

            return Ok(entrenador);
        }

        //POST api/entrenador - SOLO ADMIN
        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Post([FromBody] Entrenadores entrenador)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validar que el usuario existe
            var userExists = await _context.Users.AnyAsync(u => u.UserId == entrenador.UserId);
            if (!userExists)
            {
                return BadRequest(new
                {
                    mensaje = "Error de validación",
                    detalle = $"El usuario con ID {entrenador.UserId} no existe."
                });
            }

            // Validar que el usuario no sea ya un entrenador
            var trainerExists = await _context.Entrenadores.AnyAsync(e => e.UserId == entrenador.UserId);
            if (trainerExists)
            {
                return BadRequest(new
                {
                    mensaje = "Error de validación",
                    detalle = "Este usuario ya está registrado como entrenador."
                });
            }

            try
            {
                _context.Entrenadores.Add(entrenador);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetOnly), new { id = entrenador.EntrenadorId }, entrenador);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    mensaje = "Error al registrar el entrenador",
                    detalle = ex.InnerException?.Message ?? ex.Message
                });
            }
        }

        //PUT api/entrenador/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Put(int id, [FromBody] Entrenadores entrenador)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingEntrenador = await _context.Entrenadores.FindAsync(id);
            if (existingEntrenador == null)
            {
                return NotFound("Entrenador no encontrado.");
            }

            existingEntrenador.Especialidad = entrenador.Especialidad;
            existingEntrenador.Certificaciones = entrenador.Certificaciones;
            existingEntrenador.IsActive = entrenador.IsActive;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    mensaje = "Entrenador actualizado correctamente",
                    entrenador = existingEntrenador
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    mensaje = "Error al actualizar el entrenador",
                    detalle = ex.InnerException?.Message ?? ex.Message
                });
            }
        }

        //DELETE api/entrenador/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Delete(int id)
        {
            var entrenador = await _context.Entrenadores.FindAsync(id);
            if (entrenador == null)
            {
                return NotFound("Entrenador no encontrado.");
            }

            _context.Entrenadores.Remove(entrenador);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
