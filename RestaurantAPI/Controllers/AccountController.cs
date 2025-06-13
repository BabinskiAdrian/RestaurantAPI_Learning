using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using RestaurantAPI.Models;
using RestaurantAPI.Services;
using static RestaurantAPI.Services.AccountService;

namespace RestaurantAPI.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [Route("register")]
        [HttpPost]
        public ActionResult RegisterUser([FromBody] RegisterUserDto dto)
        {
            _accountService.RegisterUser(dto);

            return Ok("User registered successfully");
        }


        [Route("login")]
        [HttpPost]
        public ActionResult LoginUser([FromBody] LoginUserDto dto)
        {
            string token = _accountService.GenerateJwt(dto);

            return Ok(token);
        }
    }
}
