using AIAnalysis.Domain.Enums;

namespace AIAnalysis.Application.DTOs;

public record AiAnalysisResponseDto(
    DiseaseDetailsDto? DetectedDisease, 
    double ConfidenceScore, 
    string Recommendations,
    HealthStatus Status
);