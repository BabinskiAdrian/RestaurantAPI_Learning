﻿using Microsoft.AspNetCore.Http.HttpResults;
using RestaurantAPI.Exceptions;

namespace RestaurantAPI.Middleware
{
    public class ErrorHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<ErrorHandlingMiddleware> _logger;
        public ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next.Invoke(context);
            }
            catch (ForbidException forbidException)
            {
                context.Response.StatusCode = 403; // Forbidden
                await context.Response.WriteAsync(forbidException.Message);
            }
            catch (BadRequestException badRequestException)
            {
                context.Response.StatusCode = 400; 
                await context.Response.WriteAsync(badRequestException.Message);
            }
            catch (NotFoundException notFoundException)
            {
                context.Response.StatusCode = 404; // Not Found
                await context.Response.WriteAsync(notFoundException.Message);
            }
            catch (Exception ex) 
            { 
                _logger.LogError(ex, ex.Message);

                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("Somethink went wrong");
            }
        }
    }
}
