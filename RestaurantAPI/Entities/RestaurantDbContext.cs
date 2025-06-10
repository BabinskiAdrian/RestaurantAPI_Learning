using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace RestaurantAPI.Entities
{
    public class RestaurantDbContext : DbContext //Dodajemy dziedziczenie po DbContext, wymaga to dodanie biblioteki using Microsoft.EntityFrameworkCore;
    {
        private string _connectionString = "Server= (localdb)\\mssqllocaldb ;" +
            "Database=RestaurantDb;Trusted_Connection=True";

        //Tworzymy szablon
        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Dish> Dishes { get; set; }
        public DbSet<User> Users{ get; set; }
        public DbSet<Role> Roles{ get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)  //Nadpisywanie metody:
        {
            // Restaurant
            modelBuilder.Entity<Restaurant>()
                .Property(r => r.Name)  // wyrażenie lambda, wybieranie pola "Name" którego będą się tyczyć wymagania
                .IsRequired()           // wartość jest wymagana, musi być podana
                .HasMaxLength(25);      // maks długość dla wartości


            // Address
            modelBuilder.Entity<Address>()
                .Property(a => a.City)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Address>()
                .Property(a => a.Street)
                .IsRequired()
                .HasMaxLength(50);


            // Dish
            modelBuilder.Entity<Dish>()
                .Property(d => d.Name)
                .IsRequired();


            // User 
            modelBuilder.Entity<User>()
                .Property(u => u.Email)
                .IsRequired();


            // Role
            modelBuilder.Entity<Role>()
                .Property(r => r.Name)
                .IsRequired();
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
    }
}
