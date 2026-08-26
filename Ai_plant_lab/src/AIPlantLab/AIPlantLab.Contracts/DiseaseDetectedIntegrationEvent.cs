namespace AIPlantLab.Contracts;

public sealed record DiseaseDetectedIntegrationEvent(
    Guid DiagnosisId,
    Guid ExperimentId,
    Guid DetectedDiseaseId,
    string DiseaseName,
    string Recommendations,
    bool IsContagious,
    decimal LethalityIndex,
    DateTime OccurredAt);
