using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gimnasio.Models
{
    public class Ejercicios
    {
        [Key]
        public int EjercicioId { get; set; }


        [MaxLength (120, ErrorMessage = "El nombre del ejercicio no puede exceder 120 caracteres.")]
        [Required(ErrorMessage = "Debe ingresar el nombre del ejercicio.")]
        public String Nombre { get; set; } = string.Empty;


        [MaxLength (400, ErrorMessage = "La descripción del ejercicio no puede exceder 400 caracteres.")]
        [Required(ErrorMessage = "Debe ingresar la descripción del ejercicio.")]
        public String Descripcion { get; set; } = string.Empty;


        [MaxLength (60, ErrorMessage = "El grupo muscular no puede exceder 60 caracteres.")]
        [Required(ErrorMessage = "Debe ingresar el grupo muscular del ejercicio.")]
        public String GrupoMuscular { get; set; } = string.Empty;


        [Required(ErrorMessage = "Debe ingresar el estado del ejercicio.")]
        public bool IsActive { get; set; } = true;
    }
}