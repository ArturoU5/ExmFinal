using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using GimDeportivo.Data;
using GimDeportivo.Models;

namespace GimDeportivo.Controllers
{

    [ApiController]
    [Route("api/usuario")]
    public class UsuarioController : ControllerBase
    {
        private readonly AppDbContext _context;
        public UsuarioController(AppDbContext context)
        {
            _context = context;
        }



        //GET api/usuario
        [HttpGet]
        [Authorize(Roles = "ADMIN,ENTRENADOR")]
        public async Task<IActionResult> Get()
        {
            var usuarios = await _context.Usuarios.ToListAsync();
            return Ok(usuarios);
        }



        //GET api/usuario/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "ADMIN,ENTRENADOR")]
        public async Task<IActionResult> GetOnly(int id)
        {
            var usuarios = await _context.Usuarios.FindAsync(id);

            if (usuarios == null)
            {
                return NotFound();
            }

            return Ok(usuarios);
        }



        //POST api/usuarios
        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Post([FromBody] Usuario usuario)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetOnly), new { id = usuario.Id }, usuario);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    mensaje = "Error al crear el usuario",
                    detalle = ex.Message
                });
            }
        }



        //PUT api/usuario/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Put(int id, [FromBody] Usuario usuario)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingUser = await _context.Usuarios.FindAsync(id);
            if (existingUser == null)
            {
                return NotFound("Error, usuario no encontrado.");
            }

            existingUser.Nombre = usuario.Nombre;
            existingUser.Apellido = usuario.Apellido;
            existingUser.Roll = usuario.Roll;
            existingUser.Registro = usuario.Registro;
            existingUser.Membresia = usuario.Membresia;
            existingUser.Rutina = usuario.Rutina;
            existingUser.Password = usuario.Password;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    mensaje = "Usuario actualizado correctamente",
                    usuario = existingUser
                });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Usuarios.AnyAsync(e => e.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }



        //DELETE api/usuarios/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Delete(int id)
        {
            var usuarios = await _context.Usuarios.FindAsync(id);
            if (usuarios == null)
            {
                return NotFound();
            }

            _context.Usuarios.Remove(usuarios);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}