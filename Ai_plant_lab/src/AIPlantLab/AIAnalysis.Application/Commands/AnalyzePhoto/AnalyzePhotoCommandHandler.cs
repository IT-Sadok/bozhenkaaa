using AIAnalysis.Application.DTOs;
using AIAnalysis.Application.Interfaces.Repositories;
using AIAnalysis.Application.Interfaces.Services;
using AIAnalysis.Domain.Common;
using AIAnalysis.Domain.Constants;
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
        
        var healthStatus = analyzedData.Status;
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

    private async Task<Disease?> ResolveDiseaseAsync(
        HealthStatus status, 
        DiseaseDetailsDto? detectedDisease, 
        CancellationToken cancellationToken)
    {
        if (status != HealthStatus.DiseaseDetected || detectedDisease == null ||
            string.IsNullOrWhiteSpace(detectedDisease.Name))
        {
            return null;
        }

        var diseaseEntity = await _unitOfWork.Diseases.GetByNameAsync(detectedDisease.Name, cancellationToken);

        if (diseaseEntity == null)
        {
            diseaseEntity = new Disease(
                name: detectedDisease.Name,
                scientificName: DefaultDiseaseConstants.DefaultScientificName,
                description: DefaultDiseaseConstants.DefaultDescription,
                defaultRecommendations: DefaultDiseaseConstants.DefaultRecommendations,
                isContagious: detectedDisease.IsContagious,
                lethalityIndex: detectedDisease.LethalityIndex 
            );

            _unitOfWork.Diseases.Add(diseaseEntity); 
        }

        return diseaseEntity;
    }
}