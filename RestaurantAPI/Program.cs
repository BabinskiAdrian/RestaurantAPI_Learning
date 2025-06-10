using Microsoft.AspNetCore.Authentication;
using RestaurantAPI.Entities;
using RestaurantAPI.Services;
using static System.Formats.Asn1.AsnWriter;
using NLog.Web;
using RestaurantAPI.Middleware;
using static RestaurantAPI.Services.AccountService;
using Microsoft.AspNetCore.Identity;

namespace RestaurantAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            PasswordHasher<User> passwordHasher = new PasswordHasher<User>();
            passwordHasher.HashPassword(null, "admin123"); // Przykładowe hasło do haszowania

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

            // Rejestrowanie własnych serwisów i innych zależności do DI
            builder.Services.AddDbContext<RestaurantDbContext>();                       // rejestracja bazy danych

            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());    // rejestracja automappera

            builder.Services.AddScoped<RestaurantSeeder>();                             // rejestracja serwisu (seeder)
            builder.Services.AddScoped<IRestaurantServices, RestaurantServices>();      // rejestracja serwisu
            builder.Services.AddScoped<IDishService, DishService>();                    // rejestracja serwisu
            builder.Services.AddScoped<IAccountService, AccountService>();              // rejestracja serwisu
                                                                                        // 
            builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();  // rejestracja Hashera

            builder.Services.AddScoped<ErrorHandlingMiddleware>();                      // rejestracja middleware
            builder.Services.AddScoped<RequestTimeMiddleware>();                        // rejestracja middleware

            builder.Services.AddSwaggerGen();                                           // rejestracja Swaggera
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
            app.UseMiddleware<ErrorHandlingMiddleware>();   // W build trzeba zarejestrować Scoped
            app.UseMiddleware<RequestTimeMiddleware>();     // W build trzeba zarejestrować Scoped

            #endregion

            app.UseHttpsRedirection();  //Dawniej tak samo, dodaje do potoku aplikacji middleware, który automatycznie przekierowuje wszystkie żądania HTTP na HTTPS.

            app.UseSwagger();   //generuje plik
            app.UseSwaggerUI(c=>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Restaurant API"); // Ścieżka do pliku swagger.json
            });

            app.UseAuthorization();
            app.MapControllers();       //Dawniej app.UseEndpoints(endpoints => endpoints.MapControllers());
            app.Run();

            #endregion
        }
    }
}
