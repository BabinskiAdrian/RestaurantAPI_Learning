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
        public DishDto GetById(int restuarantId, int dishId);
        public IEnumerable<DishDto> GetAll(int restuarantId);
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

        public int Create(int restuarantId, CreateDishDto dto)
        {
            var restaurant = _dbContext.Restaurants.FirstOrDefault(r => r.Id == restuarantId);
            if (restaurant == null)
                throw new NotFoundException("Restaurant not found");

            var dishEntity = _mapper.Map<Dish>(dto);

            dishEntity.RestaurantId = restuarantId; // Ustawienie ID restauracji (klucz publcizny) w encji potrawy

            _dbContext.Dishes.Add(dishEntity);    // Dodawanie encji
            _dbContext.SaveChanges();             // Zapisywanie zmian w bazie danych
            
            return dishEntity.Id;               // Zwracanie ID nowo utworzonej potrawy
        }

        public DishDto GetById(int restuarantId, int dishId)
        {
            var restaurant = _dbContext.Restaurants.FirstOrDefault(r => r.Id == restuarantId);
            if (restaurant == null)
                throw new NotFoundException("Restaurant not found");


            var dish = _dbContext.Dishes.FirstOrDefault(d => d.Id == dishId);
            if( dish is null || dish.RestaurantId != dishId)
                throw new NotFoundException("Dish not found");

            var dishDto =_mapper.Map<DishDto>(dish);

            return dishDto;
        }

        public IEnumerable<DishDto> GetAll(int restuarantId)
        {
            var restaurant = _dbContext.Restaurants
                .Include(r => r.Dishes)
                .FirstOrDefault(r => r.Id == restuarantId);
            if (restaurant == null)
                throw new NotFoundException("Restaurant not found");


            var dishDtos = _mapper.Map<List<DishDto>>(restaurant.Dishes);
            return dishDtos;
        }
    }
}
