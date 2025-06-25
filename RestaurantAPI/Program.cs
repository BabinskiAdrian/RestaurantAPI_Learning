using System.Text;

using RestaurantAPI.Models.Validators;
using RestaurantAPI.Entities;
using RestaurantAPI.Services;
using RestaurantAPI.Models;
using RestaurantAPI.Middleware;
using RestaurantAPI.Authorization;

using FluentValidation;
using FluentValidation.AspNetCore;

using NLog.Web;

using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

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

            #region Ustawianie tokena JWT i autoryzacji
            // Tworzenie obiektu AuthenticationSettings
            var authenticationSettings = new AuthenticationSettings();

            //Pobranie ustawień autoryzacji z pliku konfiguracyjnego
            builder.Configuration.GetSection("Authentication").Bind(authenticationSettings); 

            // Rejestracja jako singleton, aby był dostępny w całej aplikacji
            builder.Services.AddSingleton(authenticationSettings);      

            // Rejestracja schematu autentykacjim, w tym token JWT, wraz z konfiguracją
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
                    ValidIssuer = authenticationSettings.JwtIssuer,// Wydawca tokena,
                    ValidAudience = authenticationSettings.JwtIssuer,// Odbiorca tokena (jakie podmioty mogą używać tego tokenu
                    // Tworzenie klucza prywatnego,  new SymmetricSecurityKey()
                    // Na podstawie wcześniej podanej wartości "JwtKey" zapisanej w pliku appsetings.json
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.JwtKey)),
                };
            });
            #endregion //koniec regionu dal, tokena JWT i autentykacji

            #region autoryzacja (nie mylić z autentykacją)
            builder.Services.AddAuthorization(options =>
            {
                // arguemtyn polityki ([nazwa], [warunki do spełnenia])
                options.AddPolicy("HasNationality", builder => builder.RequireClaim("Nationality", "German", "Polish"));
                options.AddPolicy("Atleast20", builder => builder.AddRequirements(new MinimumAgeRequirement(20)));
            });
            
            builder.Services.AddScoped<IAuthorizationHandler, MinimumAgeRequirementHandler>();
            builder.Services.AddScoped<IAuthorizationHandler, ResourceOperationRequirementHandler>(); /////////////////////
            #endregion //koniec regionu autoryzacji

            builder.Services.AddControllers();          //Dodanie kontrolerów do DI
            builder.Services.AddEndpointsApiExplorer(); //Dodanie eksploratora punktów końcowych
            builder.Services.AddControllers();          // Dodanie kontrolerów do DI
            builder.Services.AddFluentValidation();     // Rejestracja FluentValidation

            // Rejestrowanie własnych serwisów i innych zależności do DI
            builder.Services.AddDbContext<RestaurantDbContext>();                       // rejestracja bazy danych

            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());    // rejestracja automappera

            builder.Services.AddScoped<RestaurantSeeder>();                             // rejestracja serwisu (seeder)
            builder.Services.AddScoped<IRestaurantServices, RestaurantServices>();      // rejestracja serwisu [klasa oraz interfejs]
            builder.Services.AddScoped<IDishService, DishService>();                    // rejestracja serwisu
            builder.Services.AddScoped<IAccountService, AccountService>();              // rejestracja serwisu
                                                                                        // 
            builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();  // rejestracja Hashera

            builder.Services.AddScoped<IValidator<RegisterUserDto>, RegiserUserDtoValidator>();             // rejestracja validatora
            builder.Services.AddScoped<IValidator<CreateRestaurantDto>, CreateRestaurantDtoValidator>();    // rejestracja validatora

            builder.Services.AddScoped<ErrorHandlingMiddleware>();                      // rejestracja middleware
            builder.Services.AddScoped<RequestTimeMiddleware>();                        // rejestracja middleware

            builder.Services.AddScoped<IUserContextService, UserContextService>();      // rejestracja serwisu kontekstu użytkownika
            builder.Services.AddHttpContextAccessor();                                  // rejestracja HttpContextAccessor, aby móc używać IUserContextService
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

            
            app.UseRouting();

            app.UseAuthorization();
           
            app.MapControllers();       //Dawniej app.UseEndpoints(endpoints => endpoints.MapControllers());
            app.Run();

            #endregion
        }
    }
}
