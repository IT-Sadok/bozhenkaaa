using AIAnalysis.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIAnalysis.Infrastructure.Persistence.Configurations;

public class DiseaseConfiguration : IEntityTypeConfiguration<Disease>
{
    public void Configure(EntityTypeBuilder<Disease> builder)
    {
        builder.ToTable("diseases");
        
        builder.HasKey(d => d.Id);
        
        builder.Ignore(d => d.DomainEvents);

        builder.Property(d => d.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(d => d.ScientificName)
            .HasMaxLength(200);

        builder.Property(d => d.Description)
            .IsRequired();

        builder.Property(d => d.DefaultRecommendations)
            .IsRequired();
    }
}