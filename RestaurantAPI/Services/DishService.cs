using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Models;
using RestaurantAPI.Entities;
using RestaurantAPI.Exceptions;
using AutoMapper;

namespace RestaurantAPI.Services
{
    public interface IDishService
    {
        int Create(int restaurantId, CreateDishDto dto);
        public DishDto GetById(int restaurantId, int dishId);
        public IEnumerable<DishDto> GetAll(int restaurantId);
        public void DeleteOne(int restaurantId, int dishId);
        public void DeleteAll(int restaurantId);
    }

    public class DishService : IDishService
    {
        public readonly RestaurantDbContext _dbContext;
        public readonly IMapper _mapper;

        public DishService(RestaurantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public int Create(int restaurantId, CreateDishDto dto)
        {
            //var restaurant = GetRestaurantById(restuarantId);
            // Nie wykorzystujemy dalej zmiennej restauracja wiec nie potrzebuejmy jej zapisywać
            GetRestaurantById(restaurantId); 

            var dishEntity = _mapper.Map<Dish>(dto);
            dishEntity.RestaurantId = restaurantId; // Ustawienie id restauracji (klucz publcizny) w encji potrawy

            _dbContext.Dishes.Add(dishEntity);    // Dodawanie encji
            _dbContext.SaveChanges();             // Zapisywanie zmian w bazie danych
            
            return dishEntity.Id;               // Zwracanie ID nowo utworzonej potrawy
        }

        public DishDto GetById(int restaurantId, int dishId)
        {
            GetRestaurantById(restaurantId);
            var dish = GetDishById(restaurantId, dishId);

            var dishDto =_mapper.Map<DishDto>(dish);

            return dishDto;
        }

        public IEnumerable<DishDto> GetAll(int restaurantId)
        {
            var restaurant = GetRestaurantById(restaurantId);


            var dishDtos = _mapper.Map<List<DishDto>>(restaurant.Dishes);
            return dishDtos;
        }


        public void DeleteOne(int restaurantId, int dishId)
        {
            GetRestaurantById(restaurantId);

            var dish = GetDishById(restaurantId, dishId);

            _dbContext.RemoveRange(dish);
            _dbContext.SaveChanges();


            return;
        }

        public void DeleteAll(int restaurantId)
        {
            var restaurant = GetRestaurantById(restaurantId);

            _dbContext.RemoveRange(restaurant.Dishes); // Usuwanie wszystkich potraw z restauracji
            _dbContext.SaveChanges(); 


            return;
        }



        // Metody prywatne, pomocnicze
        private Restaurant GetRestaurantById(int restaurantId)
        {
            var restaurant = _dbContext.Restaurants
                .Include(r => r.Dishes)
                .FirstOrDefault(r => r.Id == restaurantId);
            if (restaurant == null)
                throw new NotFoundException("Restaurant not found");

            return restaurant;
        }

        private Dish GetDishById(int restaurantId, int dishId)
        {
            var dish = _dbContext.Dishes.FirstOrDefault(d => d.Id == dishId);
            if (dish is null || dish.RestaurantId != restaurantId)
                throw new NotFoundException("Dish not found");

            return dish;
        }
    }
}
