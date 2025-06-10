using RestaurantAPI.Entities;
using RestaurantAPI.Models;
using Microsoft.AspNetCore.Identity;

namespace RestaurantAPI.Services
{
    public interface IAccountService
    {
        public void RegisterUser(RegisterUserDto dto);

    }

    public class AccountService : IAccountService
    {
        private readonly RestaurantDbContext _dbContext;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AccountService(RestaurantDbContext dbContext, IPasswordHasher<User> passwordHasher)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
        }


        public void RegisterUser(RegisterUserDto dto)
        {
            var hashedPassword = _passwordHasher.HashPassword(new User(), dto.Password); // Hashowanie hasła

            var newUser = new User()
            {
                Email = dto.Email,
                DateOfBirth = dto.DateOfBirth,
                Nationality = dto.Nationality,
                PasswordHash = hashedPassword,
                RoleId = dto.RoleId
            };

            _dbContext.Users.Add(newUser);
            _dbContext.SaveChanges();
        }

    }
}
