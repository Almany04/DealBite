using DealBite.Application.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace DealBite.API.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var (statusCode, title, detail) = exception switch
            { 
                ValidationException ex => (StatusCodes.Status400BadRequest, "Validációs hiba", ex.Message),
                AuthenticationException ex => (StatusCodes.Status401Unauthorized, "Sikertelen autentikáció", ex.Message),
                NotFoundException ex => (StatusCodes.Status404NotFound, "Nem található", ex.Message),
                ForbiddenException ex => (StatusCodes.Status403Forbidden, "Hozzáférés megtagadva", ex.Message),
                KeyNotFoundException ex => (StatusCodes.Status404NotFound, "Nem található", ex.Message),

                _ => (StatusCodes.Status500InternalServerError, "Szerverhiba", "Váratlan hiba történt. Kérjük, próbáld újra később.")
            };


            if (statusCode == StatusCodes.Status500InternalServerError)
            {
                _logger.LogError(exception, "Nem kezelt kivétel: {Path}", context.Request.Path);
            }
            else
            {
                _logger.LogWarning("Kezelt kivétel ({StatusCode}): {Message} - {Path}",
                    statusCode, exception.Message, context.Request.Path);
            }

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail,
                Instance = context.Request.Path
            };

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/problem+json";

            var json = JsonSerializer.Serialize(problemDetails, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }
    }
}