namespace RestaurantAPI.Entities
{
    public class Dish
    {
        //Odpowiada jakby za WIERSZY
        public int Id { get; set; } 

        //definiuję ilość, nazwę i typ KOLUMN
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }

        public int RestaurantId { get; set; } //klucz obcy do tabeli z NazwamiRestauracji
        public virtual Restaurant Restaurant {get; set;} //referencja do klasy restaurant
    }
}
