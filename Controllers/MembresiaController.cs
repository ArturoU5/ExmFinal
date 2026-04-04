using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using GimDeportivo.Data;
using GimDeportivo.Models;

namespace GimDeportivo.Controllers
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
            var membresias = await _context.Membresias.ToListAsync();
            return Ok(membresias);
        }



        //GET api/membresia/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "ADMIN,ENTRENADOR,SOCIO")]
        public async Task<IActionResult> GetOnly(int id)
        {
            var membresias = await _context.Membresias.FindAsync(id);

            if (membresias == null)
            {
                return NotFound();
            }

            return Ok(membresias);
        }



        //POST api/membresias
        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Post([FromBody] Membresia membresia)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _context.Membresias.Add(membresia);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetOnly), new { id = membresia.Id }, membresia);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    mensaje = "Error al crear la membresía",
                    detalle = ex.Message
                });
            }
        }



        //PUT api/membresias/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Put(int id, [FromBody] Membresia membresia)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingMembresia = await _context.Membresias.FindAsync(id);
            if (existingMembresia == null)
            {
                return NotFound("Error, membresía no encontrada.");
            }

            existingMembresia.Tipo = membresia.Tipo;
            existingMembresia.fechaInicio = membresia.fechaInicio;
            existingMembresia.fechaFin = membresia.fechaFin;
            existingMembresia.Estado = membresia.Estado;
            existingMembresia.Costo = membresia.Costo;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    mensaje = "Membresía actualizada correctamente",
                    membresia = existingMembresia
                });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Membresias.AnyAsync(e => e.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }
        



        //DELETE api/membresias/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Delete(int id)
        {
            var membresias = await _context.Membresias.FindAsync(id);
            if (membresias == null)
            {
                return NotFound();
            }

            _context.Membresias.Remove(membresias);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}