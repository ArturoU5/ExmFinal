using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using GimDeportivo.Data;
using GimDeportivo.Models;

namespace GimDeportivo.Controllers
{

    [ApiController]
    [Route("api/registro")]
    public class RegistroController : ControllerBase
    {
        private readonly AppDbContext _context;
        public RegistroController(AppDbContext context)
        {
            _context = context;
        }



        //GET api/registro
        [HttpGet]
        [Authorize(Roles = "ADMIN,ENTRENADOR,SOCIO")]
        public async Task<IActionResult> Get()
        {
            var registros = await _context.Registros.ToListAsync();
            return Ok(registros);
        }



        //GET api/registro/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "ADMIN,ENTRENADOR,SOCIO")]
        public async Task<IActionResult> GetOnly(int id)
        {
            var registro = await _context.Registros.FindAsync(id);

            if (registro == null)
            {
                return NotFound();
            }

            return Ok(registro);
        }



        //POST api/registro
        [HttpPost]
        [Authorize(Roles = "ADMIN,ENTRENADOR")]
        public async Task<IActionResult> Post([FromBody] Registro registro)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _context.Registros.Add(registro);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetOnly), new { id = registro.Id }, registro);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    mensaje = "Error al registrar el ingreso",
                    detalle = ex.Message
                });
            }
        }



        //PUT api/registro/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN,ENTRENADOR")]
        public async Task<IActionResult> Put(int id, [FromBody] Registro registro)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingRegistro = await _context.Registros.FindAsync(id);
            if (existingRegistro == null)
            {
                return NotFound("Error, registro no encontrado.");
            }

            existingRegistro.Usuario = registro.Usuario;
            existingRegistro.fecha = registro.fecha;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    mensaje = "Registro actualizado correctamente",
                    registro = existingRegistro
                });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Registros.AnyAsync(e => e.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }
        



        //DELETE api/registro/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Delete(int id)
        {
            var registro = await _context.Registros.FindAsync(id);
            if (registro == null)
            {
                return NotFound();
            }

            _context.Registros.Remove(registro);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}