using LuminaResort.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http.Headers;
using System.Net;

namespace LuminaResort.Controllers
{
    public class UsuariosController : Controller
    {
        //Variable para manejar la referencia de la API publicada en la nube
        private LuminaAPI luminaAPI;

        //Variable para manejar las acciones del protocolo
        private HttpClient httpClient;

        public UsuariosController()
        {
            //se instancia el objeto que administras la  API
            luminaAPI = new LuminaAPI();

            //se inicializa la variable cliente
            httpClient = luminaAPI.Start();
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            //lista de Reservaciones
            List<Usuario> listado = new List<Usuario>();

            //se asigna la autorización por token
            httpClient.DefaultRequestHeaders.Authorization = AutorizacionToken();


            //Se utiliza el método de la API
            HttpResponseMessage response = await httpClient.GetAsync("/Usuarios/Listado");


            if (ValidarTransaccion(response.StatusCode) == false)
            {
                TempData["MensajeError"] = "No tienes los permisos para acceder a esta opción";
                return RedirectToAction("Index", "Home");
            }



            //Si todo fue correcto
            if (response.IsSuccessStatusCode)
            {
                //se realiza la lectura de datos
                var resultados = response.Content.ReadAsStringAsync().Result;

                //Se toman los datos en JSON se convierte en un listado de objetos
                listado = JsonConvert.DeserializeObject<List<Usuario>>(resultados);
            }


            //se retorna la view con la lista de los usuarios
            return View(listado);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind] Usuario pUsuario)
        {
            pUsuario.ID = 0;


            //se asigna la autorización por token
            httpClient.DefaultRequestHeaders.Authorization = AutorizacionToken();


            //Se utiliza el método agregar de la  API se envia el object Usuario con sus datos
            var agregar = httpClient.PostAsJsonAsync<Usuario>("/Usuarios/Agregar", pUsuario);

            await agregar; //se espera que termine

            //vemos su resultado
            var resultado = agregar.Result;

            //Se venció el token  por ende se debe cerrar la sesión
            if (resultado.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Logout", "Usuarios");
            }

            if (resultado.IsSuccessStatusCode) //si todo fue correcto
            {

                EnviarEmail(pUsuario);

                TempData["MensajeCreado"] = "Email enviado al Usuario";

                return RedirectToAction("Index"); //se ubica al usuario dentro del listado Usuarios
            }
            else  //En caso que no se agrego
            {
                TempData["Mensaje"] = "No se logró registrar el Usuario.."; //se muestra este mensaje de error dentro de un Alert
                return View(pUsuario);
            }

        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            //variable para almacenar los datos del Usuario
            var usuario = new Usuario();

            //se utiliza el método para consultar el Usuario por medio de su  ID
            HttpResponseMessage response = await httpClient.GetAsync($"/Usuarios/Consultar?id={id}");

            //Si todo fue correcto
            if (response.IsSuccessStatusCode)
            {
                //se realiza la lectura de los datos en formato JSON
                var resultado = response.Content.ReadAsStringAsync().Result;

                //Se convierte el  JSON  en  un  Object
                usuario = JsonConvert.DeserializeObject<Usuario>(resultado);
            }

            //se envia la  view con los datos del Usuario
            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind] Usuario pUsuario)
        {
            //se utiliza el método modificar de la API
            var modificar = httpClient.PutAsJsonAsync<Usuario>("/Usuarios/Modificar", pUsuario);
            await modificar; //Esperamos

            //Se toma el resultado
            var resultado = modificar.Result;
            //Si todo fue correcto
            if (resultado.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");  //Se ubica al usuario dentro del listado Usuarios
            }
            else  //Algo paso hay un error en los datos
            {
                TempData["Mensaje"] = "Datos incorrectos";
                return View(pUsuario); //mostramos la view con los datos del usuario  
            }
        }



        [HttpGet]
        public async Task<IActionResult> Eliminar(int id)
        {
            //variable para almacenar la informacion del Usuario
            var usuario = new Usuario();

            //se consulta el usuario a eliminar
            HttpResponseMessage mensaje = await httpClient.GetAsync($"/Usuarios/Consultar?id={id}");

            if (mensaje.IsSuccessStatusCode)//Si todo está correcto
            {
                //Se realiza lectura datos en formato JSON
                var resultado = mensaje.Content.ReadAsStringAsync().Result;

                //se convierte el JSON en  un object
                usuario = JsonConvert.DeserializeObject<Usuario>(resultado);
            }
            //se retorna la view con los datos del Usuario
            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Eliminar")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            //se utiliza el método eliminar de la API
            HttpResponseMessage response = await httpClient.DeleteAsync($"/Usuarios/Eliminar?id={id}");

            //se ubica al usuario dentro del listado usuarios
            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            Usuario temp = new Usuario();
            HttpResponseMessage response = await httpClient.GetAsync($"Usuarios/Consultar?id={id}");

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var resultado = response.Content.ReadAsStringAsync().Result;
                temp = JsonConvert.DeserializeObject<Usuario>(resultado);
            }
            return View(temp);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind] Usuario usuario)
        {
            AutorizacionResponse autorizacion = null;

            if (usuario == null)
            {
                TempData["MensajeLogin"] = "The email or password was incorrect";
                return View();
            }
            HttpResponseMessage response = await httpClient.PostAsync(
                $"/Usuarios/AutenticarPW?email={usuario.Email}&password={usuario.Password}", null);

            //si todo funciona  bien
            if (response.IsSuccessStatusCode)
            {
                //se realiza la lectura de los datos en formato JSON
                var resultado = response.Content.ReadAsStringAsync().Result;

                //Se convierte el  JSON  en  un  Object
                autorizacion = JsonConvert.DeserializeObject<AutorizacionResponse>(resultado);
            }

            //si la autorización es correcta
            if (autorizacion != null)
            {
                //se realiza el proceso de inicio sesión
                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                identity.AddClaim(new Claim(ClaimTypes.Name, usuario.Email));

                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                //se almacena el token dentro de la sesión
                HttpContext.Session.SetString("token", autorizacion.Token);

                //se ubica al usuario en la sección  home
                return RedirectToAction("Index", "Home");
            }
            else  //no está autorizado algo paso
            {
                TempData["MensajeLogin"] = "The email or password was incorrect";
                return View(usuario);
            }

        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(); //cerrar sesión
            HttpContext.Session.SetString("token", ""); //se borra el token

            return RedirectToAction("Login", "Usuarios"); //se ubica al usuario para login
        }

        private bool EnviarEmail(Usuario temp)
        {
            try
            {
                //variable control
                bool enviado = false;

                //Se instancia el objeto  email
                Email email = new Email();

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

        private AuthenticationHeaderValue AutorizacionToken()
        {
            //se extrae el token almacenado dentro de la sesión
            var token = HttpContext.Session.GetString("token");

            //Objeto que almacena los datos de authenticación del token
            AuthenticationHeaderValue autorizacion = null;

            //se valida el token
            if (token != null && token.Length != 0)
            {
                //se instancia la autenticación con los datos del access token
                autorizacion = new AuthenticationHeaderValue("Bearer", token);
            }

            //se retorna la autorización
            return autorizacion;
        }

        private bool ValidarTransaccion(HttpStatusCode resultado)
        {
            //Se venció el token  por ende se debe cerrar la sesión
            if (resultado == HttpStatusCode.Unauthorized)
            {
                TempData["MensajeSesion"] = "The session has been expired";
                return false;
            }
            else
            {
                TempData["MensajeSesion"] = null;
                return true;
            }
        }


    }
}
