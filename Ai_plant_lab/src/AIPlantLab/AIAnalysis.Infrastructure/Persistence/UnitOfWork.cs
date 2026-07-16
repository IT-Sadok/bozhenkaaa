using AIAnalysis.Application.Interfaces.Repositories;
using AIAnalysis.Infrastructure.Persistence.Repositories;

namespace AIAnalysis.Infrastructure.Persistence;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _dbContext;

    public IPlantDiagnosisRepository Diagnoses { get; }
    public IDiseaseRepository Diseases { get; }

    public UnitOfWork(AppDbContext dbContext)
    {
        _dbContext = dbContext;
        Diagnoses = new PlantDiagnosisRepository(_dbContext);
        Diseases = new DiseaseRepository(_dbContext);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
}