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
    }

    public class DishService : IDishService
    {
        public readonly RestaurantDbContext _context;
        public readonly IMapper _mapper;

        public DishService(RestaurantDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public int Create(int restuarantId, CreateDishDto dto)
        {
            var restaurant = _context.Restaurants.FirstOrDefault(r => r.Id == restuarantId);
            if (restaurant == null)
                throw new NotFoundException("Restaurant not found");

            var dishEntity = _mapper.Map<Dish>(dto);

            dishEntity.RestaurantId = restuarantId; // Ustawienie ID restauracji w encji potrawy

            _context.Dishes.Add(dishEntity);    // Dodawanie encji
            _context.SaveChanges();             // Zapisywanie zmian w bazie danych
            
            return dishEntity.Id;               // Zwracanie ID nowo utworzonej potrawy
        }

    }
}
