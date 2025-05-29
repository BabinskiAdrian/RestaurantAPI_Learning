using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Entities ;
using RestaurantAPI.Models;
using RestaurantAPI.Services;

namespace RestaurantAPI.Controllers
{
    [Route("api/restaurant")]
    public class RestaurantController : ControllerBase
    {
        private readonly IRestaurantServices _restaurantServices; 

        public RestaurantController(IRestaurantServices restaurantServices) 
        {
            _restaurantServices = restaurantServices;
         }


        [HttpPut("{id}")]
        public ActionResult Update([FromBody] UpdateRestaurantDto dto, [FromRoute] int id) 
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState); //zwróci 400
            }

            bool recordExist = _restaurantServices.Update(dto, id);
            if (!recordExist)
            {
                return NotFound();
            }

            return Ok();
        }



        [HttpDelete("{id}")]
        public ActionResult Delete([FromRoute] int id)
        {
            bool isDeleted;
            isDeleted = _restaurantServices.Delete(id);

            //W zależności od tego co zrobi funkcja, wyślemy różny status odpowiedzi
            if (isDeleted)
            {
                return NoContent(); //zwróci 204
            }
            else
            {
                return NotFound(); //zwróci 404
            }
        }


        [HttpPost]
         public ActionResult CreateRestaurant([FromBody] CreateRestaurantDto dto )
        {
            //Sprawdzanie czy wysłany rekord jest poprawny, czy dane są validate
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState); 
            }

            var restaurantId = _restaurantServices.Create(dto);

            return Created($"/api/restaurant/{restaurantId}", null);
        }
        

        [HttpGet]
        public ActionResult<IEnumerable<RestaurantDto>> GetAll()
        {
            var restaurantsDto = _restaurantServices.GetAll();

            return Ok(restaurantsDto);
        }

        [HttpGet("{id}")]
        public ActionResult<RestaurantDto> Get([FromRoute] int id)
        {
            var restaurant = _restaurantServices.GetById(id);

            if (restaurant == null)
            {
                return NotFound(); //zwróci 404
            }
            else
            { 
                return Ok(restaurant); //zwróci 200
            }

        }

    }
}
