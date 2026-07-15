using AIAnalysis.Domain.Entities;

namespace AIAnalysis.Application.Interfaces.Repositories;

public interface IPlantDiagnosisRepository
{
    void Add(PlantDiagnosis diagnosis);
}