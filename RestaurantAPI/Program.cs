using Microsoft.AspNetCore.Authentication;
using RestaurantAPI.Entities;
using RestaurantAPI.Services;
using static System.Formats.Asn1.AsnWriter;
using NLog.Web;
using RestaurantAPI.Middleware;

namespace RestaurantAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {

            #region  Utworenie web hosta
            // Włanse, utworzenie web hosta
            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseNLog(); // NLog: Setup NLog for Dependency injection
            #endregion


            #region Rejestrowanie kontkestu, zależności i innych usług
            // Dawniej w .NET5 znajdowało się w klasie Startup.cs, ConfigureServices

            builder.Services.AddControllers();          //Dodanie kontrolerów do DI
            builder.Services.AddEndpointsApiExplorer(); //Dodanie eksploratora punktów końcowych
            builder.Services.AddControllers();          // Dodanie kontrolerów do DI
            //builder.Services.AddSwaggerGen(); /       /Dodanie Swaggera do DI

            // Rejestrowanie własnych serwisów i innych zależności do DI
            builder.Services.AddDbContext<RestaurantDbContext>();                       // rejestracja bazy danych, nDodanie kontekstu do DI
            builder.Services.AddScoped<RestaurantSeeder>();                             // rejestracja serwisu , aby można było go używać w DI
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());    // rejestracja automappera, aby można było go używać w DI
            builder.Services.AddScoped<IRestaurantServices, RestaurantServices>();      // rejestracja serwisu, aby można było go używać w DI
            builder.Services.AddScoped<ErrorHandlingMiddleware>();                      // rejestracja middleware, aby można było go używać w DI
            #endregion


            // budowanie aplikacji
            var app = builder.Build();

            #region Configurowanie HTTP request pipeline
            // Dawniej w .NET5 metoda Configure
            // Własny Seeder
            var scope = app.Services.CreateScope();
            var seeder = scope.ServiceProvider.GetRequiredService<RestaurantSeeder>();
            seeder.Seed();

            #region middleware
            app.UseMiddleware<ErrorHandlingMiddleware>();

            #endregion

            app.UseHttpsRedirection();  //Dawniej tak samo, dodaje do potoku aplikacji middleware, który automatycznie przekierowuje wszystkie żądania HTTP na HTTPS.
            //Dawniej app.UseRouting(); //ale teraz jeszcze tego nie mam nie wiem czemu
            app.UseAuthorization();
            app.MapControllers();       //Dawniej app.UseEndpoints(endpoints => endpoints.MapControllers());
            app.Run();

            #endregion
        }
    }
}
