using ContentService.Helpers;
using System.Net;

namespace ContentService.Middlewares
{
    public class CustomExceptionMiddleware
    {
        public RequestDelegate _next;
        public ILogger<CustomExceptionMiddleware> _logger { get; set; }

        public CustomExceptionMiddleware(RequestDelegate next, ILogger<CustomExceptionMiddleware> logger)
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
                _logger.LogError(ex, ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            _logger.LogError($"Something went wrong: {ex}");
            return context.Response.WriteAsync(new ApiResponse<object> { Message = "Something went wrong", Status = ResponseStatus.Error }.ToString());
        }
    }
}
