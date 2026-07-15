using AIAnalysis.Domain.Enums;

namespace AIAnalysis.Application.DTOs;

public record DiagnosisResultDto(
    Guid DiagnosisId,          // ID самого діагнозу
    Guid? DetectedDiseaseId,   // ID хвороби з нашого довідника (null, якщо рослина здорова)
    string DetectedDisease,    // Назва хвороби
    double ConfidenceScore,
    string Recommendations,
    HealthStatus Status        // Наш новий Enum статус
);