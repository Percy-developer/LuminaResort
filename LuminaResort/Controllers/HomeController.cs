using LuminaResort.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace LuminaResort.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private LuminaAPI reservaAPI = null;
        private HttpClient client = null;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            reservaAPI = new LuminaAPI();
            client = reservaAPI.Start();
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

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
