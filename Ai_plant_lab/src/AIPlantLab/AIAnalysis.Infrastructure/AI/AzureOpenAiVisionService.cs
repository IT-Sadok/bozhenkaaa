using System.Text.Json;
using AIAnalysis.Application.DTOs;
using AIAnalysis.Application.Interfaces;
using AIAnalysis.Domain.Common;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Logging;
using OpenAI.Chat;

namespace AIAnalysis.Infrastructure.AI;

public sealed class AzureOpenAiVisionService : IAiVisionService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private const string AgronomistSystemPrompt = """
                                                  You are an expert plant pathologist and agronomist. 
                                                  Analyze the provided plant photo to diagnose diseases, pests, or nutrient deficiencies.
                                                  You MUST respond strictly with a valid JSON object matching the exact schema below:
                                                  {
                                                      "DetectedDisease": "string (Name of the disease or 'Healthy')",
                                                      "ConfidenceScore": number (between 0.0 and 1.0),
                                                      "Recommendations": "string (Step-by-step treatment or care advice)",
                                                      "IsHealthy": boolean
                                                  }
                                                  Do not include any additional text, markdown formatting, or explanations outside the JSON structure.
                                                  """;

    private readonly ChatClient _chatClient;
    
    // TODO: add real logging
    private readonly ILogger<AzureOpenAiVisionService> _logger;

    public AzureOpenAiVisionService(AzureOpenAIClient azureClient, ILogger<AzureOpenAiVisionService> logger)
    {
        _chatClient = azureClient.GetChatClient("gpt-4o");
        _logger = logger;
    }

    public async Task<Result<DiagnosisResultDto>> AnalyzePlantPhotoAsync(
        byte[] photoData,
        CancellationToken cancellationToken = default)
    {
        if (photoData == null || photoData.Length == 0)
        {
            return Result<DiagnosisResultDto>.ErrorResult("Photo data cannot be empty.");
        }

        try
        {
            var imageBinaryData = BinaryData.FromBytes(photoData);

            var messages = new ChatMessage[]
            {
                ChatMessage.CreateSystemMessage(AgronomistSystemPrompt),
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

            var resultDto = JsonSerializer.Deserialize<DiagnosisResultDto>(jsonResponse, JsonOptions);

            if (resultDto == null)
            {
                _logger.LogWarning("AI returned empty or unparseable JSON: {RawResponse}", jsonResponse);
                return Result<DiagnosisResultDto>.ErrorResult("Could not parse the diagnosis result from AI.");
            }

            _logger.LogInformation("Successfully diagnosed plant. Disease: {Disease}, Confidence: {Score}", 
                resultDto.DetectedDisease, resultDto.ConfidenceScore);

            return Result<DiagnosisResultDto>.Success(resultDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to analyze plant photo due to an unexpected AI service error.");
            
            return Result<DiagnosisResultDto>.ErrorResult("An error occurred while processing the image with AI.");
        }
    }
}