using Microsoft.AspNetCore.Authorization;

namespace RestaurantAPI.Authorization
{
    public class MinimumCreatedRestaurantRequirement : IAuthorizationRequirement
    {
        public int MinimumCreatedRestaurants { get; }

        public MinimumCreatedRestaurantRequirement(int minimumCreatedRestaurants)
        {
            MinimumCreatedRestaurants = minimumCreatedRestaurants;
        }
    }
}