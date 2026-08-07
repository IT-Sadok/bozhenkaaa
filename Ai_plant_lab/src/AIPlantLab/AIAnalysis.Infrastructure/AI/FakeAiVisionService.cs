using AIAnalysis.Application.DTOs;
using AIAnalysis.Application.Interfaces.Services;
using AIAnalysis.Domain.Common;
using AIAnalysis.Domain.Enums;

namespace AIAnalysis.Infrastructure.AI;

public class FakeAiVisionService : IAiVisionService
{
    public Task<Result<AiAnalysisResponseDto>> AnalyzePlantPhotoAsync(byte[] photoData,
        CancellationToken cancellationToken = default)
    {
        var fakeResponse = new AiAnalysisResponseDto(
            DetectedDisease: new DiseaseDetailsDto(
                Name: "Fake Mildew (Mock)",
                IsContagious: true,
                LethalityIndex: 0.50m,
                ContaminationDetails: new ContaminationInfo(
                    Sources: ["Infected plant debris", "Contaminated soil"],
                    TransmissionMethods: ["Airborne spores", "Water splashing"]
                )
            ),
            ConfidenceScore: 0.99,
            Recommendations: "Mock advice: Apply fungicide immediately and isolate the plant.",
            Status: HealthStatus.DiseaseDetected
        );

        return Task.FromResult(Result<AiAnalysisResponseDto>.Success(fakeResponse));
    }
}