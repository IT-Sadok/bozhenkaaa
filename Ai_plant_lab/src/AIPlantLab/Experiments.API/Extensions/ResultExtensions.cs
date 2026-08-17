using Experiments.Domain.Common;

namespace Experiments.API.Extensions;

public static class ResultExtensions
{
    public static IResult MapResult(this Result result)
    {
        return result.IsSuccess ? Results.Ok() : result.Error.MapNotFoundOrBadRequest();
    }

    public static IResult MapNotFoundOrBadRequest(this string error)
    {
        return error == "Experiment not found."
            ? Results.NotFound(new { error })
            : Results.BadRequest(new { error });
    }
}
