using AIAnalysis.API.Extensions;
using AIAnalysis.API.Models;
using AIAnalysis.Application.Commands.AnalyzePhoto;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AIAnalysis.API.Endpoints;

public static class AnalysisEndpoints
{
    private const string RoutePrefix = "api/analysis";
    private const string TagName = "Analysis";
    private const string AnalyzeVisionRoute = "vision";
    private const string AnalyzeVisionEndpointName = "AnalyzeVision";
    
    public static IEndpointRouteBuilder MapAnalysisEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(RoutePrefix)
            .WithTags(TagName);

        group.MapPost(AnalyzeVisionRoute, AnalyzeVisionAsync)
            .DisableAntiforgery()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .WithName(AnalyzeVisionEndpointName)
            .WithOpenApi();

        return app;
    }
    
    private static async Task<IResult> AnalyzeVisionAsync(
        [FromForm] AnalyzeVisionRequestDto request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var photoBytes = await request.File.ReadAsBytesAsync(cancellationToken);

        var command = new AnalyzePhotoCommand(request.ExperimentId, photoBytes);
        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Results.Ok(new { DiagnosisId = result.Value })
            : Results.BadRequest(new { result.Error });
    }
}