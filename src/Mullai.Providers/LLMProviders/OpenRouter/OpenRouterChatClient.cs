using Mullai.Providers.Common.Http;

namespace Mullai.Providers.LLMProviders.OpenRouter;

public class OpenRouterChatClient : BaseHttpChatClient<OpenRouterRequest, OpenRouterResponse, OpenRouterStreamingUpdate>
{
    public OpenRouterChatClient(HttpClient httpClient, Uri endpointUri) 
        : base(httpClient, endpointUri, new OpenRouterAdapter(), null)
    {
    }
}
