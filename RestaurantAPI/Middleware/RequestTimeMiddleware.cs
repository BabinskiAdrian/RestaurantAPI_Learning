using System.Diagnostics;

namespace RestaurantAPI.Middleware
{
    public class RequestTimeMiddleware : IMiddleware
    {
        private readonly ILogger<RequestTimeMiddleware> _logger;
        private Stopwatch _stopWatch;

        //wstrzykiwanie loggera
        public RequestTimeMiddleware(ILogger<RequestTimeMiddleware> logger)
        {
            _logger = logger;
            _stopWatch = new Stopwatch();
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            _stopWatch.Start();
            await next.Invoke(context);
            _stopWatch.Stop();

            var elapsedMilliseconds = _stopWatch.ElapsedMilliseconds; // type Int64
            if (elapsedMilliseconds > 4000)
            {
                var message = $"Request [{context.Request.Method}] at [{context.Request.Path}] took {elapsedMilliseconds} ms";
                _logger.LogInformation(message);
            }
        }

    }
}
