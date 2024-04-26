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