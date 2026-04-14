using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Gimnasio.Data;
using Gimnasio.Models;

namespace Gimnasio.Controllers
{
    [ApiController]
    [Route("api/roles")]
    public class RolesController : ControllerBase
    {
        private readonly AppDbContext _context;
        public RolesController(AppDbContext context)
        {
            _context = context;
        }

        //GET api/roles - SOLO ADMIN
        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Get()
        {
            var roles = await _context.Roles.Where(r => r.IsActive).ToListAsync();
            return Ok(roles);
        }

        //GET api/roles/{id} - SOLO ADMIN
        [HttpGet("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetOnly(int id)
        {
            var role = await _context.Roles.FindAsync(id);

            if (role == null)
            {
                return NotFound("Rol no encontrado.");
            }

            return Ok(role);
        }

        //POST api/roles - SOLO ADMIN
        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Post([FromBody] Roles role)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validar que el nombre del rol no exista
            var roleExists = await _context.Roles
                .AnyAsync(r => r.Name == role.Name);
            if (roleExists)
            {
                return BadRequest(new
                {
                    mensaje = "Error de validación",
                    detalle = "El nombre del rol ya existe."
                });
            }

            try
            {
                _context.Roles.Add(role);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetOnly), new { id = role.RoleId }, role);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    mensaje = "Error al registrar el rol",
                    detalle = ex.InnerException?.Message ?? ex.Message
                });
            }
        }

        //PUT api/roles/{id} - SOLO ADMIN
        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Put(int id, [FromBody] Roles role)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingRole = await _context.Roles.FindAsync(id);
            if (existingRole == null)
            {
                return NotFound("Rol no encontrado.");
            }

            // Validar que el nuevo nombre del rol no exista (si cambió)
            if (role.Name != existingRole.Name)
            {
                var nameExists = await _context.Roles
                    .AnyAsync(r => r.Name == role.Name);
                if (nameExists)
                {
                    return BadRequest(new
                    {
                        mensaje = "Error de validación",
                        detalle = "El nombre del rol ya existe."
                    });
                }
            }

            existingRole.Name = role.Name;
            existingRole.NormalizedName = role.NormalizedName;
            existingRole.IsActive = role.IsActive;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    mensaje = "Rol actualizado correctamente",
                    role = existingRole
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    mensaje = "Error al actualizar el rol",
                    detalle = ex.InnerException?.Message ?? ex.Message
                });
            }
        }

        //DELETE api/roles/{id} - SOLO ADMIN
        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Delete(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                return NotFound("Rol no encontrado.");
            }

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
