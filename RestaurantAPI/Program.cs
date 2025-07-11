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
            var builder = WebApplication.CreateBuilder(args);
            builder.Host.UseNLog();
            #endregion


            #region Dawniej w .NET5 klasa Startup.cs i ConfigureServices

            #region Ustawianie Autentykacji (nie autoryzacji)
            var authenticationSettings = new AuthenticationSettings();
            builder.Configuration.GetSection("Authentication").Bind(authenticationSettings);    // Pobranie ustawień autoryzacji z pliku konfiguracyjnego
            builder.Services.AddSingleton(authenticationSettings);                              // Rejestracja jako singleton, aby był dostępny w całej aplikacji

            // Rejestracja schematu autentykacji (dalej tokena JWT, wraz z konfiguracją)
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Bearer";
                options.DefaultScheme = "Bearer";
                options.DefaultChallengeScheme = "Bearer";
            })
            .AddJwtBearer(cfg =>
            {
                // Tworzenie tokena JWT
                cfg.RequireHttpsMetadata = false;   // Wymuszenie HTTPS, w produkcji powinno być true
                cfg.SaveToken = true;               // Zapis tokena w odpowiedzi, aby można było go użyć w przyszłych żądaniach

                //Tworzenie paremetrów validacji
                cfg.TokenValidationParameters = new TokenValidationParameters   
                {
                    ValidIssuer = authenticationSettings.JwtIssuer,     // Wydawca tokena,
                    ValidAudience = authenticationSettings.JwtIssuer,   // Odbiorca tokena (jakie podmioty mogą używać tego tokenu

                    // IssuerSigningKey                 <-Zapis klucza to zmiennej 
                    // new SymmetricSecurityKey()       <-Tworzenie klucza prywatnego
                    // authenticationSettings.JwtKey    <- Odwołanie się do wartości z "JwtKey" zapisanej w pliku appsetings.json
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.JwtKey)),
                };
            });
            #endregion //koniec autentykacji


            #region Autoryzacja (nie mylić z autentykacją)
            builder.Services.AddAuthorization(options =>
            {
                // arguemtyn polityki ([nazwa], [warunki do spełnenia])
                options.AddPolicy("HasNationality", builder => builder.RequireClaim("Nationality", "German", "Polish", "PolakRodak"));
                options.AddPolicy("AtLeast20Age", builder => builder.AddRequirements(new MinimumAgeRequirement(20)));
                options.AddPolicy("AtLeast2CreatedRestaurant", builder => builder.AddRequirements(new MinimumCreatedRestaurantRequirement(2)));
            });

            builder.Services.AddScoped<IAuthorizationHandler, MinimumAgeRequirementHandler>();
            builder.Services.AddScoped<IAuthorizationHandler, ResourceOperationRequirementHandler>();
            builder.Services.AddScoped<IAuthorizationHandler, MinimumCreatedRestaurantRequirementHandler>();
            #endregion //koniec regionu autoryzacji


            #region Rejestrowanie kontkestu, zależności i innych usług
            // Dawniej w .NET5 metoda ConfigureServices

            // rózne
            builder.Services.AddControllers();                                          // Dodanie kontrolerów do DI
            builder.Services.AddEndpointsApiExplorer();                                 // Dodanie eksploratora punktów końcowych
            builder.Services.AddFluentValidationAutoValidation();                       // Rejestracja FluentValidation

            builder.Services.AddDbContext<RestaurantDbContext>();                       // rejestracja bazy danych
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());    // rejestracja automappera
            builder.Services.AddScoped<RestaurantSeeder>();                             // rejestracja serwisu (seeder)
            builder.Services.AddScoped<IDataGenerator, DataGenerator>();                // Generator danych własny

            builder.Services.AddScoped<IUserContextService, UserContextService>();      // rejestracja serwisu kontekstu użytkownika
            builder.Services.AddHttpContextAccessor();                                  // rejestracja HttpContextAccessor, aby móc używać IUserContextService
            builder.Services.AddSwaggerGen();                                           // rejestracja Swaggera

            builder.Services.AddCors(option =>
            {
                option.AddPolicy("FrontEndCilent", policyBuilder =>
                    policyBuilder.AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithOrigins(builder.Configuration["AllowedOrgins"])
                    );

            });

            // rejestracja serwisu
            builder.Services.AddScoped<IRestaurantServices, RestaurantServices>();
            builder.Services.AddScoped<IDishService, DishService>();
            builder.Services.AddScoped<IAccountService, AccountService>();
            

            // rejestracja Hashera
            builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();  


            // rejestracja validatory
            builder.Services.AddScoped<IValidator<RegisterUserDto>, RegiserUserDtoValidator>();
            builder.Services.AddScoped<IValidator<CreateRestaurantDto>, CreateRestaurantDtoValidator>();
            builder.Services.AddScoped<IValidator<RestaurantQuery>, RestaurantQueryValidatior>();


            // rejestracja middleware
            builder.Services.AddScoped<ErrorHandlingMiddleware>();
            builder.Services.AddScoped<RequestTimeMiddleware>();

            #endregion // koniec rejestrowania usług
            #endregion //koniec dawnej klasy Startup.cs


            #region Configurowanie HTTP request pipeline
            // "budowanie aplikacji - ustalanie kolejnosci przepływu zapytań (pipe line). Kolejność ma znaczenie (powyżej kolejność rejestrowania nie ma znaczenia)
            var app = builder.Build();


            app.UseResponseCaching();                       // Przed UseStaticFiles(), by w cache możliwe byłoby zapisywanie plików statycznych
            app.UseStaticFiles();

            // Własny Seeder
            var scope = app.Services.CreateScope();
            var seeder = scope.ServiceProvider.GetRequiredService<RestaurantSeeder>();
            seeder.Seed();


            #region middleware
            app.UseMiddleware<ErrorHandlingMiddleware>();   // W build trzeba zarejestrować Scoped
            app.UseMiddleware<RequestTimeMiddleware>();     // W build trzeba zarejestrować Scoped
            #endregion // koniec middleware


            app.UseAuthentication();                        // Dodanie sprawdzania autentykacji zapytania http

            app.UseHttpsRedirection();                      // Dodanie middleware, który automatycznie przekierowuje wszystkie żądania HTTP na HTTPS.
            
            app.UseSwagger();                               //generuje plik dla swaggera\
            app.UseSwaggerUI(c=>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Restaurant API"); // Ścieżka do pliku swagger.json
            });

            
            app.UseRouting();

            app.UseCors("FrontEndCilent");                  // Dodanie konkretnej polityki CORS 

            app.UseAuthorization();     

            app.MapControllers();                           //Dawniej app.UseEndpoints(endpoints => endpoints.MapControllers());

            app.Run();                                      //Odpalanie aplikacji
            #endregion // koniec pipe line
        }
    }
}
