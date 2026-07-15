namespace AIAnalysis.Application.DTOs;

public record AiAnalysisResponseDto(
    string DetectedDisease,
    double ConfidenceScore,
    string Recommendations,
    string Status // Повертається як рядок (наприклад, "DiseaseDetected" або "Healthy")
);