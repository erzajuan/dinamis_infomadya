using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BlazorAuthApi.Middlewares
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            var (statusCode, title) = exception switch
            {
                // Jika exception adalah turunan BaseException, ambil StatusCode dari sana
                BaseException be => (be.StatusCode, "Application Error"),

                // Tetap petakan exception sistem yang umum
                ArgumentException => (400, "Bad Request"),
                KeyNotFoundException => (404, "Not Found"),
                UnauthorizedAccessException => (401, "Unauthorized"),

                // Default untuk error tak terduga (Bug, DB mati, dll)
                _ => (500, "Server Error")
            };
            // 1. Log errornya (sangat berguna untuk debugging)
            _logger.LogError(exception, "Terjadi error yang tidak ditangani: {Message}", exception.Message);

            // 2. Tentukan format response (ProblemDetails adalah standar industri)
            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = exception.Message,
                Instance = httpContext.Request.Path
            };

            // 3. Khusus saat Development, kamu bisa tampilkan pesan error aslinya
            // if (env.IsDevelopment()) { problemDetails.Detail = exception.Message; }

            httpContext.Response.StatusCode = problemDetails.Status.Value;

            // 4. Kirim sebagai JSON
            await httpContext.Response
                .WriteAsJsonAsync(problemDetails, cancellationToken);

            return true; // Menandakan bahwa error sudah ditangani
        }
    }

    public class BaseException : Exception
    {
        public int StatusCode { get; }

        public BaseException(string message, int statusCode = 400) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}