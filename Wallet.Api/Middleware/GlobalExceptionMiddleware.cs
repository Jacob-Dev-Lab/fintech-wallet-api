namespace Wallet.Api.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, 
            ILogger<GlobalExceptionMiddleware> logger)
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
                _logger.LogError(ex, "An unexpected error occurred");

                context.Response.StatusCode = 500;

                var response = new
                {
                    message = "An unexpected error occurred. Please try again later.",
                    errors = new[] { ex.Message }
                };

                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}
