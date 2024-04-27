public class Details
{
    public double air_temperature { get; set; }
    public double wind_speed { get; set; }
    public double relative_humidity { get; set; }
}

public class Instant
{
    public Details details { get; set; }
}

public class Data
{
    public Instant instant { get; set; }
}

public class TimeSery
{
    public DateTime time { get; set; }
    public Data data { get; set; }
}

public class Properties
{
    public List<TimeSery> timeseries { get; set; }
}

public class WeatherForecast
{
    public Properties properties { get; set; }
}