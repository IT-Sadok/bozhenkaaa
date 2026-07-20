using AIAnalysis.Application.Interfaces;
using AIAnalysis.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AIAnalysis.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<PlantDiagnosis> Diagnoses => Set<PlantDiagnosis>();

    public DbSet<Disease> Diseases => Set<Disease>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}