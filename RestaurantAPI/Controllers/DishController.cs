using Microsoft.AspNetCore.Mvc;
using RestaurantAPI.Models;
using RestaurantAPI.Services;
using Microsoft.EntityFrameworkCore.Infrastructure;
using RestaurantAPI.Entities;

namespace RestaurantAPI.Controllers
{
    [Route("api/restaurant/{restaurantId}/dish")]
    [ApiController]
    public class DishController : ControllerBase
    {
        private readonly IDishService _dishService;

        public DishController(IDishService dishService) 
        { 
            _dishService = dishService;
        }

        [HttpPost]
        public ActionResult Post([FromRoute] int restaurantId, [FromBody] CreateDishDto dto)
        {

            var newDishId = _dishService.Create(restaurantId, dto);

            return Created($"api/restaurant/{restaurantId}/dish/{newDishId}", null);
        }


        [Route("{dishId}")]
        [HttpGet]
        public ActionResult<DishDto> GetDish([FromRoute] int restaurantId, [FromRoute] int dishId)
        {
            DishDto dish = _dishService.GetById(restaurantId, dishId);

            return Ok(dish);
        }

        [HttpGet()]
        public ActionResult<IEnumerable<DishDto>> GetAll([FromRoute] int restaurantId)
        {
            var allDishes = _dishService.GetAll(restaurantId);

            return Ok(allDishes);
        }


        [HttpDelete]
        public ActionResult DeleteAllDishes([FromRoute] int restaurantId)
        {
            _dishService.DeleteAll(restaurantId);

            return NoContent();
        }

        [Route("{dishId}")]
        [HttpDelete]
        public ActionResult DeleteOnelDishes([FromRoute] int restaurantId, [FromRoute] int dishId)
        {
            _dishService.DeleteOne(restaurantId, dishId);

            return NoContent();
        }

    }
}
