using Experiments.Domain.Common;
using MediatR;

namespace Experiments.Application.Commands.ConfigureExperiment;

public sealed record ConfigureExperimentCommand(
    Guid ExperimentId,
    decimal LightHoursPerDay,
    int WateringIntervalDays,
    decimal TargetTemperatureCelsius,
    string? Notes) : IRequest<Result>;
