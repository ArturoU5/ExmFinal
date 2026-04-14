using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Gimnasio.Data;
using Gimnasio.Models;

namespace Gimnasio.Controllers
{
    [ApiController]
    [Route("api/ejercicio")]
    public class EjercicioController : ControllerBase
    {
        private readonly AppDbContext _context;
        public EjercicioController(AppDbContext context)
        {
            _context = context;
        }

        //GET api/ejercicio
        [HttpGet]
        [Authorize(Roles = "ADMIN,ENTRENADOR,SOCIO")]
        public async Task<IActionResult> Get()
        {
            var ejercicios = await _context.Ejercicios.Where(e => e.IsActive).ToListAsync();
            return Ok(ejercicios);
        }

        //GET api/ejercicio/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "ADMIN,ENTRENADOR,SOCIO")]
        public async Task<IActionResult> GetOnly(int id)
        {
            var ejercicio = await _context.Ejercicios.FindAsync(id);

            if (ejercicio == null)
            {
                return NotFound("Ejercicio no encontrado.");
            }

            return Ok(ejercicio);
        }

        //POST api/ejercicio
        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Post([FromBody] Ejercicios ejercicio)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _context.Ejercicios.Add(ejercicio);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetOnly), new { id = ejercicio.EjercicioId }, ejercicio);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    mensaje = "Error al registrar el ejercicio",
                    detalle = ex.InnerException?.Message ?? ex.Message
                });
            }
        }

        //PUT api/ejercicio/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Put(int id, [FromBody] Ejercicios ejercicio)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingEjercicio = await _context.Ejercicios.FindAsync(id);
            if (existingEjercicio == null)
            {
                return NotFound("Ejercicio no encontrado.");
            }

            existingEjercicio.Nombre = ejercicio.Nombre;
            existingEjercicio.Descripcion = ejercicio.Descripcion;
            existingEjercicio.GrupoMuscular = ejercicio.GrupoMuscular;
            existingEjercicio.IsActive = ejercicio.IsActive;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    mensaje = "Ejercicio actualizado correctamente",
                    ejercicio = existingEjercicio
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    mensaje = "Error al actualizar el ejercicio",
                    detalle = ex.InnerException?.Message ?? ex.Message
                });
            }
        }

        //DELETE api/ejercicio/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Delete(int id)
        {
            var ejercicio = await _context.Ejercicios.FindAsync(id);
            if (ejercicio == null)
            {
                return NotFound("Ejercicio no encontrado.");
            }

            _context.Ejercicios.Remove(ejercicio);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
