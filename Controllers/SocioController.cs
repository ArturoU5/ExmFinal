using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Gimnasio.Data;
using Gimnasio.Models;
using System.Security.Claims;

namespace Gimnasio.Controllers
{
    [ApiController]
    [Route("api/socio")]
    public class SocioController : ControllerBase
    {
        private readonly AppDbContext _context;
        public SocioController(AppDbContext context)
        {
            _context = context;
        }

        //GET api/socio - ADMIN y ENTRENADOR
        [HttpGet]
        [Authorize(Roles = "ADMIN,ENTRENADOR")]
        public async Task<IActionResult> Get()
        {
            var socios = await _context.Socios.Where(s => s.IsActive).ToListAsync();
            return Ok(socios);
        }

        //GET api/socio/{id} - SOCIO puede ver solo su perfil
        [HttpGet("{id}")]
        [Authorize(Roles = "ADMIN,ENTRENADOR,SOCIO")]
        public async Task<IActionResult> GetOnly(int id)
        {
            var socio = await _context.Socios.FindAsync(id);

            if (socio == null)
            {
                return NotFound("Socio no encontrado.");
            }

            // SOCIO solo puede ver su propio perfil
            if (User.IsInRole("SOCIO"))
            {
                var userIdClaim = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                if (socio.UserId != userIdClaim)
                {
                    return Forbid("No tiene permiso para ver este socio.");
                }
            }

            return Ok(socio);
        }

        //POST api/socio - SOLO ADMIN
        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Post([FromBody] Socios socio)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validar que el usuario existe
            var userExists = await _context.Users.AnyAsync(u => u.UserId == socio.UserId);
            if (!userExists)
            {
                return BadRequest(new
                {
                    mensaje = "Error de validación",
                    detalle = $"El usuario con ID {socio.UserId} no existe."
                });
            }

            // Validar que el usuario no sea ya un socio
            var socioExists = await _context.Socios.AnyAsync(s => s.UserId == socio.UserId);
            if (socioExists)
            {
                return BadRequest(new
                {
                    mensaje = "Error de validación",
                    detalle = "Este usuario ya está registrado como socio."
                });
            }

            try
            {
                _context.Socios.Add(socio);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetOnly), new { id = socio.SocioId }, socio);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    mensaje = "Error al registrar el socio",
                    detalle = ex.InnerException?.Message ?? ex.Message
                });
            }
        }

        //PUT api/socio/{id} - ADMIN y SOCIO (su propio perfil)
        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN,SOCIO")]
        public async Task<IActionResult> Put(int id, [FromBody] Socios socio)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingSocio = await _context.Socios.FindAsync(id);
            if (existingSocio == null)
            {
                return NotFound("Socio no encontrado.");
            }

            // SOCIO solo puede actualizar su propio perfil
            if (User.IsInRole("SOCIO"))
            {
                var userIdClaim = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                if (existingSocio.UserId != userIdClaim)
                {
                    return Forbid("No tiene permiso para actualizar este socio.");
                }
            }

            existingSocio.FechaNacimiento = socio.FechaNacimiento;
            existingSocio.Genero = socio.Genero;
            existingSocio.AlturaCm = socio.AlturaCm;
            existingSocio.PesoKg = socio.PesoKg;
            existingSocio.EmergenciaNombre = socio.EmergenciaNombre;
            existingSocio.EmergenciaTelefono = socio.EmergenciaTelefono;
            
            if (User.IsInRole("ADMIN"))
            {
                existingSocio.IsActive = socio.IsActive;
            }

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    mensaje = "Socio actualizado correctamente",
                    socio = existingSocio
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    mensaje = "Error al actualizar el socio",
                    detalle = ex.InnerException?.Message ?? ex.Message
                });
            }
        }

        //DELETE api/socio/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Delete(int id)
        {
            var socio = await _context.Socios.FindAsync(id);
            if (socio == null)
            {
                return NotFound("Socio no encontrado.");
            }

            _context.Socios.Remove(socio);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
