using Microsoft.Extensions.AI;
using Mullai.Providers.Common.Http;
using Mullai.Providers.Common.Models;

namespace Mullai.Providers.LLMProviders.OpenRouter;

public class OpenRouterAdapter : IProviderAdapter<OpenRouterRequest, OpenRouterResponse, OpenRouterStreamingUpdate>
{
    public OpenRouterRequest MapRequest(IEnumerable<ChatMessage> messages, ChatOptions? options, bool isStreaming)
    {
        return new OpenRouterRequest
        {
            Model = options?.ModelId ?? "google/gemini-2.0-flash-001",
            Messages = messages.Select(m => new OpenRouterChatMessage(
                m.Role.ToString().ToLowerInvariant(), 
                m.Text ?? string.Empty)).ToList(),
            Temperature = options?.Temperature,
            MaxTokens = options?.MaxOutputTokens,
            Stream = isStreaming
        };
    }

    public ChatResponse MapResponse(OpenRouterResponse response)
    {
        var firstChoice = response.Choices.FirstOrDefault();
        var message = new ChatMessage(
            firstChoice?.Message.Role == "assistant" ? ChatRole.Assistant : ChatRole.User,
            firstChoice?.Message.Content ?? string.Empty);

        return new ChatResponse(message)
        {
            ResponseId = response.Id,
            ModelId = response.Model,
            Usage = response.Usage != null ? new UsageDetails
            {
                InputTokenCount = response.Usage.PromptTokens,
                OutputTokenCount = response.Usage.CompletionTokens,
                TotalTokenCount = response.Usage.TotalTokens
            } : null
        };
    }

    public ChatResponseUpdate MapStreamingUpdate(OpenRouterStreamingUpdate streamUpdate)
    {
        var firstChoice = streamUpdate.Choices.FirstOrDefault();
        var role = firstChoice?.Delta.Role switch
        {
            "assistant" => ChatRole.Assistant,
            "system" => ChatRole.System,
            _ => ChatRole.User
        };

        return new ExtendedChatResponseUpdate
        {
            Role = role,
            Contents = new List<AIContent> { new TextContent(firstChoice?.Delta.Content ?? string.Empty) },
            ResponseId = streamUpdate.Id,
        };
    }
}
