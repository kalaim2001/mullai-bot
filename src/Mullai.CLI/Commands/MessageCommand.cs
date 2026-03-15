using Mullai.CLI.Controllers;
using Mullai.CLI.State;
using Mullai.CLI.UI;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Mullai.CLI.Commands;

public class MessageCommand : ICommand
{
    private readonly string _input;
    private readonly ChatOrchestrator _controller;
    private readonly ChatState _state;

    public MessageCommand(string input, ChatOrchestrator controller, ChatState state)
    {
        _input = input;
        _controller = controller;
        _state = state;
    }

    public async Task ExecuteAsync()
    {
        var turnStart = DateTimeOffset.Now;

        // Render user message panel
        var userMessage = new ChatMessage(_input, true, turnStart);
        AnsiConsole.Write(new ChatMessageComponent(userMessage).Render());
        AnsiConsole.WriteLine();

        // Handle agent response with live display
        await AnsiConsole.Live(new Markup("[grey]Mullai is thinking...[/]"))
            .AutoClear(false)
            .StartAsync(async ctx =>
            {
                Action updateHandler = () => ctx.UpdateTarget(RenderTurnEntries(turnStart));
                _state.StateChanged += updateHandler;
                
                try 
                {
                    await _controller.HandleMessageAsync(_input);
                }
                finally
                {
                    _state.StateChanged -= updateHandler;
                    ctx.UpdateTarget(RenderTurnEntries(turnStart));
                }
            });
        
        AnsiConsole.WriteLine();
    }

    private IRenderable RenderTurnEntries(DateTimeOffset turnStart)
    {
        var entries = _state.ChronologicalEntries.Where(e => e switch {
            ChatMessage m => !m.IsUser && m.Timestamp > turnStart,
            Mullai.Abstractions.Observability.ToolCallObservation t => t.StartedAt > turnStart,
            _ => false
        }).ToList();

        if (entries.Count == 0 && _state.IsThinking)
        {
            return new Markup("[grey]Mullai is thinking...[/]");
        }

        if (entries.Count == 0)
        {
            return new Markup(string.Empty);
        }

        return new Rows(entries.Select(e => e switch {
            ChatMessage m => new ChatMessageComponent(m).Render(),
            Mullai.Abstractions.Observability.ToolCallObservation t => new ToolCallComponent(t).Render(),
            _ => new Markup(string.Empty)
        }));
    }
}
