using Microsoft.AspNetCore.Authentication;
using RestaurantAPI.Entities;
using RestaurantAPI.Services;
using NLog.Web;
using RestaurantAPI.Middleware;
using Microsoft.AspNetCore.Identity;
using RestaurantAPI.Models;
using FluentValidation;
using RestaurantAPI.Models.Validators;
using FluentValidation.AspNetCore;
using Microsoft.IdentityModel.Tokens;

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
            
            // Ustawianie tokena JWT
            var authenticationSettings = new AuthenticationSettings();
            builder.Configuration
                .GetSection("Authentication")
                .Bind(authenticationSettings); // Pobranie ustawień autoryzacji z pliku konfiguracyjnego
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Bearer";
                options.DefaultScheme = "Bearer";
                options.DefaultChallengeScheme = "Bearer";
            }).AddJwtBearer(cfg =>
            {
                cfg.RequireHttpsMetadata = false;   // Wymuszenie HTTPS, w produkcji powinno być true
                cfg.SaveToken = true;               // Zapis tokena w odpowiedzi, aby można było go użyć w przyszłych żądaniach
                cfg.TokenValidationParameters = new TokenValidationParameters   //Tworzenie paremetrów validacji
                {
                    ValidIssuer = authenticationSettings.JwtIssuer, // Wydawca tokena,
                    ValidAudience = authenticationSettings.JwtKey,  // Odbiorca tokena (jakie podmioty mogą używać tego tokenu
                    // Tworzenie klucza prywatnego,  new SymmetricSecurityKey()
                    // Na podstawie wcześniej podanej wartości "JwtKey" zapisanej w pliku appsetings.json
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(authenticationSettings.JwtKey)),
                };
            });


            builder.Services.AddControllers();          //Dodanie kontrolerów do DI
            builder.Services.AddEndpointsApiExplorer(); //Dodanie eksploratora punktów końcowych
            builder.Services.AddControllers();          // Dodanie kontrolerów do DI
            builder.Services.AddFluentValidation(); // Rejestracja FluentValidation

            // Rejestrowanie własnych serwisów i innych zależności do DI
            builder.Services.AddDbContext<RestaurantDbContext>();                       // rejestracja bazy danych

            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());    // rejestracja automappera

            builder.Services.AddScoped<RestaurantSeeder>();                             // rejestracja serwisu (seeder)
            builder.Services.AddScoped<IRestaurantServices, RestaurantServices>();      // rejestracja serwisu
            builder.Services.AddScoped<IDishService, DishService>();                    // rejestracja serwisu
            builder.Services.AddScoped<IAccountService, AccountService>();              // rejestracja serwisu
                                                                                        // 
            builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();  // rejestracja Hashera

            builder.Services.AddScoped<IValidator<RegisterUserDto>, RegiserUserDtoValidator>(); // rejestracja validatora

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
            app.UseAuthentication();    // Dodanie sprawdzania autentykacji zapytania http
            app.UseHttpsRedirection();  // Dodanie middleware, który automatycznie przekierowuje wszystkie żądania HTTP na HTTPS.
            
            app.UseSwagger();           //generuje plik dla swaggera
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
