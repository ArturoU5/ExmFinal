using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Gimnasio.Data;
using Gimnasio.Models;
using System.Security.Claims;

namespace Gimnasio.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        //GET api/users
        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Get()
        {
            var users = await _context.Users.Where(u => u.IsActive).ToListAsync();
            return Ok(users);
        }

        //GET api/users/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetOnly(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound("Usuario no encontrado.");
            }

            return Ok(user);
        }

        //POST api/users - SOLO ADMIN
        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Post([FromBody] Users user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validar que el username no exista
            var userExists = await _context.Users
                .AnyAsync(u => u.UserName == user.UserName);
            if (userExists)
            {
                return BadRequest(new
                {
                    mensaje = "Error de validación",
                    detalle = "El nombre de usuario ya existe."
                });
            }

            // Validar que el email no exista
            var emailExists = await _context.Users
                .AnyAsync(u => u.Email == user.Email);
            if (emailExists)
            {
                return BadRequest(new
                {
                    mensaje = "Error de validación",
                    detalle = "El correo electrónico ya existe."
                });
            }

            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetOnly), new { id = user.UserId }, user);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    mensaje = "Error al registrar el usuario",
                    detalle = ex.InnerException?.Message ?? ex.Message
                });
            }
        }

        //PUT api/users/{id} 
        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Put(int id, [FromBody] Users user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null)
            {
                return NotFound("Usuario no encontrado.");
            }

            // Validar que el nuevo username no exista (si cambió)
            if (user.UserName != existingUser.UserName)
            {
                var userNameExists = await _context.Users
                    .AnyAsync(u => u.UserName == user.UserName);
                if (userNameExists)
                {
                    return BadRequest(new
                    {
                        mensaje = "Error de validación",
                        detalle = "El nombre de usuario ya existe."
                    });
                }
            }

            // Validar que el nuevo email no exista (si cambió)
            if (user.Email != existingUser.Email)
            {
                var emailExists = await _context.Users
                    .AnyAsync(u => u.Email == user.Email);
                if (emailExists)
                {
                    return BadRequest(new
                    {
                        mensaje = "Error de validación",
                        detalle = "El correo electrónico ya existe."
                    });
                }
            }

            existingUser.UserName = user.UserName;
            existingUser.NormalizedUserName = user.NormalizedUserName;
            existingUser.Email = user.Email;
            existingUser.NormalizedEmail = user.NormalizedEmail;
            existingUser.PhoneNumber = user.PhoneNumber;
            existingUser.PasswordHash = user.PasswordHash;
            existingUser.IsActive = user.IsActive;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    mensaje = "Usuario actualizado correctamente",
                    user = existingUser
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    mensaje = "Error al actualizar el usuario",
                    detalle = ex.InnerException?.Message ?? ex.Message
                });
            }
        }

        //DELETE api/users/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound("Usuario no encontrado.");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
