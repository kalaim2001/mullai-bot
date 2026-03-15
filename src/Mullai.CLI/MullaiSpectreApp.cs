using Microsoft.Extensions.DependencyInjection;
using Mullai.Agents;
using Mullai.Abstractions.Configuration;
using Mullai.Abstractions.Observability;
using Mullai.CLI.Controllers;
using Mullai.CLI.State;
using Mullai.CLI.Commands;
using Mullai.Middleware.Middlewares;
using Spectre.Console;

namespace Mullai.CLI;

public class MullaiSpectreApp
{
    private readonly IServiceProvider _services;
    private readonly ChatState _state;
    private readonly ChatOrchestrator _controller;
    private readonly ConfigController _configController;
    private readonly CommandProcessor _commandProcessor;

    public MullaiSpectreApp(IServiceProvider services)
    {
        _services = services;
        _state = new ChatState();
        var agentFactory = _services.GetRequiredService<AgentFactory>();
        var chatClient = _services.GetRequiredService<Microsoft.Extensions.AI.IChatClient>();
        _controller = new ChatOrchestrator(agentFactory, _state, chatClient);
        var credentialStorage = _services.GetRequiredService<ICredentialStorage>();
        _configController = new ConfigController(credentialStorage);
        _commandProcessor = new CommandProcessor(_controller, _configController, _state);

        // Wire the FunctionCallingMiddleware to emit tool call observations
        // into the singleton channel.
        var middleware = _services.GetRequiredService<FunctionCallingMiddleware>();
        middleware.OnToolCallObserved = obs => ToolCallChannel.Instance.Writer.TryWrite(obs);
    }

    public async Task RunAsync()
    {
        await _controller.InitialiseAsync();

        AnsiConsole.Clear();
        AnsiConsole
            .Write(new Rule("[DeepPink3_1]Mullai - Your AI Assistant[/]")
                .RuleStyle("grey")
                .Justify(Justify.Center));
        AnsiConsole.MarkupLine("[grey]Type [bold white]/quit[/] to exit.[/]");
        AnsiConsole.MarkupLine("[green]Type [bold white]/config[/] to setup models and api keys.[/]");
        AnsiConsole.WriteLine();

        while (true)
        {
            var input = AnsiConsole.Prompt(
                new TextPrompt<string>("You: ")
                    .PromptStyle("green")
            );

            var command = _commandProcessor.GetCommand(input);
            if (command != null)
            {
                await command.ExecuteAsync();
            }
        }
    }
}
