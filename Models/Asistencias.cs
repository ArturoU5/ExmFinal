using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Gimnasio.Models
{
    public class Asistencias
    {
        [Key]
        public int AsistenciaId { get; set; }


        [ForeignKey("Socios")]
        [Required(ErrorMessage = "Debe ingresar el ID del socio.")]
        public int SocioId { get; set; }


        [Required(ErrorMessage = "Debe ingresar la fecha y hora de entrada.")]
        public DateTime FechaHoraEntrada { get; set; }


        [Required(ErrorMessage = "Debe ingresar la fecha y hora de salida.")]
        public DateTime FechaHoraSalida { get; set; }


        [MaxLength(300, ErrorMessage = "Las observaciones no pueden exceder 300 caracteres.")]
        [Required(ErrorMessage = "Debe ingresar observaciones sobre la asistencia.")]
        public String Observaciones { get; set; } = string.Empty;


        [ForeignKey("Users")]
        [Required(ErrorMessage = "Debe ingresar el ID del usuario que registra la asistencia.")]
        public int RegistradaPorUserId { get; set; }
    }
}