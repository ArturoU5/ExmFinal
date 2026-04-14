using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gimnasio.Models
{
    public class Membresias
    {
        [Key]
        public int MembresiaId { get; set; }


        [MaxLength (100, ErrorMessage = "El nombre de la membresía no puede exceder 100 caracteres.")]
        [Required(ErrorMessage = "Debe ingresar el nombre de la membresía.")]
        public String Nombre { get; set; } = string.Empty;


        [MaxLength (300, ErrorMessage = "La descripción de la membresía no puede exceder 300 caracteres.")]
        public String? Descripcion { get; set; } = string.Empty;


        [Required(ErrorMessage = "Debe ingresar la duración de la membresía en días.")]
        public int DuracionDias { get; set; }


        [Precision(10, 2)]
        [Required(ErrorMessage = "Debe ingresar el precio de la membresía.")]
        public decimal Precio { get; set; }


        [Required(ErrorMessage = "Debe indicar si la membresía es renovable.")]
        public bool EsRenovable { get; set; } = true;


        [Required(ErrorMessage = "Debe ingresar el estado de la membresía.")]
        public bool IsActive { get; set; } = true;


        [Required(ErrorMessage = "Debe ingresar la fecha de creación de la membresía.")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}