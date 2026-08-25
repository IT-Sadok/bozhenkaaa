using Experiments.Domain.Common;
using Experiments.Domain.Constants;

namespace Experiments.API.Extensions;

public static class ResultExtensions
{
    public static IResult MapResult(this Result result)
    {
        return result.IsSuccess ? Results.Ok() : result.MapFailure();
    }

    public static IResult MapFailure(this Result result)
    {
        if (result.Source is not null && result.Error == ValidationMessages.NotFound)
        {
            return Results.NotFound(new { error = result.Error });
        }

        return Results.BadRequest(new { error = result.Error });
    }

    public static IResult MapFailure<T>(this Result<T> result)
    {
        if (result.Source is not null && result.Error == ValidationMessages.NotFound)
        {
            return Results.NotFound(new { error = result.Error });
        }

        return Results.BadRequest(new { error = result.Error });
    }
}
