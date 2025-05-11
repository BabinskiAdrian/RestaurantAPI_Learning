using Microsoft.AspNetCore.Authentication;

namespace RestaurantAPI
{
    

    public class Program
    {
        public static void Main(string[] args)
        {
            // Utworenie web hosta - kiedyś w program.cs
            var builder = WebApplication.CreateBuilder(args);

            //ConfigureServices - kiedyś w startup.cs

            // Dependence Injection
            // Add services to the container.

            builder.Services.AddControllers(); //Domyślne

            builder.Services.AddTransient<IWeatherForecastService, WeatherForecastService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            // Middleware
            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
