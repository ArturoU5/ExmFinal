using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace GimDeportivo.Models
{
    public class Registro
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Error, la fecha es necesaria.")]
        public DateTime fecha { get; set; }

        [Required(ErrorMessage = "Error, debe ingresar el usuario.")]
        public int Usuario { get; set; }
    }
}