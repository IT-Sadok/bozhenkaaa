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
        var aiResult = await _aiVisionService.AnalyzePlantPhotoAsync(request.PhotoData, cancellationToken);
        if (aiResult.IsFailure)
        {
            return Result<Guid>.ErrorResult(aiResult.Error);
        }

        var aiData = aiResult.Value;

        if (!Enum.TryParse<HealthStatus>(aiData.Status, true, out var healthStatus))
        {
            healthStatus = HealthStatus.Suspicious;
        }

        Guid? diseaseId = null;
        if (healthStatus == HealthStatus.DiseaseDetected && !string.IsNullOrWhiteSpace(aiData.DetectedDisease))
        {
            var diseaseRecord = await _diseaseRepository.GetByNameAsync(aiData.DetectedDisease, cancellationToken);
            diseaseId = diseaseRecord?.Id;
        }

        var diagnosis = new PlantDiagnosis(
            request.ExperimentId,
            "url_to_blob_storage_placeholder", // TODO: add saving to blob
            diseaseId,
            aiData.ConfidenceScore,
            aiData.Recommendations,
            healthStatus
        );

        _diagnosisRepository.Add(diagnosis);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(diagnosis.Id);
    }
}