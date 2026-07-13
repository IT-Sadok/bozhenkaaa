namespace AIAnalysis.Domain.Entities;

public sealed class KnowledgeArticle
{
    public Guid Id { get; private set; }
    
    public string DiseaseName { get; private set; }
    
    public string Description { get; private set; }
    
    public string Recommendation { get; private set; }
    
    public ReadOnlyMemory<float> Embedding { get; private set; }

    public KnowledgeArticle(string diseaseName, string description, string recommendation, ReadOnlyMemory<float> embedding)
    {
        Id = Guid.NewGuid();
        DiseaseName = diseaseName;
        Description = description;
        Recommendation = recommendation;
        Embedding = embedding;
    }
}