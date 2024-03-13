using LuminaResort.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

using System.Net.Http.Headers;
using System.Net;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System;
using System.Net.Http;
using System.Net.Http.Json;

namespace LuminaResort.Controllers
{
    public class ReservaController : Controller
    {
        //Handle the API reference
        private LuminaAPI reservaAPI = null;

        //Handle the HttpClient reference
        private HttpClient client = null;


        /// <summary> Contructor that inicialize the objects </summary>
        public ReservaController()
        {
            // Instance the API object
            reservaAPI = new LuminaAPI();

            // Inicialize the HttpClient object
            client = reservaAPI.Start();
        }

        /// <summary> Shows the list of reservas </summary>
        public async Task<IActionResult> ListaReservas()
        {
            // Reservas list empty
            var list = new List<Reserva>();

            //Use the method of the API 
            HttpResponseMessage response = await client.GetAsync("Reservaciones/Listado");

            // Ask if the result is correct
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                // Read de data in Json Format
                var result = response.Content.ReadAsStringAsync().Result;
                // Convert to object list
                list = JsonConvert.DeserializeObject<List<Reserva>>(result);
            }
            else
            {
                // If some error apears, save it into alert
                TempData["MessageIndex"] = "Can't find data from the API";
            }
            // Send the list to the view
            return (list.Count > 0) ? View(list) : View(new List<Reserva>());
        }

        [HttpGet]
        public async Task<IActionResult> ConsultarReserva(int id)
        {
            Reserva temp = new Reserva();
            HttpResponseMessage response = await client.GetAsync($"Reservaciones/Consultar?id={id}");

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                temp = JsonConvert.DeserializeObject<Reserva>(result);
            }
            return View(temp);
        }

        [HttpGet]
        public IActionResult Reservar()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reservar([Bind] Reserva reserva)
        {
            // Utiliza el método agregar de la API, enviando el objeto reserva con sus datos
            var agregar = await client.PostAsJsonAsync<Reserva>("/Reservaciones/Agregar", reserva);


            if (agregar.IsSuccessStatusCode)
            {

                EnviarEmail(reserva);

                TempData["MensajeCreado"] = "Email enviado al Usuario";

                return RedirectToAction("Reservar");
            }
            else
            {
                // La solicitud no fue exitosa, puedes acceder al contenido del error
                TempData["Mensaje"] = "No se logró registrar la reserva.";
                return RedirectToAction("Reservar");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<Paquete> paquetes = new List<Paquete>();
            HttpResponseMessage response = await client.GetAsync("Paquetes/Listado");

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                paquetes = JsonConvert.DeserializeObject<List<Paquete>>(result);
            }

            return View(paquetes ?? new List<Paquete>());
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            //variable para almacenar los datos del Usuario
            var reserva = new Reserva();

            //se utiliza el método para consultar la Reserva por medio de su  ID
            HttpResponseMessage response = await client.GetAsync($"Reservaciones/Consultar?id={id}");

            //Si todo fue correcto
            if (response.IsSuccessStatusCode)
            {
                //se realiza la lectura de los datos en formato JSON
                var resultado = response.Content.ReadAsStringAsync().Result;

                //Se convierte el  JSON  en  un  Object
                reserva = JsonConvert.DeserializeObject<Reserva>(resultado);
            }

            //se envia la  view con los datos de la reserva
            return View(reserva);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind] Reserva reserva)
        {
            //se utiliza el método modificar de la API
            var modificar = client.PutAsJsonAsync<Reserva>("/Reservaciones/Edit", reserva);
            await modificar; //Esperamos

            //Se toma el resultado
            var resultado = modificar.Result;
            //Si todo fue correcto
            if (resultado.IsSuccessStatusCode)
            {
                return RedirectToAction("ListaReservas");  //Se ubica al usuario dentro del listado Usuarios
            }
            else  //Algo paso hay un error en los datos
            {
                TempData["Mensaje"] = "Datos incorrectos";
                return View(reserva); //mostramos la view con los datos del usuario  
            }
        }



        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            //variable para almacenar la informacion del Usuario
            var reserva = new Reserva();

            //se consulta el usuario a eliminar
            HttpResponseMessage mensaje = await client.GetAsync($"/Reservaciones/Consultar?id={id}");

            if (mensaje.IsSuccessStatusCode)//Si todo está correcto
            {
                //Se realiza lectura datos en formato JSON
                var resultado = mensaje.Content.ReadAsStringAsync().Result;

                //se convierte el JSON en  un object
                reserva = JsonConvert.DeserializeObject<Reserva>(resultado);
            }
            //se retorna la view con los datos del Usuario
            return View(reserva);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            //se utiliza el método eliminar de la API
            HttpResponseMessage response = await client.DeleteAsync($"/Reservaciones/Eliminar?id={id}");

            //se ubica al usuario dentro del listado usuarios
            return RedirectToAction("ListaReservas");
        }


        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            Reserva temp = new Reserva();
            HttpResponseMessage response = await client.GetAsync($"Reservaciones/Consultar?id={id}");

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var resultado = response.Content.ReadAsStringAsync().Result;
                temp = JsonConvert.DeserializeObject<Reserva>(resultado);
            }
            return View(temp);
        }

        private bool EnviarEmail(Reserva temp)
        {
            try
            {
                //variable control
                bool enviado = false;

                //Se instancia el objeto  email
                EmailReserva email = new EmailReserva();

                //Se utiliza el método para enviar el email
                email.Enviar(temp);

                //se indica que se envió el email
                enviado = true;

                //se retorna la variable control
                return enviado;
            }
            catch (Exception ex)
            {

                return false;
            }
        }//Cierre método

    }
}
