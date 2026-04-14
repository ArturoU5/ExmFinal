using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Gimnasio.Data;
using Gimnasio.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using Gimnasio.Models;

namespace Gimnasio.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] Login login)
        {
            var user = _context.Users
                .FirstOrDefault(u => u.UserName == login.Nombre && u.PasswordHash == login.Password);

            if (user == null)
                return Unauthorized("Credenciales inválidas");

            // Obtener el rol del usuario
            var roleName = (from ur in _context.UserRoles
                            join r in _context.Roles on ur.RoleId equals r.RoleId
                            where ur.UserId == user.UserId
                            select r.Name)
                           .FirstOrDefault() ?? "SOCIO"; // Asignar "SOCIO" si no se encuentra un rol

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, roleName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }
    }
}