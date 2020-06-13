using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Newtonsoft.Json.Linq;

namespace ConsoleClient
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var client = new HttpClient();

            // Request discory document
            Console.WriteLine("Get dicovery document of Identity provider site ...");
            var discovery = await client.GetDiscoveryDocumentAsync("https://localhost:5000/");
            if (discovery.IsError)
            {
                Console.WriteLine($"Failed: {discovery.Error}");
                Exit(-1);
                return;
            }
            Console.WriteLine($"Get discovery document successfully:\n\t{discovery.Issuer}\n\t{discovery.TokenEndpoint}");

            // Request client token, using TokenEndpoint、ClientID+ClientSecret(writed in Identity Provider Config)
            Console.WriteLine($"Get Console Client token ...");
            var token = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest()
            {
                Address = discovery.TokenEndpoint,
                ClientId = "ConsoleClient",
                ClientSecret = "511536EF-F270-4058-80CA-1C89C192F69A",
                Scope = "api1"
            });
            if (token.IsError)
            {
                Console.WriteLine($"Failed: {token.ErrorDescription}");
                Exit(-2);
                return;
            }
            Console.WriteLine($"Get Console Client token successfully:\n\t{token.AccessToken.Left()}");

            // Call api
            Console.WriteLine($"Get Weather Forecast ...");
            var apiClient = new HttpClient();
            apiClient.SetBearerToken(token.AccessToken);
            var apiResponse = await apiClient.GetAsync("https://localhost:5001/WeatherForecast");
            if (!apiResponse.IsSuccessStatusCode)
            {
                Console.WriteLine($"Failed: {apiResponse.StatusCode}");
                Exit(-3);
                return;
            }
            var content = await apiResponse.Content.ReadAsStringAsync();
            Console.WriteLine($"Get Weather Forecast successfully: {JArray.Parse(content)}");
            var userClaims = apiResponse.Headers.GetValues("UserClaims") as IEnumerable<string>;
            Console.WriteLine($"User Claims:\n\t{string.Join("\n\t", userClaims)}");

            Console.Read();
        }

        private static void Exit(int exitCode = 0)
        {
            Console.WriteLine($"Exit Code = {exitCode}");
            Console.Read();
        }
    }
}
