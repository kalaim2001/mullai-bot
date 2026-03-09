using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Mullai.Providers.LLMProviders.Cerebras;

public static class Cerebras
{
    public static IServiceCollection AddCerebrasChatClient(
        this IServiceCollection services, 
        IConfiguration configuration,
        ILoggerFactory loggerFactory,
        HttpClient httpClient
        )
    {
        var chatClient = GetCerebrasChatClient(configuration, loggerFactory, httpClient);

        services.AddSingleton<IChatClient>(chatClient);

        return services;
    }

    public static IChatClient GetCerebrasChatClient(
        IConfiguration configuration,
        ILoggerFactory loggerFactory,
        HttpClient httpClient
    )
    {
        return OpenAICompatibleProvider.CreateChatClient(
            "Cerebras", 
            "https://api.cerebras.ai/v1/", 
            configuration, 
            httpClient);
    }
}
