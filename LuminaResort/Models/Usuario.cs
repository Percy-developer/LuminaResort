using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace LuminaResort.Models
{
    public class Usuario
    {
        [Key]
        public int ID { get; set; }
        public string Email { get; set; }
        public string NombreCompleto { get; set; }
        public string Password { get; set; }
        public DateTime FechaRegistro { get; set; }
        public char Estado { get; set; }
        public int Restablecer { get; set; }
        public string Rol { get; set; }
    }
}
