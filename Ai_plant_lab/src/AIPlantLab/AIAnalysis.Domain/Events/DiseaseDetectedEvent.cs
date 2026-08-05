namespace AIAnalysis.Domain.Events;

public record DiseaseDetectedEvent(
    Guid DiagnosisId,
    Guid ExperimentId,
    Guid DetectedDiseaseId,
    string DiseaseName,
    string Recommendations,
    bool IsContagious,
    decimal LethalityIndex
) : IDomainEvent;