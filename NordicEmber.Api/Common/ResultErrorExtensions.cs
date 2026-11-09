using Microsoft.AspNetCore.Http.HttpResults;

namespace NordicEmber.Api.Common;

public static class ResultErrorExtensions
{
    public static ProblemHttpResult ToProblem(this ResultError error)
    {
        var (statusCode, title) = error.Type switch
        {
            ResultErrorType.Validation => (StatusCodes.Status400BadRequest, "One or more validation errors occurred."),
            ResultErrorType.Conflict => (StatusCodes.Status409Conflict, "The request conflicts with existing data."),
            _ => throw new ArgumentOutOfRangeException(nameof(error))
        };

        var problem = new HttpValidationProblemDetails(
            new Dictionary<string, string[]> { [error.Field] = [error.Message] })
        {
            Status = statusCode,
            Title = title
        };

        return TypedResults.Problem(problem);
    }
}
