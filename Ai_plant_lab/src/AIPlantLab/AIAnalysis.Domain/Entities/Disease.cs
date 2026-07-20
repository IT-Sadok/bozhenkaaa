using System;
using AIAnalysis.Domain.Common;

namespace AIAnalysis.Domain.Entities;

public sealed class Disease : Entity
{
    public Guid Id { get; private init; }
    
    public required string Name { get; init; }
    
    public string? ScientificName { get; init; }
    
    public required string Description { get; init; }
    
    public required string DefaultRecommendations { get; init; }

    public Disease(
        string name,
        string description,
        string defaultRecommendations,
        string? scientificName = null)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        DefaultRecommendations = defaultRecommendations;
        ScientificName = scientificName;
    }
}