namespace AIAnalysis.Application.DTOs;

public record DiagnosisResultDto(
    string DetectedDisease,
    double ConfidenceScore,
    string Recommendations,
    bool IsHealthy
);