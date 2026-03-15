using Mullai.CLI.Controllers;
using Mullai.CLI.UI;
using Spectre.Console;

namespace Mullai.CLI.Commands;

public class ConfigCommand : ICommand
{
    private readonly ConfigController _configController;

    public ConfigCommand(ConfigController configController)
    {
        _configController = configController;
    }

    public Task ExecuteAsync()
    {
        var view = new ConfigView(_configController);
        view.Show();
        
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule("[yellow]Mullai - AI Chat Console[/]").RuleStyle("grey").Justify(Justify.Left));
        AnsiConsole.MarkupLine("[grey]Type [bold white]/quit[/] to exit.[/]");
        AnsiConsole.WriteLine();
        
        return Task.CompletedTask;
    }
}
