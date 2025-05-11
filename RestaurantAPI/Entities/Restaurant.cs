namespace RestaurantAPI.Entities
{
    public class Restaurant
    {
        //Na jej postawie entity może, utworzyć primary Key w bazie danych
        public int Id { get; set; }  //reprezentacja klucza głównego dla reszty baz danych

        //Dane odnośnie restauracji:
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public bool HasDelivery { get; set; }
        public string ContactEmail { get; set; }
        public string ContactNumber { get; set; }
        
        //Referencje do innych baz danych (Virtualne wartości na później)
        public int AddressID { get; set; } //referencja, klucz obcy do tabeli z adresem
        public virtual Address Address { get; set; }
        public virtual List<Dish> Dishes { get; set; } 
    }
}
