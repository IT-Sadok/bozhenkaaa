namespace AIAnalysis.Domain.Events;

public record DiseaseDetectedDomainEvent(
    Guid DiagnosisId, 
    string DiseaseName, 
    string Recommendation) : IDomainEvent;