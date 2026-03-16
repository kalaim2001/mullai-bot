using Microsoft.Agents.AI;
using Mullai.Agents;
using Mullai.Abstractions.Configuration;
using Mullai.CLI.State;
using System.Net.Http;

namespace Mullai.CLI.Controllers;

public class ChatOrchestrator
{
    private readonly AgentFactory _agentFactory;
    private readonly ChatState _state;
    private readonly Microsoft.Extensions.AI.IChatClient _chatClient;
    private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;
    private readonly ICredentialStorage _credentialStorage;
    private readonly HttpClient _httpClient;
    private AIAgent? _agent;
    private AgentSession? _session;

    public ChatOrchestrator(
        AgentFactory agentFactory, 
        ChatState state, 
        Microsoft.Extensions.AI.IChatClient chatClient,
        Microsoft.Extensions.Configuration.IConfiguration configuration,
        ICredentialStorage credentialStorage,
        HttpClient httpClient)
    {
        _agentFactory = agentFactory;
        _state = state;
        _chatClient = chatClient;
        _configuration = configuration;
        _credentialStorage = credentialStorage;
        _httpClient = httpClient;
    }

    public void RefreshClients()
    {
        if (_chatClient is Mullai.Providers.MullaiChatClient mullaiClient)
        {
            var config = Mullai.Providers.MullaiChatClientFactory.LoadConfig();
            var newClients = Mullai.Providers.MullaiChatClientFactory.BuildOrderedClients(
                config, 
                _configuration, 
                _credentialStorage, 
                _httpClient);
            
            mullaiClient.UpdateClients(newClients);
        }
    }

    public string ModelName => GetLabelPart(1);
    public string ProviderName => GetLabelPart(0);

    private string GetLabelPart(int index)
    {
        if (_chatClient is Mullai.Providers.MullaiChatClient mullaiClient)
        {
            var parts = mullaiClient.ActiveLabel.Split('/');
            return parts.Length > index ? parts[index] : "Unknown";
        }
        return "Unknown";
    }

    public async Task InitialiseAsync()
    {
        _agent = _agentFactory.GetAgent("Assistant");
        _session = await _agent.CreateSessionAsync();

        _ = PumpToolCallsAsync();
    }

    private async Task PumpToolCallsAsync()
    {
        await foreach (var observation in ToolCallChannel.Instance.Reader.ReadAllAsync())
        {
            _state.AddToolCall(observation);
        }
    }

    public async Task HandleMessageAsync(string userInput)
    {
        if (_agent is null || _session is null)
            return;

        _state.AddUserMessage(userInput);
        _state.BeginAgentResponse();

        try
        {
            var firstUpdate = true;
            await foreach (var update in _agent.RunStreamingAsync(userInput, _session))
            {
                var text = update?.ToString() ?? string.Empty;
                if (!string.IsNullOrEmpty(text))
                {
                    _state.AppendUpdate(text, firstUpdate);
                    if (firstUpdate) firstUpdate = false;
                }
            }
        }
        catch (Exception ex)
        {
            _state.AddErrorMessage(ex.Message);
        }
        finally
        {
            _state.CompleteAgentResponse();
        }
    }
}
