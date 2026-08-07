using System.Diagnostics.CodeAnalysis;
using AIAnalysis.Domain.Common;

namespace AIAnalysis.Domain.Entities;

public sealed class Disease : Entity
{
    public Guid Id { get; private init; }
    
    public required string Name { get; init; }
    
    public required string ScientificName { get; init; }
    
    public required string Description { get; init; }
    
    public required string DefaultRecommendations { get; init; }
    
    public required bool IsContagious { get; init; }
    
    public required decimal LethalityIndex { get; init; }
    
    private Disease() 
    { 
    }

    [SetsRequiredMembers]
    public Disease(
        string name,
        string scientificName,
        string description,
        string defaultRecommendations,
        bool isContagious = false,
        decimal lethalityIndex = 0)
    {
        Id = Guid.NewGuid();
        Name = name;
        ScientificName = scientificName;
        Description = description;
        DefaultRecommendations = defaultRecommendations;
        IsContagious = isContagious;
        LethalityIndex = lethalityIndex;
    }
}