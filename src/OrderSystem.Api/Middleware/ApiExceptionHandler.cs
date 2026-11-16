namespace OrderSystem.Api.Middleware;

using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using OrderSystem.Application.Common.Exceptions;

internal sealed class ApiExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<ApiExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var problemDetails = exception switch
        {
            ValidationException validationException =>
                CreateValidationProblemDetails(
                    validationException,
                    httpContext.Request.Path.Value),
            CustomerNotFoundException customerNotFoundException =>
                new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Customer not found",
                    Detail = customerNotFoundException.Message,
                    Instance = httpContext.Request.Path.Value
                },
            ProductsNotFoundException productsNotFoundException =>
                new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Products not found",
                    Detail = productsNotFoundException.Message,
                    Instance = httpContext.Request.Path.Value
                },
            DbUpdateException
            {
                InnerException: PostgresException
                {
                    SqlState: PostgresErrorCodes.UniqueViolation,
                    ConstraintName: "IX_Customers_Email"
                }
            } =>
                new ProblemDetails
                {
                    Status = StatusCodes.Status409Conflict,
                    Title = "Email already in use",
                    Detail = "A customer with this email already exists.",
                    Instance = httpContext.Request.Path.Value
                },
            _ => null
        };

        if (problemDetails is null)
        {
            return false;
        }

        logger.LogWarning(
            exception,
            "Handled API exception with status code {StatusCode}.",
            problemDetails.Status);

        httpContext.Response.StatusCode =
            problemDetails.Status ?? StatusCodes.Status500InternalServerError;

        await problemDetailsService.WriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails
        });

        return true;
    }

    private static ValidationProblemDetails CreateValidationProblemDetails(
        ValidationException exception,
        string? instance)
    {
        var errors = exception.Errors
            .GroupBy(error => error.PropertyName)
            .ToDictionary(
                group => group.Key,
                group => group.Select(error => error.ErrorMessage).ToArray());

        return new ValidationProblemDetails(errors)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "One or more validation errors occurred.",
            Instance = instance
        };
    }
}
