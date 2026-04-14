using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gimnasio.Models
{
    public class Rutinas
    {
        [Key]
        public int RutinaId { get; set; }


        [ForeignKey("Socios")]
        [Required(ErrorMessage = "Debe ingresar el ID del socio.")]
        public int SocioId { get; set; }


        [ForeignKey("Entrenadores")]
        [Required(ErrorMessage = "Debe ingresar el ID del entrenador.")]
        public int EntrenadorId { get; set; }


        [Required(ErrorMessage = "Debe ingresar el nombre de la rutina.")]
        [MaxLength (120, ErrorMessage = "El nombre de la rutina no puede exceder 120 caracteres.")]
        public String Nombre { get; set; } = string.Empty;


        [MaxLength (300, ErrorMessage = "El objetivo de la rutina no puede exceder 300 caracteres.")]
        public String? Objetivo { get; set; } = string.Empty;


        [Required(ErrorMessage = "Debe ingresar la fecha de inicio de la rutina.")]
        public DateOnly FechaInicio { get; set; }


        public DateOnly? FechaFin { get; set; }


        [Required(ErrorMessage = "Debe ingresar el estado de la rutina.")]
        public bool Activa { get; set; } = true;


        [Required(ErrorMessage = "Debe ingresar la fecha de creación de la rutina.")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;


    }
}