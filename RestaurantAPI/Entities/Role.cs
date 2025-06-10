namespace RestaurantAPI.Entities
{
    public class Role
    {
        public int Id { get; set; }
        
        // nazwa roli, np. "Admin", "User", "Manager"
        public string Name { get; set; }  
    }
}
