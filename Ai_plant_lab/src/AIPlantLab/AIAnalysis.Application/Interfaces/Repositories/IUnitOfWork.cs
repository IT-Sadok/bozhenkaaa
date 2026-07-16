namespace AIAnalysis.Application.Interfaces.Repositories;

public interface IUnitOfWork
{
    IPlantDiagnosisRepository Diagnoses { get; }
    IDiseaseRepository Diseases { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}