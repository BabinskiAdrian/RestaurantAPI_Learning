﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Entities;
using RestaurantAPI.Exceptions;
using RestaurantAPI.Models;
using System.Reflection.Metadata.Ecma335;

namespace RestaurantAPI.Services
{
    public interface IRestaurantServices
    {
        RestaurantDto GetById(int id);
        IEnumerable<RestaurantDto> GetAll();
        int Create(CreateRestaurantDto dto);
        void Delete(int id);
        void Update(UpdateRestaurantDto dto, int id);
    }

    public class RestaurantServices : IRestaurantServices
    {
        #region Zależności
        // Zmienne zależności
        private readonly RestaurantDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<RestaurantServices> _logger;

        // Wstrzykiwanie zależności przez konstruktor
        public RestaurantServices(RestaurantDbContext dbContext, IMapper mapper, ILogger<RestaurantServices> logger)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
        }
        #endregion

        public void Update(UpdateRestaurantDto dto ,int id)
        {
            var restaurant = _dbContext.Restaurants
                .FirstOrDefault(r => r.Id == id);

            if (restaurant == null)
                throw new NotFoundException($"Restaurant not found");

            restaurant.Name = dto.Name;
            restaurant.Description = dto.Description;
            restaurant.HasDelivery = dto.HasDelivery;

            _dbContext.SaveChanges();
        }


        public void Delete(int id)
        {
            // Logowanie błędu przed usunięciem
            _logger.LogError($"Restaurant with id {id} DELETE action invoked"); 

            //Pobranie restauracji z bazy danych na podstawie id
            var restaurant = _dbContext.Restaurants
                .FirstOrDefault(r => r.Id == id);

            // Sprawdzenie czy restauracja istnieje (czy rejestr jest w bazie danych)
            if (restaurant == null) 
                throw new NotFoundException($"Restaurant not found");

            // jeżeli nie jest nullem to wykonaj to: (usunięcie)
            _dbContext.Restaurants.Remove(restaurant);
            _dbContext.SaveChanges();
        }


        public RestaurantDto GetById(int id)
        { 
            var restaurant = _dbContext.Restaurants
                .Include(r => r.Address)        
                .Include(r => r.Dishes)         
               .FirstOrDefault(r => r.Id == id);

            if (restaurant == null)
                throw new NotFoundException($"Restaurant not found");

            var result = _mapper.Map<RestaurantDto>(restaurant); 
            return result;
        }

        public IEnumerable<RestaurantDto>GetAll()
        {
            var restaurants = _dbContext
                .Restaurants
                .Include(r => r.Address) 
                .Include(r => r.Dishes) 
                .ToList();

            var restaurantDtos = _mapper.Map<List<RestaurantDto>>(restaurants);
            return restaurantDtos;
        }

        public int Create(CreateRestaurantDto dto)
        {
            //mapowanie dto
            var restaurant = _mapper.Map<Restaurant>(dto);

            //dodać do bazy danych poprzez entity
            _dbContext.Restaurants.Add(restaurant); //dodawanie konteksu
            _dbContext.SaveChanges();               //zapisanie zmian w bazie danych

            return restaurant.Id; //zwrócenie id dodanego rekordu
        }
    }
}
