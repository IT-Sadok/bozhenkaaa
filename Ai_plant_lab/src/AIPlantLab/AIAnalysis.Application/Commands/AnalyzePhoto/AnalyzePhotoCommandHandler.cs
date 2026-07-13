using AIAnalysis.Application.Interfaces;
using AIAnalysis.Domain.Common;
using AIAnalysis.Domain.Entities;
using MediatR;

namespace AIAnalysis.Application.Commands.AnalyzePhoto;

internal sealed class AnalyzePhotoCommandHandler : IRequestHandler<AnalyzePhotoCommand, Result<Guid>>
{
    private readonly IAiVisionService _aiVisionService;
    private readonly IAppDbContext _dbContext;

    public AnalyzePhotoCommandHandler(
        IAiVisionService aiVisionService, 
        IAppDbContext dbContext)
    {
        _aiVisionService = aiVisionService;
        _dbContext = dbContext;
    }

    public async Task<Result<Guid>> Handle(AnalyzePhotoCommand request, CancellationToken cancellationToken)
    {
        var analyzePlantResult = await _aiVisionService.AnalyzePlantPhotoAsync(request.PhotoData, cancellationToken);
        
        if (analyzePlantResult.IsFailure)
        {
            return Result<Guid>.ErrorResult(analyzePlantResult.Error);
        }

        var diagnosisResultDto = analyzePlantResult.Value;
        var diagnosis = new PlantDiagnosis(
            request.ExperimentId,
            "url_to_blob_storage_placeholder", // TODO: add saving to blob
            diagnosisResultDto.DetectedDisease,
            diagnosisResultDto.ConfidenceScore,
            diagnosisResultDto.Recommendations,
            diagnosisResultDto.IsHealthy);

        _dbContext.Diagnoses.Add(diagnosis);
        
        // TODO: outbox
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(diagnosis.Id);
    }
}