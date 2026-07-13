using AIAnalysis.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AIAnalysis.Application.Interfaces;

public interface IAppDbContext
{
    DbSet<PlantDiagnosis> Diagnoses { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}