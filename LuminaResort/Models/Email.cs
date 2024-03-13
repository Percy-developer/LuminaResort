using System.Net.Mail;
using System.Net.Mime;
using System.Net;
using System.Text;

namespace LuminaResort.Models
{
    public class Email
    {
        //Método encargado de enviar el email al usuario
        public void Enviar(Usuario usuario)
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
                email.To.Add(new MailAddress(usuario.Email));

                //Una copia el administrador de la cuenta
                email.To.Add(new MailAddress("LuminaResort2@outlook.com"));

                //Se agrega el emisor
                email.From = new MailAddress("LuminaResort2@outlook.com");

                //se construye el Html para el body del email
                string html = "Bienvenido "+ usuario.NombreCompleto + " a LuminaResort gracias por formar parte de nuestro equipo";
                html += "<br>Su dirección y contraseña son:";
                html += "<br><b>Email: </b>" + usuario.Email;
                html += "<br><b>Contraseña: </b>" + usuario.Password;
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

            }
            catch (Exception ex)  //se captura el error
            {

                throw ex;  //se envia el error a la capa de aplicación
            }
        }
    }
}
