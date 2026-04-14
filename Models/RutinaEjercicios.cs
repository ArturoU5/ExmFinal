using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gimnasio.Models
{
    public class RutinaEjercicios
    {
        [Key]
        public int RutinaId { get; set; }


        [ForeignKey("Ejercicios")]
        [Required(ErrorMessage = "Debe ingresar el ID del ejercicio.")]
        public int EjercicioId { get; set; }


        [Required(ErrorMessage = "Debe ingresar el ordende la serie.")]
        public int Orden { get; set; }


        public int? Series { get; set; }


        public int? Repeticiones { get; set; }


        [Precision(6, 2)]
        public decimal? PesoObjetivoKg { get; set; }


        public int? DuracionSegundos { get; set; }


        public int? DescansoSegundos { get; set; }


        [MaxLength(250, ErrorMessage = "Las notas no pueden exceder 250 caracteres.")]
        public String? Notas { get; set; } = string.Empty;
    }
}