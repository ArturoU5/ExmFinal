using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Gimnasio.Models
{
    public class Users
    {
        [Key]
        public int UserId { get; set; }


        [Required(ErrorMessage = "Debe ingresar el nombre del usuario.")]
        [MaxLength(100, ErrorMessage = "El nombre del usuario no puede exceder 100 caracteres.")]
        public String UserName { get; set; } = string.Empty;


        [Required(ErrorMessage = "Debe ingresar el nombre normalizado del usuario.")]
        [MaxLength(100, ErrorMessage = "El nombre normalizado del usuario no puede exceder 100 caracteres.")]
        public String NormalizedUserName { get; set; } = string.Empty;


        [Required(ErrorMessage = "Debe ingresar el correo electrónico del usuario.")]
        [MaxLength(256, ErrorMessage = "El correo electrónico del usuario no puede exceder 256 caracteres.")]
        public String Email { get; set; } = string.Empty;


        [Required(ErrorMessage = "Debe ingresar el nombre normalizado del correo electrónico del usuario.")]
        [MaxLength(256, ErrorMessage = "El nombre normalizado del correo electrónico del usuario no puede exceder 256 caracteres.")]
        public String NormalizedEmail { get; set; } = string.Empty;


        [Required(ErrorMessage = "Debe ingresar el PasswordHash.")]
        [MaxLength(512, ErrorMessage = "El PasswordHash no puede exceder 512 caracteres.")]
        public String PasswordHash { get; set; } = string.Empty;


        [MaxLength(25, ErrorMessage = "El número de celular del usuario no puede exceder 25 caracteres.")]
        public String PhoneNumber { get; set; } = string.Empty;


        [Required(ErrorMessage = "Debe ingresar el estado del usuario.")]
        public bool IsActive { get; set; } = true;


        public DateTime? LastLoginAt { get; set; } = DateTime.Now;


        [Required(ErrorMessage = "Debe ingresar la fecha de creación del usuario.")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;


        public DateTime? UpdatedAt { get; set; }
    }
}