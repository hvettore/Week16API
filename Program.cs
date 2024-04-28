using System.Reflection.Metadata;
using Newtonsoft.Json;

class Program
{
    static HttpClient client = new HttpClient();
    static string logFilePath = Constants.LogFilePath;

    static async Task Main(string[] args)
    {
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
            chosenCity = Console.ReadLine();
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

        while (true)
        {
            Console.WriteLine(Constants.menuOptionOne);
            Console.WriteLine(Constants.menuOptionTwo);
            Console.WriteLine(Constants.menuOptionThree);
            Console.Write(Constants.menuChoice);
            string choice = Console.ReadLine();

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
        Console.Write(Constants.datePrompt);
        string date = Console.ReadLine();
        Console.Write(Constants.temperaturePrompt);
        double temperature = double.Parse(Console.ReadLine());
        Console.Write(Constants.windSpeedPrompt);
        double windSpeed = double.Parse(Console.ReadLine());
        Console.Write(Constants.humidityPrompt);
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
            Console.Write(string.Format(Constants.logEntryFormat, date, apiWeatherDetails.air_temperature, apiWeatherDetails.wind_speed, apiWeatherDetails.relative_humidity));
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
        weatherLogs[date] = userWeatherDetails;
        File.WriteAllText(logFilePath, JsonConvert.SerializeObject(weatherLogs));
    }

    static void ViewLogs()
    {
        Console.WriteLine(Constants.logViewOptions);
        Console.Write(Constants.menuChoice);
        string choice = Console.ReadLine();

        if (choice == Constants.menuChoiceOne)
        {
            Console.Write(Constants.logViewDateEnter);
            string date = Console.ReadLine();
            DisplayLogsForDate(date);
        }
        else if (choice == Constants.menuChoiceTwo)
        {
            Console.Write(Constants.logViewDateEnter);
            string startDate = Console.ReadLine();
            for (int i = 0; i < 7; i++)
            {
                string date = DateTime.Parse(startDate).AddDays(i).ToString(Constants.logViewWeeklyParse);
                DisplayLogsForDate(date);
            }
        }
        else if (choice == Constants.menuChoiceThree)
        {
            Console.Write(Constants.logViewMonthYearEnter);
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
            Console.WriteLine(Constants.menuChoiceInvalid);
        }
    }

    static void DisplayLogsForDate(string date)
    {
        if (File.Exists(logFilePath))
        {
            var weatherLogs = JsonConvert.DeserializeObject<Dictionary<string, Details>>(File.ReadAllText(logFilePath));
            if (weatherLogs.ContainsKey(date))
            {
                Details weatherDetails = weatherLogs[date];
                Console.Write(string.Format(Constants.logEntryFormat, date, weatherDetails.air_temperature, weatherDetails.wind_speed, weatherDetails.relative_humidity));
            }

            else
            {
                Console.Write(string.Format(Constants.logViewNoLogsDate, date));
            }
        }
        else
        {
            Console.WriteLine(Constants.logViewNoLogs);
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

