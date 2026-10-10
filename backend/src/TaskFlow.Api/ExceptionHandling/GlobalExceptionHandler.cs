using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;

namespace TaskFlow.Api.ExceptionHandling
{
    public class GlobalExceptionHandler(
      ILogger<GlobalExceptionHandler> logger)
      : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
         HttpContext httpContext,
         Exception exception,
         CancellationToken cancellationToken)
        {
            if (exception is ValidationException validationException)
            {
                var errors = validationException.Errors
                    .GroupBy(error => error.PropertyName)
                    .ToDictionary(
                        group => group.Key,
                        group => group
                            .Select(error => error.ErrorMessage)
                            .Distinct()
                            .ToArray());

                await Results.ValidationProblem(
                    errors,
                    title: "Doğrulama hatası",
                    instance: httpContext.Request.Path.Value,
                    statusCode: StatusCodes.Status400BadRequest)
                    .ExecuteAsync(httpContext);

                return true;
            }

            logger.LogError(
                exception,
                "İstek işlenirken beklenmeyen hata oluştu. Path: {Path}",
                httpContext.Request.Path);

            await Results.Problem(
                title: "Beklenmeyen bir hata oluştu.",
                detail: "İşlem tamamlanamadı. Lütfen daha sonra tekrar deneyin.",
                instance: httpContext.Request.Path.Value,
                statusCode: StatusCodes.Status500InternalServerError)
                .ExecuteAsync(httpContext);

            return true;
        }
    }
}
