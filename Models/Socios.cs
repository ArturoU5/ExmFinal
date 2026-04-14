using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gimnasio.Models
{
    public class Socios
    {
        [Key]
        public int SocioId { get; set; }


        [ForeignKey("Users")]
        [Required(ErrorMessage = "Debe ingresar el ID del usuario.")]
        public int UserId { get; set; }


        public DateOnly? FechaNacimiento { get; set; }


        [MaxLength (1, ErrorMessage = "Debe escoger entre 'M' para masculino o 'F' para femenino.")]
        public String? Genero { get; set; } = string.Empty;


        [Precision(5, 2)]
        public decimal? AlturaCm { get; set; }


        [Precision(6, 2)]
        public decimal? PesoKg { get; set; }


        [MaxLength(120, ErrorMessage = "El nombre del contacto de emergencia no puede exceder 120 caracteres.")]
        public String? EmergenciaNombre { get; set; } = string.Empty;


        [MaxLength(25, ErrorMessage = "El teléfono de emergencia no puede exceder 25 caracteres.")]
        public String? EmergenciaTelefono { get; set; } = string.Empty;


        [Required(ErrorMessage = "Debe ingresar la fecha de registro del socio.")]
        public DateOnly FechaRegistro { get; set; } = DateOnly.FromDateTime(DateTime.Now);


        [Required(ErrorMessage = "Debe ingresar el estado del socio.")]
        public bool IsActive { get; set; } = true;
    }
}