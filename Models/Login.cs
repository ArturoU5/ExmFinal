using Microsoft.EntityFrameworkCore;

namespace Gimnasio.Models
{
    public class Login
    {
        public string Nombre { get; set; } = "";
        public string Password { get; set; } = "";
    }
}