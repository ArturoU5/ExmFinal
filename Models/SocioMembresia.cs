using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gimnasio.Models
{
    public class SocioMembresia
    {
        [Key]
        public int SocioMembresiaId { get; set; }


        [ForeignKey("Socios")]
        [Required(ErrorMessage = "Debe ingresar el ID del socio.")]
        public int SocioId { get; set; }


        [ForeignKey("Membresias")]
        [Required(ErrorMessage = "Debe ingresar el ID de la membresía.")]
        public int MembresiaId { get; set; }


        [Required(ErrorMessage = "Debe ingresar la fecha de inicio de la membresía.")]
        public DateOnly FechaInicio { get; set; } = DateOnly.FromDateTime(DateTime.Now);


        [Required(ErrorMessage = "Debe ingresar la fecha de fin de la membresía.")]
        public DateOnly FechaFin { get; set; }


        [Required(ErrorMessage = "Debe ingresar el estado de la membresía.")]
        [MaxLength(20, ErrorMessage = "El estado de la membresía no puede exceder 20 caracteres.")]
        public String Estado { get; set; } = string.Empty;


        [Required(ErrorMessage = "Debe ingresar el monto pagado por la membresía.")]
        [Precision(10, 2)]
        public decimal MontoPagado { get; set; } = 0;


        [MaxLength(300, ErrorMessage = "Las notas de la membresía no pueden exceder 300 caracteres.")]
        public String? Notas { get; set; } = string.Empty;


        [Required(ErrorMessage = "Debe ingresar la fecha de creación del registro de membresía.")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}