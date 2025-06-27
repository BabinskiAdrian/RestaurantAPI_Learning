using Microsoft.AspNetCore.Authorization;
using RestaurantAPI.Entities;
using System.Diagnostics;
using System.Security.Claims;


namespace RestaurantAPI.Authorization
{
    public class MinimumCreatedRestaurantRequirementHandler : AuthorizationHandler<MinimumCreatedRestaurantRequirement>
    {
        
        private readonly RestaurantDbContext _dbContext;
        public MinimumCreatedRestaurantRequirementHandler(RestaurantDbContext restaurantDbContext)
        {
            _dbContext = restaurantDbContext;
        }
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            MinimumCreatedRestaurantRequirement requirement)
        {
            int minRequirment = requirement.MinimumCreatedRestaurants;
            int userId = int.Parse(context.User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value);

            int howManyCreated = _dbContext.Restaurants
                .Where(r => r.CreatedById == userId)
                .Count();

            if (minRequirment <= howManyCreated)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
