namespace AIAnalysis.Infrastructure.AI;

public class AzureOpenAiSettings
{
    public const string SectionName = "AzureOpenAI";

    public string Endpoint { get; init; } = string.Empty;
    public string ApiKey { get; init; } = string.Empty;
    public string DeploymentName { get; init; } = "gpt-4o";
    
    public string SystemPrompt { get; init; } = string.Empty;
}