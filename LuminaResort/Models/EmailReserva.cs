using System.Net.Mail;
using System.Net.Mime;
using System.Net;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace LuminaResort.Models
{
    public class EmailReserva
    {

        public byte[] GenerarPDF(Reserva reserva)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                Document document = new Document();
                PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                document.Open();

                // Aquí agregas los detalles de la reserva al PDF
                document.Add(new Paragraph("Factura de la reserva:"));
                document.Add(new Paragraph($"Nombre: {reserva.NombreCompleto}"));
                document.Add(new Paragraph($"Cedula: {reserva.Cedula}"));
                document.Add(new Paragraph($"Cantidad de Noches: {reserva.CantidadNoches}"));
                document.Add(new Paragraph($"Cantidad de Personas: {reserva.CantidadPersonas}"));
                document.Add(new Paragraph($"Modalidad de Pago: {reserva.TipoPago}"));
                if (reserva.TipoPago == "Cheque")
                {
                    document.Add(new Paragraph($"Numero de Cheque: {reserva.NumeroCheque}"));
                    document.Add(new Paragraph($"Banco: {reserva.Banco}"));
                    // Agrega más detalles según necesites
                }
                document.Add(new Paragraph($"Rebajas aplicadas: {reserva.Descuento}"));
                document.Add(new Paragraph($"Costo Total de la reservacion en colones: {reserva.CostoTotalReservacion}"));
                document.Add(new Paragraph($"Costo Total de la reservacion en dolares: {reserva.CostoTotalReservacionDolares}"));
                document.Add(new Paragraph($"Reservacion realizada el día: {reserva.Fecha}"));

                document.Close();
                return memoryStream.ToArray();
            }
        }


        //Método encargado de enviar el email al cliente
        public void Enviar(Reserva reserva)
        {
            //Se intenta enviar el email
            try
            {
                //Se crea una instancia del objeto Email
                MailMessage email = new MailMessage();

                //Asunto
                email.Subject = "Datos de registro en plataforma LuminaResort";

                //destinatarios
                //Se agrega al email el correo del usuario
                email.To.Add(new MailAddress(reserva.Email));

                //Una copia el administrador de la cuenta
                email.To.Add(new MailAddress("LuminaResort2@outlook.com"));

                //Se agrega el emisor
                email.From = new MailAddress("LuminaResort2@outlook.com");

                //se construye el Html para el body del email
                string html = "Bienvenido " + reserva.NombreCompleto + " a LuminaResort gracias por preferirnos";
                html += "<br>Los detalles de su reserva se encuentra en el siguiente pdf:";

                // Generar el PDF
                byte[] pdfBytes = GenerarPDF(reserva);

                // Adjuntar el PDF al correo
                MemoryStream stream = new MemoryStream(pdfBytes);
                Attachment pdfAttachment = new Attachment(stream, "reserva.pdf", MediaTypeNames.Application.Pdf);
                email.Attachments.Add(pdfAttachment);


                html += "<br><b>No responda este correo porque fue generado de forma automatica.";
                html += " Que tenga buen día.</b>";

                //se indica que el contenido es en html
                email.IsBodyHtml = true;

                //se indica la prioridad, recomendación utilizar prioridad normal
                email.Priority = MailPriority.Normal; //No utilizar alta porque algunos firewall bloquean estos tipos de emails

                //Se intancia la vista del html para el cuerpo del email
                AlternateView view = AlternateView.CreateAlternateViewFromString(html, Encoding.UTF8, MediaTypeNames.Text.Html);

                //se agrega la view al email
                email.AlternateViews.Add(view);

                //Se configura el protocolo de comunicación smtp
                SmtpClient smtp = new SmtpClient();

                //Nombre del servidor smtp a sincronizar la cuenta
                smtp.Host = "smtp-mail.outlook.com";

                //se indica el número de puerto para la comunicación
                smtp.Port = 587;

                //Se indica si utiliza seguridad tipo SSL
                smtp.EnableSsl = true;

                //se indica las credenciales por default para el buzón de correo
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("LuminaResort2@outlook.com", "LRP2024*");

                //se envia el email
                smtp.Send(email);

                //se liberan los recursos
                email.Dispose();
                smtp.Dispose();
                stream.Dispose();

            }
            catch (Exception ex)  //se captura el error
            {

                throw ex;  //se envia el error a la capa de aplicación
            }
        }
    }
}