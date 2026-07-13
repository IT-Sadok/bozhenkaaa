using AIAnalysis.Domain.Common;
using AIAnalysis.Domain.Events;

namespace AIAnalysis.Domain.Entities;

public sealed class PlantDiagnosis : Entity
{
    public Guid Id { get; }
    
    public Guid ExperimentId { get; private set; } 
    
    public string ImageUrl { get; private set; }
    
    public string? DetectedDisease { get; }
    
    public double ConfidenceScore { get; private set; }
    
    public string Recommendations { get; }
    
    public DateTime CreatedAt { get; private set; }
    
    public bool IsHealthy { get; }

    public PlantDiagnosis(Guid experimentId, string imageUrl, string detectedDisease, double confidenceScore,
        string recommendations, bool isHealthy)
    {
        Id = Guid.NewGuid();
        ExperimentId = experimentId;
        ImageUrl = imageUrl;
        DetectedDisease = detectedDisease;
        ConfidenceScore = confidenceScore;
        Recommendations = recommendations;
        IsHealthy = isHealthy;
        CreatedAt = DateTime.UtcNow;
        if (!IsHealthy)
        {
            AddDomainEvent(new DiseaseDetectedDomainEvent(Id, DetectedDisease, Recommendations));
        }
    }
}