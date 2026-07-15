using AIAnalysis.API.Extensions;
using AIAnalysis.Application.Commands.AnalyzePhoto;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AIAnalysis.API.Endpoints;

public static class AnalysisEndpoints
{
    public static IEndpointRouteBuilder MapAnalysisEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/analysis")
            .WithTags("Analysis");

        group.MapPost("vision", AnalyzeVisionAsync)
            .DisableAntiforgery()
            .Accepts<IFormFile>("multipart/form-data")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .WithName("AnalyzeVision")
            .WithOpenApi();

        return app;
    }
    
    private static async Task<IResult> AnalyzeVisionAsync(
        IFormFile file,
        [FromQuery] Guid experimentId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var photoBytes = await file.ReadAsBytesAsync(cancellationToken);

        var command = new AnalyzePhotoCommand(experimentId, photoBytes);
        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Results.Ok(new { DiagnosisId = result.Value })
            : Results.BadRequest(new { result.Error });
    }
}