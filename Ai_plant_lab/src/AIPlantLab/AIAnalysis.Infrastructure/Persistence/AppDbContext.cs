using AIAnalysis.Application.Interfaces;
using AIAnalysis.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AIAnalysis.Infrastructure.Persistence;

public sealed class AppDbContext : DbContext, IAppDbContext
{
    public DbSet<PlantDiagnosis> Diagnoses => Set<PlantDiagnosis>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PlantDiagnosis>(builder =>
        {
            builder.ToTable("diagnoses");
            builder.HasKey(p => p.Id);
            
            builder.Ignore(p => p.DomainEvents);
            
            builder.Property(p => p.DetectedDisease).HasMaxLength(200);
            builder.Property(p => p.ImageUrl).HasMaxLength(500);
        });
    }
}