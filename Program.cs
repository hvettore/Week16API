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
            CitySelector();
            FetchAPI();
            

        }

        public static void CitySelector()
        {
            var cities = new Dictionary<string, (string Latitude, string Longitude)>
            {
            { "Grimstad", ("58.34", "8.59") },
            { "Oslo", ("59.91", "10.75") },
            };

            (string Latitude, string Longitude) coordinates;
            string chosenCity;

            Console.WriteLine("This is Hugo's Weather Loggin Application!");

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
        }

        public static void FetchAPI()
        {
            Details userMeasurements = GetUserMeasurements();
            string url = $"https://api.met.no/weatherapi/locationforecast/2.0/compact?lat={coordinates.Latitude}&lon={coordinates.Longitude}";
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Week16API (https://github.com/HVettore/Week16API)");
            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                WeatherForecast forecast = JsonConvert.DeserializeObject<WeatherForecast>(responseBody);
                Details apiDataYR = forecast.properties.timeseries[0].data.instant.details;

                WeatherLogEntry logEntry = new WeatherLogEntry
                {
                    Date = DateTime.Now,
                    UserMeasurements = userMeasurements,
                    YrMeasurements = apiDataYR
                };
                SaveLogEntry(logEntry);

            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
        }


        public static Details GetUserMeasurements()
        {
            Details userMeasurements = new Details();

            Console.Write("Enter air temperature (°C): ");
            userMeasurements.air_temperature = double.Parse(Console.ReadLine());

            Console.Write("Enter wind speed (m/s): ");
            userMeasurements.wind_speed = double.Parse(Console.ReadLine());

            Console.Write("Enter relative humidity (%): ");
            userMeasurements.relative_humidity = double.Parse(Console.ReadLine());

            return userMeasurements;
        }

        public static void SaveLogEntry(WeatherLogEntry logEntry)
        {
            List<WeatherLogEntry> entries;
            if (File.Exists("weatherLogEntries.json"))
            {
                using (StreamReader file = File.OpenText("weatherLogEntries.json"))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    entries = serializer.Deserialize<List<WeatherLogEntry>>(new JsonTextReader(file));
                }
            }
            else
            {
                entries = new List<WeatherLogEntry>();
            }

            entries.Add(logEntry);

            using (StreamWriter file = File.CreateText("weatherLogEntries.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, entries);
            }
        }

    }
}
