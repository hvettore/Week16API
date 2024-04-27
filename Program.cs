using Newtonsoft.Json;

class Program
{
    static HttpClient client = new HttpClient();
    static string logFilePath = "weatherLogs.json";

    static async Task Main(string[] args)
    {
        client.DefaultRequestHeaders.UserAgent.ParseAdd("Hugo's WeatherLogApp");

        Dictionary<string, (string Latitude, string Longitude)> cities = new Dictionary<string, (string Latitude, string Longitude)>
        {
            {"Oslo", ("59.9139", "10.7522")},
            {"Grimstad", ("58.3444", "8.5897")},
            {"Kristiansand", ("58.1464", "7.9950")},
            {"New York City", ("40.7128", "-74.0060")},
            {"Tokyo", ("35.6895", "139.6917")},
            {"London Kingdom", ("51.5074", "0.1278")},
            {"Sydney", ("-33.8688", "151.2093")},
            {"Cairo", ("30.0444", "31.2357")},
            {"Rio de Janeiro", ("-22.9068", "-43.1729")},
            {"Moscow", ("55.7558", "37.6176")}
        };

        Console.WriteLine("Choose a city from the list:");
        foreach (var city in cities.Keys)
        {
            Console.WriteLine(city);
        }

        string chosenCity;
        while (true)
        {
            Console.Write("Enter your choice: ");
            chosenCity = Console.ReadLine();
            if (cities.ContainsKey(chosenCity))
            {
                break;
            }
            else
            {
                Console.WriteLine("Invalid choice. Please enter a city from the list.");
            }
        }

        (string Latitude, string Longitude) coordinates = cities[chosenCity];

        while (true)
        {
            Console.WriteLine("1. Log weather");
            Console.WriteLine("2. View logs");
            Console.WriteLine("3. Exit");
            Console.Write("Enter your choice: ");
            string choice = Console.ReadLine();

            if (choice == "1")
            {
                await LogWeather(coordinates);
            }
            else if (choice == "2")
            {
                ViewLogs();
            }
            else if (choice == "3")
            {
                break;
            }
            else
            {
                Console.WriteLine("Invalid choice. Please enter 1, 2, or 3.");
            }
        }
    }

    static async Task LogWeather((string Latitude, string Longitude) coordinates)
    {
        Console.Write("Enter the date (yyyy-mm-dd): ");
        string date = Console.ReadLine();
        Console.Write("Enter the temperature: ");
        double temperature = double.Parse(Console.ReadLine());
        Console.Write("Enter the wind speed: ");
        double windSpeed = double.Parse(Console.ReadLine());
        Console.Write("Enter the humidity: ");
        double humidity = double.Parse(Console.ReadLine());

        Details userWeatherDetails = new Details
        {
            air_temperature = temperature,
            wind_speed = windSpeed,
            relative_humidity = humidity
        };

        Details apiWeatherDetails = await FetchWeatherData(date, coordinates);
        if (apiWeatherDetails != null)
        {
            Console.WriteLine($"API data for {date}:");
            Console.WriteLine($"Temperature: {apiWeatherDetails.air_temperature}");
            Console.WriteLine($"Wind speed: {apiWeatherDetails.wind_speed}");
            Console.WriteLine($"Humidity: {apiWeatherDetails.relative_humidity}");
        }
        else
        {
            Console.WriteLine("No API data found for the specified date.");
        }

        // Save the user's weather data to a JSON file
        var weatherLogs = new Dictionary<string, Details>();
        if (File.Exists(logFilePath))
        {
            weatherLogs = JsonConvert.DeserializeObject<Dictionary<string, Details>>(File.ReadAllText(logFilePath));
        }
        weatherLogs[date] = userWeatherDetails;
        File.WriteAllText(logFilePath, JsonConvert.SerializeObject(weatherLogs));
    }

    static void ViewLogs()
    {
        Console.Write("Enter the date (yyyy-mm-dd) of the log you want to view: ");
        string date = Console.ReadLine();

        if (File.Exists(logFilePath))
        {
            var weatherLogs = JsonConvert.DeserializeObject<Dictionary<string, Details>>(File.ReadAllText(logFilePath));
            if (weatherLogs.ContainsKey(date))
            {
                Details weatherDetails = weatherLogs[date];
                Console.WriteLine($"Temperature: {weatherDetails.air_temperature}");
                Console.WriteLine($"Wind speed: {weatherDetails.wind_speed}");
                Console.WriteLine($"Humidity: {weatherDetails.relative_humidity}");
            }
            else
            {
                Console.WriteLine("No log found for the specified date.");
            }
        }
        else
        {
            Console.WriteLine("No logs found.");
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

