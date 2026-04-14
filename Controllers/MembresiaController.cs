using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Gimnasio.Data;
using Gimnasio.Models;

namespace Gimnasio.Controllers
{
    [ApiController]
    [Route("api/membresia")]
    public class MembresiaController : ControllerBase
    {
        private readonly AppDbContext _context;
        public MembresiaController(AppDbContext context)
        {
            _context = context;
        }

        //GET api/membresia
        [HttpGet]
        [Authorize(Roles = "ADMIN,ENTRENADOR,SOCIO")]
        public async Task<IActionResult> Get()
        {
            var membresias = await _context.Membresias.Where(m => m.IsActive).ToListAsync();
            return Ok(membresias);
        }

        //GET api/membresia/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "ADMIN,ENTRENADOR,SOCIO")]
        public async Task<IActionResult> GetOnly(int id)
        {
            var membresia = await _context.Membresias.FindAsync(id);

            if (membresia == null)
            {
                return NotFound("Membresía no encontrada.");
            }

            return Ok(membresia);
        }

        //POST api/membresia - SOLO ADMIN
        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Post([FromBody] Membresias membresia)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (membresia.Precio <= 0)
            {
                return BadRequest(new
                {
                    mensaje = "Error de validación",
                    detalle = "El precio de la membresía debe ser mayor a 0."
                });
            }

            if (membresia.DuracionDias <= 0)
            {
                return BadRequest(new
                {
                    mensaje = "Error de validación",
                    detalle = "La duración de la membresía debe ser mayor a 0 días."
                });
            }

            try
            {
                _context.Membresias.Add(membresia);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetOnly), new { id = membresia.MembresiaId }, membresia);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    mensaje = "Error al registrar la membresía",
                    detalle = ex.InnerException?.Message ?? ex.Message
                });
            }
        }

        //PUT api/membresia/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Put(int id, [FromBody] Membresias membresia)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingMembresia = await _context.Membresias.FindAsync(id);
            if (existingMembresia == null)
            {
                return NotFound("Membresía no encontrada.");
            }

            if (membresia.Precio <= 0)
            {
                return BadRequest(new
                {
                    mensaje = "Error de validación",
                    detalle = "El precio de la membresía debe ser mayor a 0."
                });
            }

            if (membresia.DuracionDias <= 0)
            {
                return BadRequest(new
                {
                    mensaje = "Error de validación",
                    detalle = "La duración de la membresía debe ser mayor a 0 días."
                });
            }

            existingMembresia.Nombre = membresia.Nombre;
            existingMembresia.Descripcion = membresia.Descripcion;
            existingMembresia.DuracionDias = membresia.DuracionDias;
            existingMembresia.Precio = membresia.Precio;
            existingMembresia.EsRenovable = membresia.EsRenovable;
            existingMembresia.IsActive = membresia.IsActive;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    mensaje = "Membresía actualizada correctamente",
                    membresia = existingMembresia
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    mensaje = "Error al actualizar la membresía",
                    detalle = ex.InnerException?.Message ?? ex.Message
                });
            }
        }

        //DELETE api/membresia/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Delete(int id)
        {
            var membresia = await _context.Membresias.FindAsync(id);
            if (membresia == null)
            {
                return NotFound("Membresía no encontrada.");
            }

            _context.Membresias.Remove(membresia);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
