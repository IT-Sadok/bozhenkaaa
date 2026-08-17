using AIAnalysis.Domain.Entities;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace AIAnalysis.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<PlantDiagnosis> Diagnoses => Set<PlantDiagnosis>();

    public DbSet<Disease> Diseases => Set<Disease>();
    
    public DbSet<KnowledgeArticle> KnowledgeArticles => Set<KnowledgeArticle>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.HasPostgresExtension("vector");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        
        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
    }
}