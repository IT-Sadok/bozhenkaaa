using System.Text.Json;
using AIAnalysis.Application.DTOs;
using AIAnalysis.Application.Interfaces;
using AIAnalysis.Application.Interfaces.Services;
using AIAnalysis.Domain.Common;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenAI.Chat;

namespace AIAnalysis.Infrastructure.AI;

public sealed class AzureOpenAiVisionService : IAiVisionService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
    private readonly ChatClient _chatClient;
    private readonly AzureOpenAiSettings _azureOpenAiSettings;
    
    // TODO: add real logging
    private readonly ILogger<AzureOpenAiVisionService> _logger;

    public AzureOpenAiVisionService(AzureOpenAIClient azureClient, ILogger<AzureOpenAiVisionService> logger,
        IOptionsSnapshot<AzureOpenAiSettings> options)
    {
        _azureOpenAiSettings = options.Value;
        _chatClient = azureClient.GetChatClient(_azureOpenAiSettings.DeploymentName);
        _logger = logger;
    }

    public async Task<Result<AiAnalysisResponseDto>> AnalyzePlantPhotoAsync(byte[] photoData,
        CancellationToken cancellationToken = default)
    {
        if (photoData == null || photoData.Length == 0)
        {
            return Result<AiAnalysisResponseDto>.ErrorResult("Photo data cannot be empty.");
        }

        try
        {
            var imageBinaryData = BinaryData.FromBytes(photoData);

            var messages = new ChatMessage[]
            {
                ChatMessage.CreateSystemMessage(_azureOpenAiSettings.SystemPrompt),
                ChatMessage.CreateUserMessage(
                    ChatMessageContentPart.CreateImagePart(imageBinaryData, "image/jpeg")
                )
            };

            var options = new ChatCompletionOptions
            {
                Temperature = 0.2f,
                ResponseFormat = ChatResponseFormat.CreateJsonObjectFormat()
            };

            _logger.LogInformation("Sending plant photo ({Size} bytes) to Azure OpenAI Vision...", photoData.Length);

            ChatCompletion response = await _chatClient.CompleteChatAsync(messages, options, cancellationToken);
            var jsonResponse = response.Content[0].Text;

            var resultDto = JsonSerializer.Deserialize<AiAnalysisResponseDto>(jsonResponse, JsonOptions);

            if (resultDto == null)
            {
                _logger.LogWarning("AI returned empty or unparseable JSON: {RawResponse}", jsonResponse);
                return Result<AiAnalysisResponseDto>.ErrorResult("Could not parse the diagnosis result from AI.");
            }

            _logger.LogInformation("Successfully diagnosed plant. Disease: {Disease}, Confidence: {Score}", 
                resultDto.DetectedDisease, resultDto.ConfidenceScore);

            return Result<AiAnalysisResponseDto>.Success(resultDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to analyze plant photo due to an unexpected AI service error.");
            
            return Result<AiAnalysisResponseDto>.ErrorResult("An error occurred while processing the image with AI.");
        }
    }
}