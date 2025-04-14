namespace RestaurantAPI
{
    public interface IWeatherForecastService
    {
        IEnumerable<WeatherForecast> Get();

        IEnumerable<WeatherForecast> GetZadaniePraktyczne(int numberOfElements, int minTemperature, int maxTemperature);
    }
}
