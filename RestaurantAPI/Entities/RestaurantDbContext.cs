using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace RestaurantAPI.Entities
{
    public class RestaurantDbContext : DbContext //Dodajemy dziedziczenie po DbContext, wymaga to dodanie biblioteki using Microsoft.EntityFrameworkCore;
    {
        //Tworzymy szablon
        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Dish> Dishes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)  //Nadpisywanie metody:
        {
            modelBuilder.Entity<Restaurant>()
                .Property(r => r.Name) //wyrażenie lambda, wybieranie właściwości Name
                .IsRequired()       //wymagana
                .HasMaxLength(25);  //maks długość dla wartości

            modelBuilder.Entity<Dish>()
                .Property(d => d.Name)
                .IsRequired();


            modelBuilder.Entity<Address>()
                .Property(a => a.City)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Address>()
                .Property(a => a.Street)
                .IsRequired()
                .HasMaxLength(50);
        }

        //Nadpisywanie metody, trzeba dodać kolejnego Nugeta jeżeli chcemy korzystać z bazy danych MySQL
        private string _connectionString =
                   "Server= (localdb)\\mssqllocaldb ; Database=RestaurantDb; Trusted_Connection=True";
                  //"Server=.\\SQLExpress;Database=RestaurantDB;Trusted_Connection=Yes"; //wersja dla SQLExpress

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
    }
}
