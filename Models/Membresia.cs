using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace GimDeportivo.Models
{
    public class Membresia
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Error, debe ingresar el tipo de membresía.")]
        [MaxLength(50, ErrorMessage = "Error, el tipo no puede tener más de 50 caracteres.")]
        public String Tipo { get; set; } = "";

        [Required(ErrorMessage = "Error, debe ingresar la fecha de inicio de la membresía.")]
        public DateOnly fechaInicio { get; set; }

        [Required(ErrorMessage = "Error, debe ingresar la fecha de fin de la membresía.")]
        public DateOnly fechaFin { get; set; }

        [Required(ErrorMessage = "Error, debe ingresar el estado de la membresía.")]
        public String Estado { get; set; } = "";

        [Required(ErrorMessage = "Error, debe ingresar un costo correcto.")]
        [Precision(18, 2)]
        public double Costo { get; set; }
    }
}