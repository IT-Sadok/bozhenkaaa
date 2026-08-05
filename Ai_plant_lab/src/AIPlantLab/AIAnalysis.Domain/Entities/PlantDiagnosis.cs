using System.Diagnostics.CodeAnalysis;
using AIAnalysis.Domain.Common;
using AIAnalysis.Domain.Enums;
using AIAnalysis.Domain.Events;

namespace AIAnalysis.Domain.Entities;

public sealed class PlantDiagnosis : Entity
{
    public Guid Id { get; private init; }
    
    public required Guid ExperimentId { get; init; } 
    
    public required string ImageUrl { get; init; }
    
    public Guid? DetectedDiseaseId { get; private init; }
    
    public Disease? DetectedDisease { get; private set; }
    
    public required double ConfidenceScore { get; init; }
    
    public required string Recommendations { get; init; }
    
    public DateTime CreatedAt { get; private init; }
    
    public required HealthStatus Status { get; init; }
    
    private PlantDiagnosis() 
    { 
    }

    [SetsRequiredMembers]
    public PlantDiagnosis(
        Guid experimentId,
        string imageUrl,
        Guid? detectedDiseaseId,
        double confidenceScore,
        string recommendations,
        HealthStatus status, 
        Disease? detectedDisease)
    {
        Id = Guid.NewGuid();
        ExperimentId = experimentId;
        ImageUrl = imageUrl;
        DetectedDiseaseId = detectedDiseaseId;
        ConfidenceScore = confidenceScore;
        Recommendations = recommendations;
        Status = status;
        DetectedDisease = detectedDisease;
        CreatedAt = DateTime.UtcNow;

        if (Status == HealthStatus.DiseaseDetected && DetectedDiseaseId.HasValue)
        {
            AddDomainEvent(new DiseaseDetectedEvent(
                DiagnosisId: Id,
                ExperimentId: ExperimentId,
                DetectedDiseaseId: DetectedDiseaseId.Value,
                DiseaseName: DetectedDisease?.Name ?? "Unknown Disease",
                Recommendations: Recommendations,
                IsContagious: DetectedDisease?.IsContagious ?? false,
                LethalityIndex: DetectedDisease?.LethalityIndex ?? 0
            ));
        }
    }
}