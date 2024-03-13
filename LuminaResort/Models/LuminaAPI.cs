namespace LuminaResort.Models
{
    public class LuminaAPI
    {
        public HttpClient Start()
        {
            //Manage the HttpClient object
            var client = new HttpClient();

            //client.BaseAddress = new Uri("https://localhost:7112/");
            //URL API
            client.BaseAddress = new Uri("http://www.luminaresortapi.somee.com/");

            // Return the object
            return client;
        }
    }
}
