using Microsoft.AspNetCore.Authentication;
using RestaurantAPI.Entities;
using static System.Formats.Asn1.AsnWriter;

namespace RestaurantAPI
{
    

    public class Program
    {
        public static void Main(string[] args)
        {

            #region  Utworenie web hosta
            // Włanse, utworzenie web hosta
            var builder = WebApplication.CreateBuilder(args);
            #endregion


            #region w .NET5 configure services, rejestrowanie kontkestu, rejestrowanie zależności kontenera, dependecy injection
            // Domyślne 3
            builder.Services.AddControllers(); //Dodanie kontrolerów do DI
            builder.Services.AddEndpointsApiExplorer(); //Dodanie eksploratora punktów końcowych
                                                        //builder.Services.AddSwaggerGen(); //Dodanie Swaggera do DI

            // Własne, dodanie czegoś, naszego serwisu? nie wiem
            builder.Services.AddTransient<IWeatherForecastService, WeatherForecastService>();
            builder.Services.AddControllers(); //Dodanie kontrolerów do DI
            builder.Services.AddDbContext<RestaurantDbContext>(); // rejestracja bazy danych, nDodanie kontekstu do DI
            builder.Services.AddScoped<RestaurantSeeder>(); // rejestracja serwisu , aby można było go używać w DI

            #endregion


            // budowanie aplikacji
            var app = builder.Build();

            #region middleware //dawniej dla .NET5 w configure
            // Własne Seeder
            var scope = app.Services.CreateScope();
            var seeder = scope.ServiceProvider.GetRequiredService<RestaurantSeeder>();
            seeder.Seed();

            // Domyślne 4 metody
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();

            #endregion
        }
    }
}
