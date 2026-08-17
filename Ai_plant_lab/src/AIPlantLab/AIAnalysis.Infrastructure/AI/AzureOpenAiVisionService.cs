using System.Text.Json;
using AIAnalysis.Application.DTOs;
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

    private readonly ChatClient _miniChatClient;
    private readonly ChatClient _visionChatClient;
    private readonly AzureOpenAiSettings _azureOpenAiSettings;
    // TODO: add real logging
    private readonly ILogger<AzureOpenAiVisionService> _logger;

    private sealed record GuardrailResponse(bool IsValidPlantImage, string? RejectionReason);

    public AzureOpenAiVisionService(
        AzureOpenAIClient azureClient, 
        ILogger<AzureOpenAiVisionService> logger,
        IOptionsSnapshot<AzureOpenAiSettings> options)
    {
        _azureOpenAiSettings = options.Value;
        
        _miniChatClient = azureClient.GetChatClient(_azureOpenAiSettings.MiniDeploymentName);
        _visionChatClient = azureClient.GetChatClient(_azureOpenAiSettings.DeploymentName);
        
        _logger = logger;
    }

    public async Task<Result<AiAnalysisResponseDto>> AnalyzePlantPhotoAsync(
        byte[] photoData,
        CancellationToken cancellationToken = default)
    {
        if (photoData == null || photoData.Length == 0)
        {
            return Result<AiAnalysisResponseDto>.ErrorResult("Photo data cannot be empty.");
        }

        try
        {
            var imageBinaryData = BinaryData.FromBytes(photoData);

            var guardrailResult = await RunGuardrailCheckAsync(imageBinaryData, cancellationToken);
            if (guardrailResult.IsFailure)
            {
                return Result<AiAnalysisResponseDto>.ErrorResult(guardrailResult.Error);
            }

            if (!guardrailResult.Value.IsValidPlantImage)
            {
                var reason = guardrailResult.Value.RejectionReason ?? "There are no plants on the photo";
                
                return Result<AiAnalysisResponseDto>.ErrorResult(reason);
            }

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

            ChatCompletion response = await _visionChatClient.CompleteChatAsync(messages, options, cancellationToken);
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

    private async Task<Result<GuardrailResponse>> RunGuardrailCheckAsync(
        BinaryData imageBinaryData, 
        CancellationToken cancellationToken)
    {
        try
        {
            var messages = new ChatMessage[]
            {
                ChatMessage.CreateSystemMessage(_azureOpenAiSettings.GuardrailSystemPrompt),
                ChatMessage.CreateUserMessage(
                    ChatMessageContentPart.CreateImagePart(imageBinaryData, "image/jpeg")
                )
            };

            var options = new ChatCompletionOptions
            {
                Temperature = 0.0f,
                ResponseFormat = ChatResponseFormat.CreateJsonObjectFormat(),
                MaxOutputTokenCount = 100
            };

            ChatCompletion response = await _miniChatClient.CompleteChatAsync(messages, options, cancellationToken);
            var jsonResponse = response.Content[0].Text;

            var result = JsonSerializer.Deserialize<GuardrailResponse>(jsonResponse, JsonOptions);
            if (result == null)
            {
                return Result<GuardrailResponse>.ErrorResult("Could not parse guardrail response.");
            }

            return Result<GuardrailResponse>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during mini-model guardrail evaluation.");
            return Result<GuardrailResponse>.ErrorResult("Guardrail evaluation failed.");
        }
    }
}