namespace Api.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
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

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var (statusCode, errorType) = exception switch
            {
                Application.Exceptions.AppValidationException => (StatusCodes.Status400BadRequest, "ValidationError"),
                Application.Exceptions.NotFoundException => (StatusCodes.Status404NotFound, "NotFoundError"),
                Application.Exceptions.UnauthorizedException => (StatusCodes.Status401Unauthorized, "UnauthorizedError"),
                Application.Exceptions.InternalServerErrorException => (StatusCodes.Status500InternalServerError, "InternalServerError"),
                _ => (StatusCodes.Status500InternalServerError, "ServerError")
            };

            context.Response.StatusCode = statusCode;

            var response = new
            {
                error = errorType,
                message = exception.Message
            };

            await context.Response.WriteAsJsonAsync(response);
        }
    }

    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionCatcherMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
