using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Entities;
using RestaurantAPI.Exceptions;
using RestaurantAPI.Models;
using RestaurantAPI.Authorization;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Linq.Expressions;

namespace RestaurantAPI.Services
{
    public interface IRestaurantServices
    {
        RestaurantDto GetById(int id);
        PagedResult<RestaurantDto> GetAll(RestaurantQuery query);
        int Create(CreateRestaurantDto dto);    
        void Delete(int id);
        void Update(int id, UpdateRestaurantDto dto);
    }

    public class RestaurantServices : IRestaurantServices
    {
        #region Zależności
        // Zmienne zależności
        private readonly RestaurantDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<RestaurantServices> _logger;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserContextService _userContextService;

        // Wstrzykiwanie zależności przez konstruktor
        public RestaurantServices(RestaurantDbContext dbContext, IMapper mapper, ILogger<RestaurantServices> logger,
            IAuthorizationService authorizationService, IUserContextService userContextService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
            _authorizationService = authorizationService;
            _userContextService = userContextService;
        }
        #endregion

        public void Update(int id, UpdateRestaurantDto dto)
        {

            var restaurant = _dbContext.Restaurants
                .FirstOrDefault(r => r.Id == id);

            if (restaurant == null)
                throw new NotFoundException($"Restaurant not found");


            var authorizationResult = _authorizationService
                .AuthorizeAsync(_userContextService.User, restaurant, new ResourceOperationRequirement(ResourceOperation.Update)).Result;

            if(!authorizationResult.Succeeded)
            {
                throw new ForbidException();
            }

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


            var authorizationResult = _authorizationService
                .AuthorizeAsync(_userContextService.User, restaurant, new ResourceOperationRequirement(ResourceOperation.Delete)).Result;

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
        public PagedResult<RestaurantDto>GetAll(RestaurantQuery query)
        {
            var baseQuery = _dbContext
                .Restaurants
                .Include(r => r.Address)
                .Include(r => r.Dishes)
                .Where(r => (query.SearchPhrase == null) 
                || r.Name.ToLower().Contains(query.SearchPhrase.ToLower())
                || r.Description.ToLower().Contains(query.SearchPhrase.ToLower()));


            if(!string.IsNullOrEmpty(query.SortBy))
            {
                var columnsSelector = new Dictionary<string, Expression<Func<Restaurant, object>>>
                {
                    //{ "Name", r=>r.Name }, // hardcode jak dla debila
                    { nameof(Restaurant.Name), r=>r.Name},
                    { nameof(Restaurant.Description), r=>r.Description},
                    { nameof(Restaurant.Category), r=>r.Category},
                };
                var selectedColumn = columnsSelector[query.SortBy];

                baseQuery = query.SortDirection == SortDirection.ASC
                    ? baseQuery.OrderBy(selectedColumn)
                    : baseQuery.OrderByDescending(selectedColumn);
            }

            var restaurants = baseQuery
                .Skip(query.PageSize * (query.PageNumber - 1))
                .Take(query.PageSize)
                .ToList();

            var totalItemsCount = baseQuery.Count();

            var restaurantDtos = _mapper.Map<List<RestaurantDto>>(restaurants);

            var result = new PagedResult<RestaurantDto>(restaurantDtos, totalItemsCount, query.PageSize, query.PageNumber);

            return result;
        }

        public int Create(CreateRestaurantDto dto)
        {
            //mapowanie dto
            var restaurant = _mapper.Map<Restaurant>(dto);
            restaurant.CreatedById = _userContextService.GetUserId; //pobranie id użytkownika z kontekstu użytkownika

            //dodać do bazy danych poprzez entity
            _dbContext.Restaurants.Add(restaurant); //dodawanie konteksu
            _dbContext.SaveChanges();               //zapisanie zmian w bazie danych

            return restaurant.Id; //zwrócenie id dodanego rekordu
        }


    }
}
