using AIAnalysis.Domain.Entities;

namespace AIAnalysis.Application.Interfaces.Repositories;

public interface IDiseaseRepository
{
    Task<Disease?> GetByNameAsync(string name, CancellationToken cancellationToken);
}