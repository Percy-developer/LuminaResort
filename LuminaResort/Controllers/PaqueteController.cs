using LuminaResort.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System;
using System.Net.Http;

namespace LuminaResort.Controllers
{
    public class PaqueteController : Controller
    {
        //Handle the API reference
        private LuminaAPI reservaAPI = null;

        //Handle the HttpClient reference
        private HttpClient client = null;


        /// <summary> Contructor that inicialize the objects </summary>
        public PaqueteController()
        {
            // Instance the API object
            reservaAPI = new LuminaAPI();

            // Inicialize the HttpClient object
            client = reservaAPI.Start();
        }

        /// <summary> Shows the list of reservas </summary>
        public async Task<IActionResult> ListaPaquetes()
        {
            // Reservas list empty
            var list = new List<Paquete>();

            //Use the method of the API 
            HttpResponseMessage response = await client.GetAsync("Paquetes/Listado");

            // Ask if the result is correct
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                // Read de data in Json Format
                var result = response.Content.ReadAsStringAsync().Result;
                // Convert to object list
                list = JsonConvert.DeserializeObject<List<Paquete>>(result);
            }
            else
            {
                // If some error apears, save it into alert
                TempData["MessageIndex"] = "Can't find data from the API";
            }
            // Send the list to the view
            return (list.Count > 0) ? View(list) : View(new List<Paquete>());
        }

        [HttpGet]
        public async Task<IActionResult> ConsultarPaquetes(string id)
        {
            Paquete temp = new Paquete();
            HttpResponseMessage response = await client.GetAsync($"Paquetes/Consultar?id={id}");

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                temp = JsonConvert.DeserializeObject<Paquete>(result);
            }
            return View(temp);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind] Paquete paquete)
        {
            // Utiliza el método agregar de la API, enviando el objeto paquete con sus datos
            var agregar = await client.PostAsJsonAsync<Paquete>("/Paquetes/Agregar", paquete);

            if (agregar.IsSuccessStatusCode)
            {
                return RedirectToAction("ListaPaquetes");
            }
            else
            {
                // La solicitud no fue exitosa, puedes acceder al contenido del error
                TempData["Mensaje"] = "No se logró registrar el paquete.";
                return RedirectToAction("ListaPaquetes");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            //variable para almacenar los datos del Usuario
            var paquete = new Paquete();

            //se utiliza el método para consultar el Usuario por medio de su  ID
            HttpResponseMessage response = await client.GetAsync($"Paquetes/Consultar?id={id}");

            //Si todo fue correcto
            if (response.IsSuccessStatusCode)
            {
                //se realiza la lectura de los datos en formato JSON
                var resultado = response.Content.ReadAsStringAsync().Result;

                //Se convierte el  JSON  en  un  Object
                paquete = JsonConvert.DeserializeObject<Paquete>(resultado);
            }

            //se envia la  view con los datos del Usuario
            return View(paquete);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind] Paquete paquete)
        {
            //se utiliza el método modificar de la API
            var modificar = client.PutAsJsonAsync<Paquete>("Paquetes/Modificar", paquete);
            await modificar; //Esperamos

            //Se toma el resultado
            var resultado = modificar.Result;
            //Si todo fue correcto
            if (resultado.IsSuccessStatusCode)
            {
                return RedirectToAction("ListaPaquetes");  //Se ubica al usuario dentro del listado de paquetes
            }
            else  //Algo paso hay un error en los datos
            {
                TempData["Mensaje"] = "Datos incorrectos";
                return View(paquete);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            Paquete temp = new Paquete();
            HttpResponseMessage response = await client.GetAsync($"Paquetes/Consultar?id={id}");

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var resultado = response.Content.ReadAsStringAsync().Result;
                temp = JsonConvert.DeserializeObject<Paquete>(resultado);
            }
            return View(temp);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            //variable para almacenar la informacion del Usuario
            var paquete = new Paquete();

            //se consulta el usuario a eliminar
            HttpResponseMessage mensaje = await client.GetAsync($"Paquetes/Consultar?id={id}");

            if (mensaje.IsSuccessStatusCode)//Si todo está correcto
            {
                //Se realiza lectura datos en formato JSON
                var resultado = mensaje.Content.ReadAsStringAsync().Result;

                //se convierte el JSON en  un object
                paquete = JsonConvert.DeserializeObject<Paquete>(resultado);
            }
            //se retorna la view con los datos del Usuario
            return View(paquete);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeletePaquete(string id)
        {
            //se utiliza el método eliminar de la API
            HttpResponseMessage response = await client.DeleteAsync($"Paquetes/Eliminar?id={id}");

            //se ubica al usuario dentro del listado usuarios
            return RedirectToAction("ListaPaquetes");
        }
    }
}
