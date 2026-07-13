using AIAnalysis.Domain.Common;
using MediatR;

namespace AIAnalysis.Application.Commands.AnalyzePhoto;

public record AnalyzePhotoCommand(
    Guid ExperimentId, 
    byte[] PhotoData) : IRequest<Result<Guid>>;