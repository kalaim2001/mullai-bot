using System.ClientModel;
using System.ClientModel.Primitives;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mullai.OpenTelemetry.OpenTelemetry;
using OpenAI;


namespace Mullai.Providers.LLMProviders.OpenRouter;

public static class OpenRouter
{
    public static IServiceCollection AddOpenRouterChatClient(
        this IServiceCollection services, 
        IConfiguration configuration,
        ILoggerFactory loggerFactory,
        HttpClient httpClient
        )
    {
        var chatClient = GetOpenRouterChatClient(configuration, loggerFactory, httpClient);

        services.AddSingleton<IChatClient>(chatClient);

        return services;
    }

    public static IChatClient GetOpenRouterChatClient(
        IConfiguration configuration,
        HttpClient httpClient,
        string? modelId = null
    )
    {
        var apiKey = configuration["OpenRouter:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new InvalidOperationException("OpenRouter:ApiKey is missing from configuration.");

        var endpoint = configuration["OpenRouter:Endpoint"] ?? "https://openrouter.ai/api/v1/chat/completions";
        
        return new OpenRouterChatClient(httpClient, new Uri(endpoint))
        {
            OnBeforeRequest = req =>
            {
                req.Headers.Add("Authorization", $"Bearer {apiKey}");
                req.Headers.Add("HTTP-Referer", "https://github.com/agentmatters/mullai");
                req.Headers.Add("X-Title", "Mullai Bot");
            }
        };
    }

    [Obsolete("Use GetOpenRouterChatClient(configuration, httpClient, modelId) instead.")]
    public static IChatClient GetOpenRouterChatClient(
        IConfiguration configuration,
        ILoggerFactory loggerFactory,
        HttpClient httpClient
    ) => GetOpenRouterChatClient(configuration, httpClient);
}