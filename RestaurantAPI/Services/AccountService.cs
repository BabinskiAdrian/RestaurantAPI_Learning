using RestaurantAPI.Entities;
using RestaurantAPI.Models;
using static RestaurantAPI.Services.AccountService;

namespace RestaurantAPI.Services
{
    public class AccountService : IAccountService
    {
        public interface IAccountService
        {
            public void RegisterUser(RegisterUserDto dto);

        }


        private readonly RestaurantDbContext _dbContext;
        public AccountService(RestaurantDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public void RegisterUser(RegisterUserDto dto)
        {
            var newUser = new User()
            {
                Email = dto.Email,
                DateOfBirth = dto.DateOfBirth,
                Nationality = dto.Nationality,
                //PasswordHash
                RoleId = dto.RoleId,
            };
            
            _dbContext.Users.Add(newUser);
            _dbContext.SaveChanges();

        }

    }
}
