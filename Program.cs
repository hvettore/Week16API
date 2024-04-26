using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Globalization;
using Newtonsoft.Json;



namespace WeatherLogApp
{
    class Program
    {
        static readonly HttpClient client = new HttpClient();
        static void Main(string[] args)
        {
            Console.Clear();
            var cities = new Dictionary<string, (string Latitude, string Longitude)>
            {
                { "Grimstad", ("58.34", "8.59") },
                { "Oslo", ("59.91", "10.75") },
                { "Bergen", ("60.39", "5.32") },
                { "London", ("51.51", "-0.13") },
                { "Berlin", ("52.52", "13.41") },
                { "Paris", ("48.86", "2.35") },
                { "New York", ("40.71", "-74.01") }
            };

            string chosenCity;
            (string Latitude, string Longitude) coordinates;
            

            Console.WriteLine("This is Hugo's Weather Logging Application!");

            do
            {
                Console.WriteLine("\nPlease enter the city you want to get the weather data for:");
                foreach (var city in cities.Keys)
                {
                    Console.WriteLine(city);
                }

                chosenCity = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(chosenCity))
                {
                    chosenCity = "Grimstad";
                }

            } while (!cities.TryGetValue(chosenCity, out coordinates));

            WeatherLog weatherLog = LoadWeatherLog();


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
                        Console.WriteLine("Exiting Weather Logging App...");
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

            Console.Write("Temperature (Celsius): ");
            float temperature = float.Parse(Console.ReadLine());

            Console.Write("Wind Speed (m/s): ");
            float wind_speed = float.Parse(Console.ReadLine());

            WeatherData newData = new WeatherData(date, temperature, wind_speed);
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

        static WeatherLog LoadWeatherLog()
        {
            if (File.Exists("weatherlog.json"))
            {
                string json = File.ReadAllText("weatherlog.json");
                return JsonConvert.DeserializeObject<WeatherLog>(json);
            }
            else
            {
                return new WeatherLog();
            }
        }
        static void SaveWeatherLog(WeatherLog weatherLog)
        {
            string json = JsonConvert.SerializeObject(weatherLog);
            File.WriteAllText("weatherlog.json", json);
        }
    }

    class WeatherData
    {
        public DateTime Date { get; set; }
        public float Temperature { get; set; }
        public float WindSpeed { get; set; }

        public WeatherData(DateTime date, float temperature, float wind_speed)
        {
            Date = date;
            Temperature = temperature;
            WindSpeed = wind_speed;
        }
    }

    class WeatherLog
    {
        public List<WeatherData> Data { get; set; }

        public WeatherLog()
        {
            Data = new List<WeatherData>();
        }

        public void AddWeatherData(WeatherData newData)
        {
            Data.Add(newData);
        }

        public void PrintDailyReport(DateTime date)
        {
            bool found = false;
            foreach (var data in Data)
            {
                if (data.Date.Date == date.Date)
                {
                    found = true;
                    Console.WriteLine($"Weather Report for {date.ToShortDateString()}:");
                    Console.WriteLine($"Temperature: {data.Temperature}°C");
                    Console.WriteLine($"Wind Speed: {data.WindSpeed}m/s");
                    break;
                }
            }
            if (!found)
            {
                Console.WriteLine("No data found for the specified date.");
            }
        }

        public void PrintWeeklyReport(DateTime startDate)
        {
            DateTime endDate = startDate.AddDays(6);
            bool found = false;

            Console.WriteLine($"Weekly Report from {startDate.ToShortDateString()} to {endDate.ToShortDateString()}:");

            foreach (var data in Data)
            {
                if (data.Date.Date >= startDate.Date && data.Date.Date <= endDate.Date)
                {
                    found = true;
                    Console.WriteLine($"Date: {data.Date.ToShortDateString()}");
                    Console.WriteLine($"Temperature: {data.Temperature}°C");
                    Console.WriteLine($"Wind Speed: {data.WindSpeed}m/s");
                    Console.WriteLine();
                }
            }

            if (!found)
            {
                Console.WriteLine("No data found for the specified week.");
            }
        }

        public void PrintMonthlyReport(DateTime month)
        {
            bool found = false;
            Console.WriteLine($"Monthly Report for {month.ToString("MMMM yyyy")}:");
            foreach (var data in Data)
            {
                if (data.Date.Month == month.Month && data.Date.Year == month.Year)
                {
                    found = true;
                    Console.WriteLine($"Date: {data.Date.ToShortDateString()}");
                    Console.WriteLine($"Temperature: {data.Temperature}°C");
                    Console.WriteLine($"Wind Speed: {data.WindSpeed}m/s");
                    Console.WriteLine();
                }
            }
            if (!found)
            {
                Console.WriteLine("No data found for the specified month.");
            }
        }
    }
}
