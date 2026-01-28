using LMS.Application.Exceptions;
using System.Net;
using System.Text.Json;

namespace LMS.API.Middleware
{
    /// <summary>
    /// Global Exception Handling Middleware
    /// Catches all unhandled exceptions in pipeline and converts them to appropriate HTTP responses
    /// 
    /// Flow:
    /// Request → [Other Middlewares] → ExceptionHandlingMiddleware → [Controllers] → Response
    ///                                          ↓
    ///                                    (Catch exceptions)
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        /// <summary>
        /// Constructor inject RequestDelegate and ILogger
        /// </summary>
        /// <param name="next">Next middleware in the pipeline</param>
        /// <param name="logger">Logger for logging exceptions</param>
        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Invoke method - called on every HTTP request
        /// Wraps _next() in try-catch to catch all exceptions
        /// </summary>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Call next middleware in the pipeline
                await _next(context);
            }
            catch (Exception ex)
            {
                // Log exception with full stack trace
                _logger.LogError(ex, "An unhandled exception occurred");
                
                // Convert exception to HTTP response
                await HandleExceptionAsync(context, ex);
            }
        }

        /// <summary>
        /// Convert exception to HTTP response with appropriate status code and message
        /// 
        /// Mapping:
        /// - NotFoundException → 404 Not Found
        /// - ForbiddenException → 403 Forbidden
        /// - UnauthorizedException → 401 Unauthorized
        /// - BadRequestException → 400 Bad Request
        /// - Other exceptions → 500 Internal Server Error
        /// </summary>
        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Set response content type
            context.Response.ContentType = "application/json";

            // Map exception type → HTTP status code and message
            var (statusCode, message, errors) = exception switch
            {
                NotFoundException notFound => (HttpStatusCode.NotFound, notFound.Message, new List<string>()),
                ForbiddenException forbidden => (HttpStatusCode.Forbidden, forbidden.Message, new List<string>()),
                UnauthorizedException unauthorized => (HttpStatusCode.Unauthorized, unauthorized.Message, new List<string>()),
                BadRequestException badRequest => (HttpStatusCode.BadRequest, badRequest.Message, badRequest.Errors),
                _ => (HttpStatusCode.InternalServerError, "An internal server error occurred", new List<string>())
            };

            // Set HTTP status code
            context.Response.StatusCode = (int)statusCode;

            // Create response object with standard format
            var result = JsonSerializer.Serialize(new
            {
                status = (int)statusCode,      // HTTP status code (400, 401, 403, 404, 500...)
                message,                       // Error message
                errors                         // Detailed errors (for validation)
            });

            // Write response
            return context.Response.WriteAsync(result);
        }
    }
}

