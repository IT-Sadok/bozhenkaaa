using AIAnalysis.Application.Interfaces.Repositories;
using MassTransit;

namespace AIAnalysis.Infrastructure.Persistence;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _dbContext;
    private readonly IPublishEndpoint _publishEndpoint;

    public IPlantDiagnosisRepository Diagnoses { get; }
    public IDiseaseRepository Diseases { get; }

    public UnitOfWork(
        AppDbContext dbContext,
        IPlantDiagnosisRepository diagnoses,
        IDiseaseRepository diseases, IPublishEndpoint publishEndpoint)
    {
        _dbContext = dbContext;
        Diagnoses = diagnoses;
        Diseases = diseases;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
}