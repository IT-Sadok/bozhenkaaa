using AIAnalysis.Application.Interfaces.Repositories;
using AIAnalysis.Application.Interfaces.Services;
using AIAnalysis.Domain.Common;
using AIAnalysis.Domain.Entities;
using AIAnalysis.Domain.Enums;
using MediatR;

namespace AIAnalysis.Application.Commands.AnalyzePhoto;

internal sealed class AnalyzePhotoCommandHandler : IRequestHandler<AnalyzePhotoCommand, Result<Guid>>
{
    private readonly IAiVisionService _aiVisionService;
    private readonly IPlantDiagnosisRepository _diagnosisRepository;
    private readonly IDiseaseRepository _diseaseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AnalyzePhotoCommandHandler(
        IAiVisionService aiVisionService,
        IPlantDiagnosisRepository diagnosisRepository,
        IDiseaseRepository diseaseRepository,
        IUnitOfWork unitOfWork)
    {
        _aiVisionService = aiVisionService;
        _diagnosisRepository = diagnosisRepository;
        _diseaseRepository = diseaseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(AnalyzePhotoCommand request, CancellationToken cancellationToken)
    {
        var analyzePlantPhotoResult = await _aiVisionService.AnalyzePlantPhotoAsync(request.PhotoData, cancellationToken);
        if (analyzePlantPhotoResult.IsFailure)
        {
            return Result<Guid>.ErrorResult(analyzePlantPhotoResult.Error);
        }

        var analyzedData = analyzePlantPhotoResult.Value;

        var healthStatus = ParseHealthStatus(analyzedData.Status);
        var diseaseId = await ResolveDiseaseIdAsync(healthStatus, analyzedData.DetectedDisease, cancellationToken);

        var diagnosis = new PlantDiagnosis(
            request.ExperimentId,
            "url_to_blob_storage_placeholder", // TODO: add saving to blob
            diseaseId,
            analyzedData.ConfidenceScore,
            analyzedData.Recommendations,
            healthStatus
        );

        _diagnosisRepository.Add(diagnosis);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(diagnosis.Id);
    }

    private static HealthStatus ParseHealthStatus(string? status)
    {
        return Enum.TryParse<HealthStatus>(status, true, out var result)
            ? result
            : HealthStatus.Suspicious;
    }

    private async Task<Guid?> ResolveDiseaseIdAsync(
        HealthStatus status, 
        string? detectedDiseaseName, 
        CancellationToken cancellationToken)
    {
        if (status != HealthStatus.DiseaseDetected || string.IsNullOrWhiteSpace(detectedDiseaseName))
        {
            return null;
        }

        var diseaseRecord = await _diseaseRepository.GetByNameAsync(detectedDiseaseName, cancellationToken);
        return diseaseRecord?.Id;
    }
}