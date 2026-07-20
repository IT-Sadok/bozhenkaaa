using AIAnalysis.Application.Interfaces.Repositories;
using AIAnalysis.Domain.Entities;

namespace AIAnalysis.Infrastructure.Persistence.Repositories;

public sealed class PlantDiagnosisRepository : IPlantDiagnosisRepository
{
    private readonly AppDbContext _dbContext;

    public PlantDiagnosisRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(PlantDiagnosis diagnosis)
    {
        _dbContext.Diagnoses.Add(diagnosis);
    }
}