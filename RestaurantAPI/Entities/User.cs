namespace RestaurantAPI.Entities
{
    public class User
    {
        // primary key
        public int Id { get; set; }

        // dane o użytkowniku
        public string Email { get; set; }
        public string FirstName { get; set; } 
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }  // może być null
        public string Nationality { get; set; }
        public string PasswordHash { get; set; }    // hash hasła


        // referencja do tabeli z rolami
        public int RoleId { get; set; }
        public virtual Role Role { get; set; }
    }
}
