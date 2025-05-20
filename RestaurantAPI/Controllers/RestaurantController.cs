using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Entities;
using RestaurantAPI.Models;

namespace RestaurantAPI.Controllers
{
    [Route("api/restaurant")]
    public class RestaurantController : ControllerBase
    {
        //pola tylko do otycztu służące do wstrzykiwania zależności, w kolejnym kroku
        private readonly RestaurantDbContext _dbContext;
        private readonly IMapper _mapper;

        //konstruktor kontorlera, w ktrym wstrzykiwane są zależności
        public RestaurantController(RestaurantDbContext dbContext, IMapper mapper) 
        {
            _dbContext = dbContext; 
            _mapper = mapper;
        }

        [HttpPost]
         public ActionResult CreateRestaurant([FromBody] CreateRestaurantDto dto )
        {
            //Sprawdzanie czy wysłany rekord jest poprawny, czy dane są validate
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState); //zwróci 400
            }

            //mapowanie dto
            var restaurant = _mapper.Map<Restaurant>(dto);

            //dodać do bazy danych poprzez entity
            _dbContext.Restaurants.Add(restaurant); //dodawanie konteksu
            _dbContext.SaveChanges();               //zapisanie zmian w bazie danych

            return Created($"/api/restaurant/{restaurant.Id}", null); //zwróci 201
        }
        

        [HttpGet]
        public ActionResult<IEnumerable<RestaurantDto>> GetAll()
        {
            //entity framework, stoworzy odpowiednie zapytanie do baz danych SQL żeby wyciągnąć listę restauracji
            var restaurants = _dbContext
                .Restaurants
                .Include(r => r.Address)    //rozszerzenie o tabele powiązane
                .Include(r => r.Dishes)     //rozszerzenie o tabele powiązane
                .ToList();                  //pobranie wszystkich restauracji z bazy danych

            var restaurantsDto = _mapper.Map<List<RestaurantDto>>(restaurants);

            return Ok(restaurantsDto);
        }

        [HttpGet("{id}")]
        public ActionResult<RestaurantDto> Get([FromRoute] int id)
        {
            //Zwróci element restauracji o podanym id, jeżeli istnieje. Jeżeli nie istnieje to zwróci null
            var restaurant = _dbContext.Restaurants
                .Include(r => r.Address)    //rozszerzenie o tabele powiązane
                .Include(r => r.Dishes)     //rozszerzenie o tabele powiązane
               .FirstOrDefault(r => r.Id == id); //predykata jako wyraażenie lambda
            
            if(restaurant == null)
            {
                return NotFound(); //zwróci 404
            }

            var restaurantDto = _mapper.Map<RestaurantDto>(restaurant); //mapowanie z obiektu do DTO
            {
                return Ok(restaurantDto); //zwróci 200
            }
        }

    }
}
