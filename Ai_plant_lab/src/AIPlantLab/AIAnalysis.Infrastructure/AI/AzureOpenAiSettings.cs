namespace AIAnalysis.Infrastructure.AI;

public class AzureOpenAiSettings
{
    public const string SectionName = "AzureOpenAI";
    // for testing purposes the same as DefaultDeploymentName
    private const string DefaultMiniDeploymentName = "gpt-4o";
    private const string DefaultDeploymentName = "gpt-4o";

    public required string Endpoint { get; init; } = string.Empty;
    public required string ApiKey { get; init; } = string.Empty;
    public string MiniDeploymentName { get; init; } = DefaultMiniDeploymentName;
    public string DeploymentName { get; init; } = DefaultDeploymentName;
    public string SystemPrompt { get; init; } = string.Empty;
    public string GuardrailSystemPrompt { get; init; } = string.Empty;
}