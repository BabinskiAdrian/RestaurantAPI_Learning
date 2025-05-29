using RestaurantAPI.Entities;

namespace RestaurantAPI.Models
{
    public class RestaurantDto
    {
        //Bezpośrednio skopiowane z klasy Restaurant
        public string Id{ get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public bool HasDelivery { get; set; }

        //Dane odnośnie adresu (spłaszczanie struktury, poprzez zapisanie tutaj bezpośrenio adresu)
        public string City { get; set; }
        public string Street { get; set; }
        public string PostalCode { get; set; }

        //referencja do klasy DishDto
        public List<DishDto> Dishes { get; set; }
    }
}
