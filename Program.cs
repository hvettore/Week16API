using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Globalization;
using Newtonsoft.Json;

namespace WeatherLogApp
{
    class Program
    {
        static HttpClient client = new HttpClient();
        static async Task Main(string[] args)
        {
            var cities = new Dictionary<string, (string Latitude, string Longitude)>
            {
            { "Grimstad", ("58.34", "8.59") },
            { "Oslo", ("59.91", "10.75") },
            };

            (string Latitude, string Longitude) coordinates;
            string chosenCity;

            while (true)
            {
                Console.WriteLine("Please choose a city: ");
                foreach (var city in cities.Keys)
                {
                    Console.WriteLine(city);
                }
                Console.WriteLine();

                chosenCity = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(chosenCity))
                {
                    coordinates = cities["Grimstad"];
                    break;
                }

                if (cities.TryGetValue(chosenCity, out coordinates))
                {
                    break;
                }

                Console.WriteLine("Invalid city name.");
            }

            string url = $"https://api.met.no/weatherapi/locationforecast/2.0/compact?lat={coordinates.Latitude}&lon={coordinates.Longitude}";

            client.DefaultRequestHeaders.UserAgent.ParseAdd("Week16API (https://github.com/HVettore/Week16API)");
            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine("API Response Body:");
                Console.WriteLine(responseBody);


                WeatherForecast forecast = JsonConvert.DeserializeObject<WeatherForecast>(responseBody);
                Details yrMeasurements = forecast.properties.timeseries[0].data.instant.details;

            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }

        }
    }
}
