using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace GimDeportivo.Models
{
    public class Roles
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Error, el nombre es necesario.")]
        [MaxLength(50, ErrorMessage = "Error, el nombre no puede tener más de 50 caracteres.")]
        public string Nombre { get; set; } = "";
    }
}