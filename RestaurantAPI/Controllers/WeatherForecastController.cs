using Microsoft.AspNetCore.Mvc;

namespace RestaurantAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {

        private readonly IWeatherForecastService _service;
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IWeatherForecastService service)
        {
            _logger = logger;
            _service = service;
        }

        //Zadanie
        [HttpPost]
        [Route("/generate")]
        //public IEnumerable<WeatherForecast> GetZadaniePraktyczne()
        //By móc w body wysłać więcej niż 2 parametry, musimy zrobić to poprzez klasę.
        public ActionResult<IEnumerable<WeatherForecast>> GetZadaniePraktyczne([FromQuery]int count, [FromBody] TemperatureRequest request)
        {
            if (count < 0 || request.min > request.max) 
            {
                return BadRequest();
            }

            var results = _service.GetZadaniePraktyczne(count, request.min, request.max);
            return Ok(results);
        }






        //Notatki
        // bazowy endpoint
        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var result = _service.Get();
            return result;
        }

        // nr.1 sposób modyfikacji ścieżki do endpoint
        [HttpGet]
        [Route("currentDay/{max}")]
        public IEnumerable<WeatherForecast> Get2([FromQuery]int take, [FromRoute]int max)
        {
            var result = _service.Get();
            return result;
        }

        // nr.2 sposub modyfikacji ścieżki do endpoint
        [HttpGet("futureDay")]
        public IEnumerable<WeatherForecast> Get3()
        { 
            var result = _service.Get();
            return result;
        }
        /*
        [HttpPost]
        public string SayHello([FromBody] string name)
        {
            return $"Hello {name}";
        }

        //By móc zmienić statusCode, potrzeba zmienić typ zwracany z string na ActionResult<T>
        */

        [HttpPost]
        public ActionResult<string> SayHello([FromBody] string name)
        {
            return StatusCode(401, $"Hello {name}");
        }


        [HttpPost]
        [Route("NotFound404")]
        public ActionResult<string> SayHello2([FromBody] string name)
        {
            return NotFound($"Hello, but not found {name}");
        }
    }
}
