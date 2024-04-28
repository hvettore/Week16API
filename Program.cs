using System.Reflection.Metadata;
using Newtonsoft.Json;
using System.Globalization;

class Program
{
    static HttpClient client = new HttpClient();
    static string logFilePath = Constants.LogFilePath;
    static async Task Main(string[] args)
    {
        Console.Clear();
        client.DefaultRequestHeaders.UserAgent.ParseAdd(Constants.UserAgent);

        Console.WriteLine(Constants.cityChoose);
        foreach (var city in CityCoords.cities.Keys)
        {
            Console.WriteLine(city);
        }

        string chosenCity;
        while (true)
        {
            Console.Write(Constants.menuChoice);

            chosenCity = Console.ReadLine()?.ToLowerInvariant() ?? string.Empty;
            chosenCity = char.ToUpperInvariant(chosenCity[0]) + chosenCity[1..];

            if (CityCoords.cities.ContainsKey(chosenCity))
            {
                break;
            }
            else
            {
                Console.WriteLine(Constants.cityInvalid);
            }
        }

        (string Latitude, string Longitude) coordinates = CityCoords.cities[chosenCity];

        Console.Clear();
        while (true)
        {
            Console.WriteLine(Constants.menuOptionOne);
            Console.WriteLine(Constants.menuOptionTwo);
            Console.WriteLine(Constants.menuOptionThree);
            Console.Write(Constants.menuChoice);
            string choice = Console.ReadLine() ?? string.Empty;

            if (choice == Constants.menuChoiceOne)
            {
                await LogWeather(coordinates);
            }
            else if (choice == Constants.menuChoiceTwo)
            {
                ViewLogs();
            }
            else if (choice == Constants.menuChoiceThree)
            {
                break;
            }
            else
            {
                Console.WriteLine(Constants.menuChoiceInvalid);
            }
        }
    }

