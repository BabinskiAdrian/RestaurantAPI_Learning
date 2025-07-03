using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Entities;
using RestaurantAPI.Models;

namespace RestaurantAPI
{
    public interface IDataGenerator
    {
        public void Seed(int amountOfNewRestaurants);
    }

    public class DataGenerator : IDataGenerator
    {
        private RestaurantDbContext _context;
        public DataGenerator(RestaurantDbContext context)
        {
            _context = context;
        }

        public void Seed(int amountOfNewRestaurants)
        {
            Randomizer.Seed = new Random(0);

            string locale = "pl"; // Ustawienie lokalizacji na polski
            string generatedPhrase = "Generated_";

            string newUserEmail="";

            var usersGenerator = new Faker<User>(locale)
                //.StrictMode(true)
                .RuleFor(u => u.Email, f => newUserEmail = f.Internet.Email())
                .RuleFor(u => u.FirstName, f => f.Person.FirstName)
                .RuleFor(u => u.LastName, f => f.Person.LastName)
                .RuleFor(u => u.DateOfBirth, f => f.Date.Past(30, DateTime.Now))
                .RuleFor(u => u.Nationality, f => f.Address.Country())
                .RuleFor(u => u.PasswordHash, new PasswordHasher<User>().HashPassword(null, generatedPhrase + "Password")) //Genrowanie hasła i jego haszowanie
                .RuleFor(u => u.RoleId, f => f.Random.Int(2, 3)); //admin lub manager
            var user = usersGenerator.Generate();

            if (null == _context.Users.FirstOrDefault(u => u.Email == newUserEmail))
            {
                //Jeżeli użytkownik nie istnieje, dodajemy go
                _context.Users.Add(user);
                _context.SaveChanges();
            }

            int userId = _context.Users
                .OrderByDescending(u => u.Id)
                .FirstOrDefault().Id;

            for (int i = 0; i < amountOfNewRestaurants; i++)
            {
                var addressGenerator = new Faker<Address>(locale)
                    .RuleFor(a => a.City, f => f.Address.City())
                    .RuleFor(a => a.Street, f => f.Address.StreetName())
                    .RuleFor(a => a.PostalCode, f => f.Address.ZipCode());


                var restaurantGenerator = new Faker<Restaurant>(locale)
                        .RuleFor(r => r.Name, f => {
                            var name = f.Company.CompanyName();
                            return name.Length > 25 ? name.Substring(0, 25) : name;
                        })
                        .RuleFor(r => r.Description, f => "Kfc")//generatedPhrase + f.Random.String2(20, "abcde fghijkl mnopq rstu vwxyz żźćń łąśęó "))
                        .RuleFor(r => r.Category, f => f.Commerce.Categories(1)[0])
                        .RuleFor(r => r.HasDelivery, f => f.Random.Bool())
                        .RuleFor(r => r.ContactEmail, f => f.Internet.Email())
                        .RuleFor(r => r.ContactNumber, f => f.Phone.PhoneNumber("###-###-###"))
                        .RuleFor(r => r.CreatedById, userId)
                        .RuleFor(r => r.Address, addressGenerator.Generate());

                var restaurant = restaurantGenerator.Generate();
                _context.Restaurants.Add(restaurant);
            }

            _context.SaveChanges();
        }
    }
}
