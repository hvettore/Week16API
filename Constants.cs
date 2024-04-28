using System.Runtime.CompilerServices;

public static class Constants
{
    public const string logFilePath = "weatherLogs.json";
    public const string userAgent = "Hugo's WeatherLogApp";
    public const string weatherApiUrl = "https://api.met.no/weatherapi/locationforecast/2.0/compact?lat={0}&lon={1}";
    public const string hifen = "-";

    public const string cityChoose = "Choose a city from the list:\n";
    public const string cityInvalid = "Invalid choice. Please enter a city from the list.";

    public const string menuOptionOne = "1. Log weather";
    public const string menuOptionTwo = "2. View logs";
    public const string menuOptionThree = "3. Exit";
    public const string menuChoice = "\nEnter your choice: ";
    public const string menuChoiceInvalid = "Invalid choice. Please enter 1, 2, or 3.";

    public const string menuChoiceOne = "1";
    public const string menuChoiceTwo = "2";
    public const string menuChoiceThree = "3";

    public const string datePrompt = "Enter the date (yyyy-mm-dd): ";
    public const string temperaturePrompt = "Enter the temperature: ";
    public const string windSpeedPrompt = "Enter the wind speed: ";
    public const string humidityPrompt = "Enter the humidity: ";

    public const string logEntryUserFormat = "User logged data for: {0}. Temperature: {1}, Wind Speed: {2}, Humidity: {3}\n";
    public const string logEntryAPIFormat = "API data for: {0}. Temperature: {1}, Wind Speed: {2}, Humidity: {3}\n";
    public const string emptyAPIResponse = "No API data available for this date.";

    public const string logViewOptions = "1. View logs for a specific date\n2. View logs for a week\n3. View logs for a month\n";
    public const string logViewDateEnter = "Enter the date (yyyy-mm-dd) of the log you want to view: ";
    public const string logViewWeeklyParse = "yyyy-mm-dd";
    public const string logViewWeeklyParseCapitalized = "yyyy-MM-dd";
    public const string logViewMonthlyParse = "yyyy-mm";
    public const string logViewMonthYearEnter = "Enter the month and year (yyyy-mm) you want to view: ";
    public const string logViewDate = "Date: {0}. Temperature: {1}, Wind Speed: {2}, Humidity: {3}";
    public const string logViewNoLogsDate = "No logs available for {1}.";
    public const string logViewNoLogs = "No logs found.";

    public const string fetchWeatherDataException = "\nException Caught!";
    public const string fetchWeatherDataErrorMessage = "Message :{0} ";

    public const string pressEnterToReturn = "\nPress Enter to return to the main menu.";

    public const string invalidDateFormat = "Invalid date format. Please enter in yyyy-mm-dd format.";
    public const string invalidDateFormatMonthly = "Invalid date format. Please enter in yyyy-mm format.";
    public const string invalidTemperature = "Invalid temperature. Please enter a number between -50 and 50.";
    public const string invalidWindSpeed = "Invalid wind speed. Please enter a number between 0 and 100.";
    public const string invalidHumidity = "Invalid humidity. Please enter a number between 0 and 100.";

    public const string doesTemperatureMatch = "Does the temperature match the API data?: ";
    public const string doesWindSpeedMatch = "Does the wind speed match the API data?: ";
    public const string doesHumidityMatch = "Does the humidity match the API data?: ";

    public const string matchingInfoYes = "Yes";
    public const string matchingInfoNo = "No";
}

public static class CityCoords
{
    public static Dictionary<string, (string Latitude, string Longitude)> cities = new Dictionary<string, (string Latitude, string Longitude)>
    {
        {"Oslo", ("59.9139", "10.7522")},
        {"Grimstad", ("58.3444", "8.5897")},
        {"Kristiansand", ("58.1464", "7.9950")},
        {"New York City", ("40.7128", "-74.0060")},
        {"Tokyo", ("35.6895", "139.6917")},
        {"London", ("51.5074", "0.1278")},
        {"Sydney", ("-33.8688", "151.2093")},
        {"Cairo", ("30.0444", "31.2357")},
        {"Moscow", ("55.7558", "37.6176")}
    };
}