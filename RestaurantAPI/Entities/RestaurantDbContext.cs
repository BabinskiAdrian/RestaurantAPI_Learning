using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace RestaurantAPI.Entities
{
    //Dodajemy dziedziczenie po DbContext, wymaga to dodanie biblioteki using Microsoft.EntityFrameworkCore;
    public class RestaurantDbContext : DbContext
    {
        //Tworzymy szablon
        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Dish> Dishes { get; set; }

        
        //Nadpisywanie metody:
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Restaurant>()
                .Property(r => r.Name) //wyrażenie lambda, wybieranie właściwości Name
                .IsRequired()       //wymagana
                .HasMaxLength(25);  //maks długość dla wartości

            modelBuilder.Entity<Dish>()
                .Property(d => d.Name)
                .IsRequired();
        }

        //Nadpisywanie metody
        //Trzeba dodać kolejnego Nugeta jeżeli chcemy korzystać z bazy danych MySQL
        private string _connectionString = "" +
            "Server=(local)\\mssqllocaldb;" +
            "Database=RestaurantDb" +
            "Trusted_Connection=True";

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
    }
}
