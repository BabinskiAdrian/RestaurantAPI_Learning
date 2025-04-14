namespace RestaurantAPI.Entities
{
    public class Address
    {
        public int Id { get; set; }
    
        public string City{ get; set; }
        public string Street { get; set; }
        public string PostCode{ get; set; }
    
        //Referencje do baz danych
        public virtual Restaurant Restaurant { get; set; }
    }
}
