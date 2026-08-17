using Experiments.API.Extensions;
using Experiments.API.Models;
using Experiments.Application.Commands.AddPlantGroup;
using Experiments.Application.Commands.ConfigureExperiment;
using Experiments.Application.Commands.CreateExperiment;
using Experiments.Application.Commands.FinishExperiment;
using Experiments.Application.Commands.StartExperiment;
using Experiments.Application.DTOs;
using Experiments.Application.Queries.GetExperimentById;
using Experiments.Application.Queries.ListExperiments;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Experiments.API.Endpoints;

public static class ExperimentEndpoints
{
    private const string RoutePrefix = "api/experiments";
    private const string TagName = "Experiments";

    public static void MapExperimentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(RoutePrefix)
            .WithTags(TagName);

        group.MapPost("/", CreateExperimentAsync)
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .WithName("CreateExperiment")
            .WithOpenApi();

        group.MapPut("/{id:guid}/configure", ConfigureExperimentAsync)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("ConfigureExperiment")
            .WithOpenApi();

        group.MapPost("/{id:guid}/plant-groups", AddPlantGroupAsync)
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("AddPlantGroup")
            .WithOpenApi();

        group.MapPost("/{id:guid}/start", StartExperimentAsync)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("StartExperiment")
            .WithOpenApi();

        group.MapPost("/{id:guid}/finish", FinishExperimentAsync)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("FinishExperiment")
            .WithOpenApi();

        group.MapGet("/{id:guid}", GetExperimentByIdAsync)
            .Produces<ExperimentDto>()
            .Produces(StatusCodes.Status404NotFound)
            .WithName("GetExperimentById")
            .WithOpenApi();

        group.MapGet("/", ListExperimentsAsync)
            .Produces<PagedResult<ExperimentSummaryDto>>()
            .Produces(StatusCodes.Status400BadRequest)
            .WithName("ListExperiments")
            .WithOpenApi();
    }

    private static async Task<IResult> CreateExperimentAsync(
        CreateExperimentRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new CreateExperimentCommand(request.Name, request.Description);
        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Results.Created($"{RoutePrefix}/{result.Value}", new { ExperimentId = result.Value })
            : Results.BadRequest(new { result.Error });
    }

    private static async Task<IResult> ConfigureExperimentAsync(
        Guid id,
        ConfigureExperimentRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var configuration = new ExperimentConfigurationDto
        {
            LightHoursPerDay = request.LightHoursPerDay,
            WateringIntervalDays = request.WateringIntervalDays,
            TargetTemperatureCelsius = request.TargetTemperatureCelsius,
            Notes = request.Notes
        };

        var command = new ConfigureExperimentCommand(id, configuration);
        var result = await sender.Send(command, cancellationToken);

        return result.MapResult();
    }

    private static async Task<IResult> AddPlantGroupAsync(
        Guid id,
        AddPlantGroupRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new AddPlantGroupCommand(id, request.Name, request.Species, request.PlantCount);
        var result = await sender.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            return Results.Created($"{RoutePrefix}/{id}/plant-groups/{result.Value}", new { PlantGroupId = result.Value });
        }

        return result.Error.MapNotFoundOrBadRequest();
    }

    private static async Task<IResult> StartExperimentAsync(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new StartExperimentCommand(id), cancellationToken);
        return result.MapResult();
    }

    private static async Task<IResult> FinishExperimentAsync(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new FinishExperimentCommand(id), cancellationToken);
        return result.MapResult();
    }

    private static async Task<IResult> GetExperimentByIdAsync(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetExperimentByIdQuery(id), cancellationToken);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.NotFound(new { result.Error });
    }

    private static async Task<IResult> ListExperimentsAsync(
        [FromQuery] int page,
        [FromQuery] int pageSize,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new ListExperimentsQuery(page == 0 ? 1 : page, pageSize == 0 ? 20 : pageSize),
            cancellationToken);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(new { result.Error });
    }
}
