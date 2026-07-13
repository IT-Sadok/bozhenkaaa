using AIAnalysis.Application.DTOs;
using AIAnalysis.Domain.Common;

namespace AIAnalysis.Application.Interfaces;

public interface IAiVisionService
{
    Task<Result<DiagnosisResultDto>> AnalyzePlantPhotoAsync(byte[] photoData,
        CancellationToken cancellationToken = default);
}