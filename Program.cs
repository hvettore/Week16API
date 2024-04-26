using System.Globalization;
using Newtonsoft.Json;

namespace WeatherLogApp
{
    class Program
    {
        static void Main(string[] args)
        {
            WeatherLog weatherLog = LoadWeatherLog();
            Console.WriteLine("This is Hugo's Weather Loggin Application!");

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
                        Console.WriteLine("Exiting Weather Loggin App...");
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

            Console.Write("Precipitation (mm): ");
            float precipitation = float.Parse(Console.ReadLine());

            WeatherData newData = new WeatherData(date, temperature, precipitation);
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
        public float Precipitation { get; set; }

        public WeatherData(DateTime date, float temperature, float precipitation)
        {
            Date = date;
            Temperature = temperature;
            Precipitation = precipitation;
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
                    Console.WriteLine($"Precipitation: {data.Precipitation}mm");
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
                    Console.WriteLine($"Precipitation: {data.Precipitation}mm");
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
                    Console.WriteLine($"Precipitation: {data.Precipitation}mm");
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
