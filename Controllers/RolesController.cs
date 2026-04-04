using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using GimDeportivo.Data;
using GimDeportivo.Models;

namespace GimDeportivo.Controllers
{

    [ApiController]
    [Route("api/Roles")]
    public class RolesController : ControllerBase
    {
        private readonly AppDbContext _context;
        public RolesController(AppDbContext context)
        {
            _context = context;
        }



        //GET api/roles
        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Get()
        {
            var Roles = await _context.Roles.ToListAsync();
            return Ok(Roles);
        }



        //GET api/roles/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetOnly(int id)
        {
            var Roles = await _context.Roles.FindAsync(id);

            if (Roles == null)
            {
                return NotFound();
            }

            return Ok(Roles);
        }



        //POST api/roles
        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Post([FromBody] Roles roles)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _context.Roles.Add(roles);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetOnly), new { id = roles.Id }, roles);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    mensaje = "Error al crear el rol",
                    detalle = ex.Message
                });
            }
        }



        //PUT api/roles/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Put(int id, [FromBody] Roles roles)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingRole = await _context.Roles.FindAsync(id);
            if (existingRole == null)
            {
                return NotFound("Error, rol no encontrado.");
            }

            existingRole.Nombre = roles.Nombre;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    mensaje = "Rol actualizado correctamente",
                    rol = existingRole
                });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Roles.AnyAsync(e => e.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }



        //DELETE api/roles/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Delete(int id)
        {
            var roles = await _context.Roles.FindAsync(id);
            if (roles == null)
            {
                return NotFound();
            }

            _context.Roles.Remove(roles);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}