    static async Task LogWeather((string Latitude, string Longitude) coordinates)
    {
        Console.Clear();
        string date;
        while (true)
        {
            Console.Write(Constants.datePrompt);
            date = Console.ReadLine() ?? string.Empty;
            if (DateTime.TryParseExact(date, Constants.logViewWeeklyParse, CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
            {
                break;
            }
            else
            {
                Console.Clear();
                Console.WriteLine(Constants.invalidDateFormat);
            }
        }

        double temperature;
        while (true)
        {
            Console.Write(Constants.temperaturePrompt);
            if (double.TryParse(Console.ReadLine(), out temperature) && temperature >= -50 && temperature <= 50)
            {
                break;
            }
            else
            {
                Console.Clear();
                Console.WriteLine(Constants.invalidTemperature);
            }
        }

        double windSpeed;
        while (true)
        {
            Console.Write(Constants.windSpeedPrompt);
            if (double.TryParse(Console.ReadLine(), out windSpeed) && windSpeed >= 0 && windSpeed <= 100)
            {
                break;
            }
            else
            {
                Console.Clear();
                Console.WriteLine(Constants.invalidWindSpeed);
            }
        }

        double humidity;
        while (true)
        {
            Console.Write(Constants.humidityPrompt);
            if (double.TryParse(Console.ReadLine(), out humidity) && humidity >= 0 && humidity <= 100)
            {
                break;
            }
            else
            {
                Console.Clear();
                Console.WriteLine(Constants.invalidHumidity);
            }
        }

        Details userWeatherDetails = new Details
        {
            air_temperature = temperature,
            wind_speed = windSpeed,
            relative_humidity = humidity
        };

        Details apiWeatherDetails = await FetchWeatherData(date, coordinates);
        if (apiWeatherDetails != null)
        {
            Console.Clear();
            Console.Write(string.Format(Constants.logEntryUserFormat, date, userWeatherDetails.air_temperature, userWeatherDetails.wind_speed, userWeatherDetails.relative_humidity));
            Console.Write(string.Format(Constants.logEntryAPIFormat, date, apiWeatherDetails.air_temperature, apiWeatherDetails.wind_speed, apiWeatherDetails.relative_humidity));

            bool isTemperatureMatch = Math.Abs(userWeatherDetails.air_temperature - apiWeatherDetails.air_temperature) < 0.01;
            bool isWindSpeedMatch = Math.Abs(userWeatherDetails.wind_speed - apiWeatherDetails.wind_speed) < 0.01;
            bool isHumidityMatch = Math.Abs(userWeatherDetails.relative_humidity - apiWeatherDetails.relative_humidity) < 0.01;

            Console.WriteLine(Constants.doesTemperatureMatch + (isTemperatureMatch ? Constants.matchingInfoYes : Constants.matchingInfoNo));
            Console.WriteLine(Constants.doesWindSpeedMatch + (isWindSpeedMatch ? Constants.matchingInfoYes : Constants.matchingInfoNo));
            Console.WriteLine(Constants.doesHumidityMatch + (isHumidityMatch ? Constants.matchingInfoYes : Constants.matchingInfoNo));
        }
        else
        {
            Console.WriteLine(Constants.emptyAPIResponse);
        }
        var weatherLogs = new Dictionary<string, Details>();
        if (File.Exists(logFilePath))
        {
            weatherLogs = JsonConvert.DeserializeObject<Dictionary<string, Details>>(File.ReadAllText(logFilePath));
        }
        if (weatherLogs != null)
        {
            weatherLogs[date] = userWeatherDetails;
            File.WriteAllText(logFilePath, JsonConvert.SerializeObject(weatherLogs));
        }
        Console.WriteLine(Constants.pressEnterToReturn);
        while (Console.ReadKey(true).Key != ConsoleKey.Enter) { }
        Console.Clear();
    }

    static void ViewLogs()
    {
        Console.Clear();
        Console.WriteLine(Constants.logViewOptions);
        string choice = Console.ReadLine() ?? "";
        if (choice == "1")
        {
            Console.Clear();
            Console.Write("Enter the date (yyyy-mm-dd) of the log you want to view: ");
            string date = Console.ReadLine();
            DisplayLogsForDate(date);
        }
        else if (choice == "2")
        {
            Console.Clear();
            Console.Write("Enter the start date (yyyy-mm-dd) of the week you want to view: ");
            string startDate = Console.ReadLine();
            for (int i = 0; i < 7; i++)
            {
                string date = DateTime.Parse(startDate).AddDays(i).ToString("yyyy-MM-dd");
                DisplayLogsForDate(date);
            }
        }
        else if (choice == "3")
        {
            Console.Clear();
            Console.Write("Enter the month and year (yyyy-mm) you want to view: ");
            string monthYear = Console.ReadLine();
            int daysInMonth = DateTime.DaysInMonth(int.Parse(monthYear.Split('-')[0]), int.Parse(monthYear.Split('-')[1]));
            for (int i = 1; i <= daysInMonth; i++)
            {
                string date = $"{monthYear}-{i:D2}";
                DisplayLogsForDate(date);
            }
        }
        else
        {
            Console.WriteLine("Invalid choice. Please enter 1, 2, or 3.");
        }
        Console.WriteLine(Constants.pressEnterToReturn);
        while (Console.ReadKey(true).Key != ConsoleKey.Enter) { }
        Console.Clear();
    }

    static void DisplayLogsForDate(string date)
    {
        if (File.Exists(logFilePath))
        {
            var weatherLogs = JsonConvert.DeserializeObject<Dictionary<string, Details>>(File.ReadAllText(logFilePath));
            if (weatherLogs.ContainsKey(date))
            {
                Details weatherDetails = weatherLogs[date];
                if (weatherDetails != null)
                {
                    Console.WriteLine($"Date: {date}");
                    Console.WriteLine($"Temperature: {weatherDetails.air_temperature} °C");
                    Console.WriteLine($"Wind speed: {weatherDetails.wind_speed} m/s");
                    Console.WriteLine($"Humidity: {weatherDetails.relative_humidity} %");
                    Console.WriteLine();
                }
            }
        }
        else
        {
            Console.WriteLine("No logs found.");
        }
    }

    public async static Task<Details> FetchWeatherData(string date, (string Latitude, string Longitude) coordinates)
    {
        string url = string.Format(Constants.WeatherApiUrl, coordinates.Latitude, coordinates.Longitude);
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
            Console.WriteLine(Constants.fetchWeatherDataException);
            Console.Write(string.Format(Constants.fetchWeatherDataErrorMessage, e.Message));
            return null;
        }
    }
}