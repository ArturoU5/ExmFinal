using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gimnasio.Models
{
    public class UserRoles
    {
        [Key]
        public int UserId { get; set; }

        [ForeignKey("Roles")]
        [Required(ErrorMessage = "Debe ingresar el ID del rol.")]
        public int RoleId { get; set; }


        [Required(ErrorMessage = "Debe ingresar la fecha de asignación del rol.")]
        public DateTime AssignedAt { get; set; } = DateTime.Now;
    }
}