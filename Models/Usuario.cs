using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GimDeportivo.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Error, el nombre es necesario.")]
        [MaxLength(50, ErrorMessage = "Error, el nombre no puede tener más de 50 caracteres.")]
        public string Nombre { get; set; } ="";

        [Required(ErrorMessage = "Error, el apellido es necesario.")]
        [MaxLength(50, ErrorMessage = "Error, el apellido no puede tener más de 50 caracteres.")]
        public String Apellido { get; set; } ="";

        [Required(ErrorMessage = "Error, debe ingresar roll del cliente.")]
        public int Roll { get; set; }

        [Required(ErrorMessage = "Error, debe ingresar el registro del cliente.")]
        public int Registro { get; set; }

        [Required(ErrorMessage = "Error, debe ingresar la membresia del cliente.")]
        public int Membresia { get; set; }

        [Required(ErrorMessage = "Error, debe ingresar la rutina del cliente.")]
        [MaxLength(500, ErrorMessage = "Error, la rutina no puede tener más de 500 caracteres.")]
        public String Rutina { get; set; } ="";

        [Required(ErrorMessage = "Error, debe ingresar una contraseña.")]
        [MaxLength(15, ErrorMessage = "Error, la contraseña no puede tener más de 15 caracteres.")]
        public String Password { get; set; } = "";

    }
}