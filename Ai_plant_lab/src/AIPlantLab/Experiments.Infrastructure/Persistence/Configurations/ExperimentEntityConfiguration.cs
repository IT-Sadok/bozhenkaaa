using Experiments.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Experiments.Infrastructure.Persistence.Configurations;

public class ExperimentEntityConfiguration : IEntityTypeConfiguration<Experiment>
{
    public void Configure(EntityTypeBuilder<Experiment> builder)
    {
        builder.ToTable("experiments");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedNever();

        builder.Ignore(e => e.DomainEvents);

        builder.Property(e => e.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.Description)
            .HasMaxLength(2000);

        builder.Property(e => e.Status)
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.OwnsOne(e => e.Configuration, config =>
        {
            config.Property(c => c.LightHoursPerDay).HasColumnName("light_hours_per_day");
            config.Property(c => c.WateringIntervalDays).HasColumnName("watering_interval_days");
            config.Property(c => c.TargetTemperatureCelsius).HasColumnName("target_temperature_celsius");
            config.Property(c => c.Notes).HasColumnName("configuration_notes").HasMaxLength(2000);
        });

        builder.HasMany(e => e.PlantGroups)
            .WithOne()
            .HasForeignKey(pg => pg.ExperimentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(e => e.PlantGroups)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasField("_plantGroups");
    }
}
