using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gimnasio.Models
{
    public class Roles
    {
        [Key]
        public int RoleId { get; set; }


        [Required(ErrorMessage = "Debe ingresar el nombre del rol.")]
        [MaxLength(50, ErrorMessage = "El nombre del rol no puede exceder 50 caracteres.")]
        public string Name { get; set; } = string.Empty;


        [Required(ErrorMessage = "Debe ingresar el nombre normalizado del rol.")]
        [MaxLength(50, ErrorMessage = "El nombre normalizado del rol no puede exceder 50 caracteres.")]
        public String NormalizedName { get; set; } = string.Empty;


        [Required(ErrorMessage = "Debe ingresar el estado del rol.")]
        public bool IsActive { get; set; } = true;


        [Required(ErrorMessage = "Debe ingresar la fecha de creación del rol.")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}