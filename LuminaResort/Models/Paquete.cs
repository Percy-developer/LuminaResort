using System.ComponentModel.DataAnnotations;

namespace LuminaResort.Models
{
    public class Paquete
    {
        [Key]
        public string IdPaquete { get; set; }

        [Required(ErrorMessage = "Por favor digite el nombre del paquete")]
        [DataType(DataType.Text)]
        public string NombrePaquete { get; set; }

        [Required(ErrorMessage = "Por favor digite la descripción del paquete")]
        [DataType(DataType.Text)]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "Por favor digite el costo del paquete")]
        [Range(0, Int32.MaxValue)]
        public decimal CostoPaquete { get; set; }

        [Required(ErrorMessage = "Por favor digite la mensualidad del paquete")]
        [DataType(DataType.Text)]
        public string MensualidadPaquete { get; set; }       
    }
}
