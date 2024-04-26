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
        static Task Main(string[] args)
        {
            CitySelector();
            WeatherLog weatherLog = new WeatherLog();
            FetchAPI();
            MenuPrint(weatherLog);

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
        public async static void FetchAPI()
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

        public static void MenuPrint(WeatherLog weatherLog)
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
                        LogWeatherData(weatherLog);
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



        static void LogWeatherData(WeatherLog weatherLog)
        {
            Console.WriteLine("\nEnter Weather Data:");

            Console.Write("Date (yyyy-mm-dd): ");
            DateTime date;
            while (!DateTime.TryParse(Console.ReadLine(), out date))
            {
                Console.Write("Invalid date format. Please enter date in yyyy-mm-dd format: ");
            }

            GetUserMeasurements();

            WeatherData newData = new WeatherData(date, userMeasurements.air_temperature, userMeasurements.wind_speed, userMeasurements.relative_humidity);
            weatherLog.AddWeatherData(newData);

            Console.WriteLine("Weather data logged successfully!");
        }


        static void ViewReports(WeatherLog weatherLog)
        {
            Console.WriteLine("\nView Reports:");
            Console.WriteLine("1. Daily Report");
            Console.WriteLine("2. Weekly Report");
            Console.WriteLine("3. Monthly Report");
            Console.Write("Enter your choice: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.Write("Enter date for daily report (yyyy-mm-dd): ");
                    DateTime dailyDate;
                    while (!DateTime.TryParse(Console.ReadLine(), out dailyDate))
                    {
                        Console.Write("Invalid date format. Please enter date in yyyy-mm-dd format: ");
                    }
                    weatherLog.PrintDailyReport(dailyDate);
                    break;
                case "2":
                    Console.Write("Enter start date for weekly report (yyyy-mm-dd): ");
                    DateTime startDate;
                    while (!DateTime.TryParse(Console.ReadLine(), out startDate))
                    {
                        Console.Write("Invalid date format. Please enter date in yyyy-mm-dd format: ");
                    }
                    weatherLog.PrintWeeklyReport(startDate);
                    break;
                case "3":
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
