using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace IdentityConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var client = new HttpClient();
            var discovery = await client.GetDiscoveryDocumentAsync("http://localhost:5000/");

            if (discovery.IsError)
            {
                Console.WriteLine(discovery.Error);
                return;
            }

            var accecToken = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = discovery.TokenEndpoint,
                ClientId = "m2m.client",
                ClientSecret = "511536EF-F270-4058-80CA-1C89C192F69A", //511536EF-F270-4058-80CA-1C89C192F69A
                Scope = "scope1"
            });

            if (accecToken.IsError)
            {
                Console.WriteLine(accecToken.Error);
                return;
            }

            //call api
            var apiClient = new HttpClient();
            apiClient.SetBearerToken(accecToken.AccessToken);

            var respones = await apiClient.GetAsync("http://localhost:5001/WeatherForecast");
            if (!respones.IsSuccessStatusCode)
            {
                Console.WriteLine(respones.StatusCode);
            }
            else
            {
                var content = respones.Content.ReadAsStringAsync();
                //Console.WriteLine(JArray.Parse)
            }

            Console.ReadKey();
        }
    }
}
