using Spectre.Console;

namespace Mullai.CLI.Commands;

public class QuitCommand : ICommand
{
    public Task ExecuteAsync()
    {
        AnsiConsole.MarkupLine("[yellow]Goodbye![/]");
        Environment.Exit(0);
        return Task.CompletedTask;
    }
}
