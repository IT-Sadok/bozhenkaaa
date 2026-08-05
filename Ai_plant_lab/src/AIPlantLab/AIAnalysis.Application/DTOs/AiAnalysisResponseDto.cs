namespace AIAnalysis.Application.DTOs;

public record AiAnalysisResponseDto(  
    string DetectedDisease, 
    double ConfidenceScore, 
    string Recommendations,
    string Status,
    bool IsContagious,
    decimal LethalityIndex
);