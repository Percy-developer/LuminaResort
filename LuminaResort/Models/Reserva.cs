using System.ComponentModel.DataAnnotations;

namespace LuminaResort.Models
{
    public class Reserva
    {
       

        [Required(ErrorMessage = "Por favor digite la cedula")]
        [DataType(DataType.Text)]
        public string Cedula { get; set; }

        [DataType(DataType.Text)]
        public string TipoCedula { get; set; }

        [DataType(DataType.Text)]
        public string NombreCompleto { get; set; }

        [Required(ErrorMessage = "Por favor digite su telefono")]
        [DataType(DataType.Text)]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "Por favor digite su direccion")]
        [DataType(DataType.Text)]
        public string Direccion { get; set; }

        [Required(ErrorMessage = "Por favor digite su correo electronico ")]
        [DataType(DataType.Text)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Por favor digite el tipo de paquete")]
        [DataType(DataType.Text)]
        public string TipoPaquete { get; set; }

        [Required(ErrorMessage = "Por favor digite su metodo de pago")]
        [DataType(DataType.Text)]
        public string TipoPago { get; set; }

        [Required(ErrorMessage = "Por favor digite la cantidad de noches")]
        public int CantidadNoches { get; set; }

        [Required(ErrorMessage = "Por favor digite la cantiadad de personas")]
        public int CantidadPersonas { get; set; }

        [Range(0, Int32.MaxValue)]
        public decimal CostoTotalReservacion { get; set; } = 1000;

        [DataType(DataType.Text)]
        public string NumeroCheque { get; set; } = "X";

        [DataType(DataType.Text)]
        public string Banco { get; set; } = "BCR";

        [Range(0, Int32.MaxValue)]
        public decimal Descuento { get; set; } = 0;

        [Range(0, Int32.MaxValue)]
        public decimal CostoTotalReservacionDolares { get; set; } = 0;

        [Key]
        public int IdReservacion { get; set; }

        public DateTime Fecha { get; set; } = DateTime.Now;
    }
}
