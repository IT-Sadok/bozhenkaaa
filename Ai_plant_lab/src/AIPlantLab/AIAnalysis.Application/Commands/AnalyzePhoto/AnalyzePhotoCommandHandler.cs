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
    private readonly IUnitOfWork _unitOfWork;

    public AnalyzePhotoCommandHandler(
        IAiVisionService aiVisionService,
        IUnitOfWork unitOfWork)
    {
        _aiVisionService = aiVisionService;
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
        var disease = await ResolveDiseaseAsync(healthStatus, analyzedData.DetectedDisease, cancellationToken);

        var diagnosis = new PlantDiagnosis(
            experimentId: request.ExperimentId,
            imageUrl: "url_to_blob_storage_placeholder",
            detectedDiseaseId: disease?.Id,
            confidenceScore: analyzedData.ConfidenceScore,
            recommendations: analyzedData.Recommendations,
            status: healthStatus,
            detectedDisease: disease
        );

        _unitOfWork.Diagnoses.Add(diagnosis);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(diagnosis.Id);
    }

    private static HealthStatus ParseHealthStatus(string? status)
    {
        return Enum.TryParse<HealthStatus>(status, true, out var result)
            ? result
            : HealthStatus.Suspicious;
    }

    private async Task<Disease?> ResolveDiseaseAsync(
        HealthStatus status, 
        string? detectedDiseaseName, 
        CancellationToken cancellationToken)
    {
        if (status != HealthStatus.DiseaseDetected || string.IsNullOrWhiteSpace(detectedDiseaseName))
        {
            return null;
        }
    
        var diseaseEntity = await _unitOfWork.Diseases.GetByNameAsync(detectedDiseaseName, cancellationToken);
    
        if (diseaseEntity == null)
        {
            // todo: expand
            diseaseEntity = new Disease(
                name: detectedDiseaseName,
                scientificName: "Unknown",
                description: "Auto-generated from AI analysis",
                defaultRecommendations: "See specific AI analysis recommendations.",
                isContagious: false, 
                lethalityIndex: 1
            );

            _unitOfWork.Diseases.Add(diseaseEntity); 
        }

        return diseaseEntity;
    }
}