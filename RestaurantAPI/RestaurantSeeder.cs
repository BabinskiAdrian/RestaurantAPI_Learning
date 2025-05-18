using RestaurantAPI.Entities;
using System.Diagnostics; // dla RestaurantDbContext

namespace RestaurantAPI
{
    public class RestaurantSeeder
    {
        private readonly RestaurantDbContext _dbcontext; //using RestaurantAPI.Entities;
        
        public RestaurantSeeder(RestaurantDbContext dbcontext) //Wstrzykiwanie zależności
        {
            _dbcontext = dbcontext;
        }

        public void Seed()
        {
            if(_dbcontext.Database.CanConnect())
            {
                Debug.WriteLine("!AB-Udało się połączyć");
                if(!_dbcontext.Restaurants.Any())
                {
                    Debug.WriteLine("!AB-Pusta baza danych, odpalam program");
                    var restaurants = GetRestaurants();
                    _dbcontext.Restaurants.AddRange(restaurants);
                    _dbcontext.SaveChanges();
                }
            }

        }

        private IEnumerable<Restaurant> GetRestaurants()
        {
            var restaurants = new List<Restaurant>()
            {
                new Restaurant()
                {
                    Name = "KFC",
                    Category = "Fast Food",
                     Description =
                    "KFC (short for Kentucky Fried Chicken) is an American fast food restaurant chain headquartered in Louisville, Kentucky, that specializes in fried chicken.",
                    ContactEmail = "contact@kfc.com",
                    HasDelivery = true,
                    Dishes =
                    [
                        new ()
                        {
                            Name = "Nashville Hot Chicken",
                            Description = "Nashville Hot Chicken (10 pcs.)",
                            Price = 10.30M,
                        },

                        new ()
                        {
                            Name = "Chicken Nuggets",
                            Description = "Chicken Nuggets (5 pcs.)",
                            Price = 5.30M,
                        },
                    ],
                    Address = new ()
                    {
                        City = "London",
                        Street = "Cork St 5",
                        PostalCode = "WC2N 5DU"
                    }
                },

                new Restaurant ()
                {
                    Name = "McDonald",
                    Category = "Fast Food",
                    Description =
                        "McDonald's Corporation (McDonald's), incorporated on December 21, 1964, operates and franchises McDonald's restaurants.",
                    ContactEmail = "contact@mcdonald.com",
                    HasDelivery = true,
                    Address = new Address()
                    {
                        City = "London",
                        Street = "Boots 193",
                        PostalCode = "W1F 8SR"
                    }
                }
            };
            
            return restaurants;
        }
    }
}
