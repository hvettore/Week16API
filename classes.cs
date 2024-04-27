public class Details
{
    public double air_temperature { get; set; }
    public double wind_speed { get; set; }
    public double relative_humidity { get; set; }
}

public class WeatherForecast
{
    public ForecastProperties properties { get; set; }
}

public class ForecastProperties
{
    public List<TimeSeries> timeseries { get; set; }
}

public class TimeSeries
{
    public TimeSeriesData data { get; set; }
}

public class TimeSeriesData
{
    public Instant instant { get; set; }
}

public class Instant
{
    public Details details { get; set; }
}

public class WeatherLogEntry
{
    public DateTime Date { get; set; }
    public Details UserMeasurements { get; set; }
    public Details YrMeasurements { get; set; }
}


class WeatherData
{
    public DateTime Date { get; set; }
    public double Temperature { get; set; }
    public double WindSpeed { get; set; }
    public double Humidity { get; set; }
    public Details ApiDataYR { get; set; }

    public WeatherData(DateTime date, double air_temperature, double wind_speed, double relative_humidity, Details apiDataYR)
    {
        Date = date;
        Temperature = air_temperature;
        WindSpeed = wind_speed;
        Humidity = relative_humidity;
        ApiDataYR = apiDataYR;
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
                Console.WriteLine($"Precipitation: {data.WindSpeed}m/s");
                Console.WriteLine($"Humidity: {data.Humidity}%");
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
                Console.WriteLine($"Precipitation: {data.WindSpeed}m/s");
                Console.WriteLine($"Humidity: {data.Humidity}%");
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
                Console.WriteLine($"Precipitation: {data.WindSpeed}m/s");
                Console.WriteLine($"Humidity: {data.Humidity}%");
                Console.WriteLine();
            }
        }
        if (!found)
        {
            Console.WriteLine("No data found for the specified month.");
        }
    }

    public class WeatherLogEntry
    {
        public DateTime Date { get; set; }
        public Details UserMeasurements { get; set; }
        public Details YrMeasurements { get; set; }
    }
}

