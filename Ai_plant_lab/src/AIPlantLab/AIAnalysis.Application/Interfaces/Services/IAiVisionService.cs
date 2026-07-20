using AIAnalysis.Application.DTOs;
using AIAnalysis.Domain.Common;

namespace AIAnalysis.Application.Interfaces.Services;

public interface IAiVisionService
{
    Task<Result<AiAnalysisResponseDto>> AnalyzePlantPhotoAsync(byte[] photoData,
        CancellationToken cancellationToken = default);
}