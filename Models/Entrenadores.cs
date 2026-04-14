using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gimnasio.Models
{
    public class Entrenadores
    {
        [Key]
        public int EntrenadorId { get; set; }


        [ForeignKey("Users")]
        [Required(ErrorMessage = "Debe ingresar el ID del usuario.")]
        public int UserId { get; set; }


        [MaxLength (120, ErrorMessage = "La especialidad del entrenador no puede exceder 120 caracteres.")]
        public String? Especialidad { get; set; } = string.Empty;


        [MaxLength (250, ErrorMessage = "La certificación del entrenador no puede exceder 250 caracteres.")]
        public String? Certificaciones { get; set; } = string.Empty;


        [MaxLength (250, ErrorMessage = "La certificación del entrenador no puede exceder 250 caracteres.")]
        [Required(ErrorMessage = "Debe ingresar la fecha de ingreso del entrenador.")]
        public DateOnly FechaIngreso { get; set; } = DateOnly.FromDateTime(DateTime.Now);


        [Required(ErrorMessage = "Debe ingresar el estado del entrenador.")]
        public bool IsActive { get; set; } = true;
    }
}