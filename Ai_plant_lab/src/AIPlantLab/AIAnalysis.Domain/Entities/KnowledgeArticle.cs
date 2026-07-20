namespace AIAnalysis.Domain.Entities;

public sealed class KnowledgeArticle
{
    public Guid Id { get; private set; }
    
    public required string DiseaseName { get; init; }
    
    public required string Description { get; init; }
    
    public required string Recommendation { get; init; }
    
    public required ReadOnlyMemory<float> Embedding { get; init; }

    public KnowledgeArticle(
        string diseaseName,
        string description,
        string recommendation,
        ReadOnlyMemory<float> embedding)
    {
        Id = Guid.NewGuid();
        DiseaseName = diseaseName;
        Description = description;
        Recommendation = recommendation;
        Embedding = embedding;
    }
}