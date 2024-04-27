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
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Hugo's WeatherLogApp");

            // Replace with a specific date and coordinates
            string date = "2024-04-27";
            (string Latitude, string Longitude) coordinates = ("59.9127", "10.7461"); // Coordinates for Oslo

            Details weatherDetails = await FetchWeatherData(date, coordinates);
            if (weatherDetails != null)
            {
                Console.WriteLine($"Temperature: {weatherDetails.air_temperature}");
                Console.WriteLine($"Wind speed: {weatherDetails.wind_speed}");
                Console.WriteLine($"Humidity: {weatherDetails.relative_humidity}");
            }
            else
            {
                Console.WriteLine("No weather data found for the specified date and coordinates.");
            }
        }


        public async static Task<Details> FetchWeatherData(string date, (string Latitude, string Longitude) coordinates)
        {
            string url = $"https://api.met.no/weatherapi/locationforecast/2.0/compact?lat={coordinates.Latitude}&lon={coordinates.Longitude}";
            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                WeatherForecast forecast = JsonConvert.DeserializeObject<WeatherForecast>(responseBody);
                Details weatherDetails = forecast.properties.timeseries
                    .Where(t => t.time.Date == DateTime.Parse(date).Date)
                    .Select(t => t.data.instant.details)
                    .FirstOrDefault();

                return weatherDetails;
            }
            catch (Exception e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                return null;
            }
        }
    }
}
