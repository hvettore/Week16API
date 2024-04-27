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
            Console.Clear();
            Console.WriteLine("This is Hugo's Weather Loggin Application!");
            WeatherLog weatherLog = new WeatherLog();
            MenuPrint(weatherLog);
        }

        public static async Task MenuPrint(WeatherLog weatherLog)
        {
            while (true)
            {
                Console.WriteLine("\nMenu:");
                Console.WriteLine("1. Log New Weather Data");
                Console.WriteLine("2. View Logged Reports");
                Console.WriteLine("3. Exit");
                Console.Write("Enter your choice: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        await LogWeatherData(weatherLog);
                        break;
                    case "2":
                        ViewReports(weatherLog);
                        break;
                    case "3":
                        SaveWeatherLog(weatherLog);
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please enter a valid input.");
                        break;
                }
            }
        }

        static async Task LogWeatherData(WeatherLog weatherLog)
        {
            Console.WriteLine("\nEnter Weather Data:");

            var coordinates = await CitySelector();
            var apiDataYR = await FetchWeatherData(coordinates);

            Console.Write("Date (yyyy-mm-dd): ");
            DateTime date;
            while (!DateTime.TryParse(Console.ReadLine(), out date))
            {
                Console.Write("Invalid date format. Please enter date in yyyy-mm-dd format: ");
            }
            Details userMeasurements = GetUserMeasurements();
            WeatherData newData = new WeatherData(date, userMeasurements.air_temperature, userMeasurements.wind_speed, userMeasurements.relative_humidity, apiDataYR);
            weatherLog.AddWeatherData(newData);

            Console.WriteLine("Weather data logged successfully!");
        }


        public async static Task<(string Latitude, string Longitude)> CitySelector()
        {
            var cities = new Dictionary<string, (string Latitude, string Longitude)>
            {
            { "Grimstad", ("58.34", "8.59") },
            { "Oslo", ("59.91", "10.75") },
            };


            string chosenCity;
            (string Latitude, string Longitude) coordinates;

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
            return coordinates;
        }



        public async static Task<Details> FetchWeatherData((string Latitude, string Longitude) coordinates)
        {
            string url = $"https://api.met.no/weatherapi/locationforecast/2.0/compact?lat={coordinates.Latitude}&lon={coordinates.Longitude}";
            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                Console.WriteLine($"HTTP response status code: {response.StatusCode}");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                WeatherForecast forecast = JsonConvert.DeserializeObject<WeatherForecast>(responseBody);
                Details apiDataYR = forecast.properties.timeseries[0].data.instant.details;

                return apiDataYR;
            }
            catch (Exception e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                return null;
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








        static void ViewReports(WeatherLog weatherLog)
        {
            Console.Clear();
            Console.WriteLine("\nView Reports:");
            Console.WriteLine("1. Daily Report");
            Console.WriteLine("2. Weekly Report");
            Console.WriteLine("3. Monthly Report");
            Console.Write("Enter your choice: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.Clear();
                    Console.Write("Enter date for daily report (yyyy-mm-dd): ");
                    DateTime dailyDate;
                    while (!DateTime.TryParse(Console.ReadLine(), out dailyDate))
                    {
                        Console.Write("Invalid date format. Please enter date in yyyy-mm-dd format: ");
                    }
                    weatherLog.PrintDailyReport(dailyDate);
                    break;
                case "2":
                    Console.Clear();
                    Console.Write("Enter start date for weekly report (yyyy-mm-dd): ");
                    DateTime startDate;
                    while (!DateTime.TryParse(Console.ReadLine(), out startDate))
                    {
                        Console.Write("Invalid date format. Please enter date in yyyy-mm-dd format: ");
                    }
                    weatherLog.PrintWeeklyReport(startDate);
                    break;
                case "3":
                    Console.Clear();
                    Console.Write("Enter month and year for monthly report (yyyy-mm): ");
                    DateTime month;
                    while (!DateTime.TryParseExact(Console.ReadLine(), "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out month))
                    {
                        Console.Write("Invalid month format. Please enter month and year in yyyy-mm format: ");
                    }
                    weatherLog.PrintMonthlyReport(month);
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }

        static void SaveWeatherLog(WeatherLog weatherLog)
        {
            string json = JsonConvert.SerializeObject(weatherLog);
            File.WriteAllText("weatherlog.json", json);
        }

    }
}
