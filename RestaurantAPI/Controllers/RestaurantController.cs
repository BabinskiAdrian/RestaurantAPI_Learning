using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Entities ;
using RestaurantAPI.Models;
using RestaurantAPI.Services;
using System.Security.Claims;

namespace RestaurantAPI.Controllers
{
    [Route("api/restaurant")]
    [ApiController]
    //[Authorize]
    public class RestaurantController : ControllerBase
    {
        private readonly IRestaurantServices _restaurantServices;

        public RestaurantController(IRestaurantServices restaurantServices) 
        {
            _restaurantServices = restaurantServices;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Default()
        {
            return Ok("Hello from Restaurant API");
        }


        [HttpPut("{id}")]
        public ActionResult Update([FromBody] UpdateRestaurantDto dto, [FromRoute] int id) 
        {
            _restaurantServices.Update(id, dto);

            return Ok();
        }



        [HttpDelete("{id}")]
        [Authorize(Policy = "Atleast20Age")]
        public ActionResult Delete([FromRoute] int id)
        {
            _restaurantServices.Delete(id);

            return NoContent(); //zwróci 204
        }


        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public ActionResult CreateRestaurant([FromBody] CreateRestaurantDto dto)
        {
            var userId = int.Parse(User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var restaurantId = _restaurantServices.Create(dto);

            return Created($"/api/restaurant/{restaurantId}", null);
        }

        [HttpGet("TestAuthorization")]
        [Authorize(Policy = "AtLeast2CreatedRestaurant")]
        public ActionResult<IEnumerable<RestaurantDto>> TestAuthorization()
        {
            return Ok("Test autoryzacji przebiegl poprawnie");
        }


        [HttpGet("all")]
        public ActionResult<IEnumerable<RestaurantDto>> GetAll([FromQuery]RestaurantQuery query)
        {
            var restaurantsDto = _restaurantServices.GetAll(query);

            return Ok(restaurantsDto);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public ActionResult<RestaurantDto> Get([FromRoute]int id)
        {
            var restaurant = _restaurantServices.GetById(id);

            return Ok(restaurant); //zwróci 200
        }

    }
}
