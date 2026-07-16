using AIAnalysis.Application.Interfaces.Repositories;

namespace AIAnalysis.Infrastructure.Persistence;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _dbContext;

    public IPlantDiagnosisRepository Diagnoses { get; }
    public IDiseaseRepository Diseases { get; }

    public UnitOfWork(
        AppDbContext dbContext,
        IPlantDiagnosisRepository diagnoses,
        IDiseaseRepository diseases)
    {
        _dbContext = dbContext;
        Diagnoses = diagnoses;
        Diseases = diseases;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
}