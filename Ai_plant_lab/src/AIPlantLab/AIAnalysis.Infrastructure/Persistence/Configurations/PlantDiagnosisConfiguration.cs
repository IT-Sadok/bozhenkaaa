using AIAnalysis.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIAnalysis.Infrastructure.Persistence.Configurations;

public class PlantDiagnosisConfiguration : IEntityTypeConfiguration<PlantDiagnosis>
{
    public void Configure(EntityTypeBuilder<PlantDiagnosis> builder)
    {
        builder.ToTable("diagnoses");
        
        builder.HasKey(p => p.Id);
        
        builder.Ignore(p => p.DomainEvents);

        builder.Property(p => p.ImageUrl)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(p => p.ConfidenceScore)
            .IsRequired();

        builder.Property(p => p.Status)
            .IsRequired();

        builder.HasOne(p => p.DetectedDisease)
            .WithMany() 
            .HasForeignKey(p => p.DetectedDiseaseId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}