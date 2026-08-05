using AIAnalysis.Application.DTOs;
using AIAnalysis.Application.Interfaces.Services;
using AIAnalysis.Domain.Common;

namespace AIAnalysis.Infrastructure.AI;

public class FakeAiVisionService : IAiVisionService
{
    public Task<Result<AiAnalysisResponseDto>> AnalyzePlantPhotoAsync(byte[] photoData,
        CancellationToken cancellationToken = default)
    {
        var fakeResponse = new AiAnalysisResponseDto(
            "Fake Mildew (Mock)",
            0.99,
            "Mock advice: Apply fungicide.",
            "DiseaseDetected",
            true,
            50
        );

        return Task.FromResult(Result<AiAnalysisResponseDto>.Success(fakeResponse));
    }
}