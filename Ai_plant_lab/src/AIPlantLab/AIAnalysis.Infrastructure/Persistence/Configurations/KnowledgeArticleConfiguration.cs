using AIAnalysis.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pgvector;

namespace AIAnalysis.Infrastructure.Persistence.Configurations;

public sealed class KnowledgeArticleConfiguration : IEntityTypeConfiguration<KnowledgeArticle>
{
    public void Configure(EntityTypeBuilder<KnowledgeArticle> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.DiseaseName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .IsRequired();

        builder.Property(x => x.Recommendation)
            .IsRequired();

        builder.Property(x => x.Embedding)
            .HasConversion(
                v => new Vector(v.ToArray()),
                v => new ReadOnlyMemory<float>(v.ToArray())
            )
            .HasColumnType("vector(1536)")
            .IsRequired();

        builder.HasIndex(x => x.Embedding)
            .HasMethod("hnsw")
            .HasOperators("vector_cosine_ops");
    }
}