public static class Constants
{
    public const string LogFilePath = "weatherLogs.json";
    public const string UserAgent = "Hugo's WeatherLogApp";
    public const string WeatherApiUrl = "https://api.met.no/weatherapi/locationforecast/2.0/compact?lat={0}&lon={1}";

    public const string cityChoose = "Choose a city from the list:";
    public const string cityInvalid = "Invalid choice. Please enter a city from the list.";

    public const string menuOptionOne = "1. Log weather";
    public const string menuOptionTwo = "2. View logs";
    public const string menuOptionThree = "3. Exit";
    public const string menuChoice = "Enter your choice: ";
    public const string menuChoiceInvalid = "Invalid choice. Please enter 1, 2, or 3.";


    public const string menuChoiceOne = "1";
    public const string menuChoiceTwo = "2";
    public const string menuChoiceThree = "3";

    public const string datePrompt = "Enter the date (yyyy-mm-dd): ";
    public const string temperaturePrompt = "Enter the temperature: ";
    public const string windSpeedPrompt = "Enter the wind speed: ";
    public const string humidityPrompt = "Enter the humidity: ";


    public const string logEntryFormat = "API data for: {0}. Temperature: {1}, Wind Speed: {2}, Humidity: {3}";
    public const string emptyAPIResponse = "No API data available for this date.";

    public const string logViewOptions = "1. View logs for a specific date\n2. View logs for a week\n3. View logs for a month\n";

    public const string logViewDateEnter = "Enter the date (yyyy-mm-dd) of the log you want to view: ";
    public const string logViewWeeklyParse = "yyyy-MM-dd";
    public const string logViewMonthYearEnter = "Enter the month and year (yyyy-mm) you want to view: ";

    public const string logViewDate = "Date: {0}. Temperature: {1}, Wind Speed: {2}, Humidity: {3}";
    public const string logViewNoLogsDate = "No logs available for {1}.";
    public const string logViewNoLogs = "No logs found.";

    public const string fetchWeatherDataException = "\nException Caught!";
    public const string fetchWeatherDataErrorMessage = "Message :{0} ";

}