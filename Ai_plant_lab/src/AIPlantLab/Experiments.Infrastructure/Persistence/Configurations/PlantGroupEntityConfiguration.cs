using Experiments.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Experiments.Infrastructure.Persistence.Configurations;

public class PlantGroupEntityConfiguration : IEntityTypeConfiguration<PlantGroup>
{
    public void Configure(EntityTypeBuilder<PlantGroup> builder)
    {
        builder.ToTable("plant_groups");

        builder.HasKey(pg => pg.Id);

        builder.Property(pg => pg.Id)
            .ValueGeneratedNever();

        builder.Ignore(pg => pg.DomainEvents);

        builder.Property(pg => pg.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(pg => pg.Species)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(pg => pg.PlantCount)
            .IsRequired();
    }
}
