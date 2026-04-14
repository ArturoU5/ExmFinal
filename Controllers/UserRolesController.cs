using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Gimnasio.Data;
using Gimnasio.Models;

namespace Gimnasio.Controllers
{
    [ApiController]
    [Route("api/user-roles")]
    public class UserRolesController : ControllerBase
    {
        private readonly AppDbContext _context;
        public UserRolesController(AppDbContext context)
        {
            _context = context;
        }

        //GET api/user-roles/{userId}
        [HttpGet("{userId}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetByUser(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound("Usuario no encontrado.");
            }

            var userRoles = await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .ToListAsync();

            return Ok(userRoles);
        }

        //POST api/user-roles - SOLO ADMIN
        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Post([FromBody] UserRoles userRole)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validar que el usuario existe
            var userExists = await _context.Users.AnyAsync(u => u.UserId == userRole.UserId);
            if (!userExists)
            {
                return BadRequest(new
                {
                    mensaje = "Error de validación",
                    detalle = $"El usuario con ID {userRole.UserId} no existe."
                });
            }

            // Validar que el rol existe
            var roleExists = await _context.Roles.AnyAsync(r => r.RoleId == userRole.RoleId);
            if (!roleExists)
            {
                return BadRequest(new
                {
                    mensaje = "Error de validación",
                    detalle = $"El rol con ID {userRole.RoleId} no existe."
                });
            }

            // Validar que el usuario no tenga ya asignado este rol
            var roleAlreadyAssigned = await _context.UserRoles
                .AnyAsync(ur => ur.UserId == userRole.UserId && ur.RoleId == userRole.RoleId);
            if (roleAlreadyAssigned)
            {
                return BadRequest(new
                {
                    mensaje = "Error de validación",
                    detalle = "Este usuario ya tiene asignado este rol."
                });
            }

            try
            {
                _context.UserRoles.Add(userRole);
                await _context.SaveChangesAsync();
                return Created("", userRole);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    mensaje = "Error al asignar rol al usuario",
                    detalle = ex.InnerException?.Message ?? ex.Message
                });
            }
        }

        //DELETE api/user-roles/{userId}/{roleId}
        [HttpDelete("{userId}/{roleId}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Delete(int userId, int roleId)
        {
            var userRole = await _context.UserRoles
                .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);

            if (userRole == null)
            {
                return NotFound("Rol del usuario no encontrado.");
            }

            _context.UserRoles.Remove(userRole);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
