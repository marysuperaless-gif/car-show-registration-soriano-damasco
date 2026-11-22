namespace Api.Common.Filters;

using FluentValidation;

/// <summary>Runs the FluentValidation validator of T BEFORE the endpoint handler. Failures return 400.</summary>
public sealed class ValidationFilter<T> : IEndpointFilter where T : class
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var validator = context.HttpContext.RequestServices.GetService<IValidator<T>>();
        if (validator is null)
        {
            return await next(context);
        }

        var argument = context.Arguments.OfType<T>().FirstOrDefault();
        if (argument is null)
        {
            return Results.Problem(
                title: "Registration.Validation",
                detail: "The request body is missing or invalid.",
                statusCode: StatusCodes.Status400BadRequest);
        }

        var result = await validator.ValidateAsync(argument, context.HttpContext.RequestAborted);
        if (!result.IsValid)
        {
            // 400 ProblemDetails with an "errors" dictionary: { "Email": ["..."], ... }
            return Results.ValidationProblem(result.ToDictionary());
        }

        return await next(context);
    }
}
