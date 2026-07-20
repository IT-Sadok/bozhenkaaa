using AIAnalysis.Application.Interfaces.Repositories;
using AIAnalysis.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AIAnalysis.Infrastructure.Persistence.Repositories;

public sealed class DiseaseRepository : IDiseaseRepository
{
    private readonly AppDbContext _dbContext;

    public DiseaseRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Disease?> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        return await _dbContext.Diseases
            .FirstOrDefaultAsync(d => d.Name == name, cancellationToken);
    }
}