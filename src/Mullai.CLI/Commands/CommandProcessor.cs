using Mullai.CLI.Controllers;
using Mullai.CLI.State;

namespace Mullai.CLI.Commands;

public class CommandProcessor
{
    private readonly ChatOrchestrator _chatController;
    private readonly ConfigController _configController;
    private readonly ChatState _state;

    public CommandProcessor(ChatOrchestrator chatController, ConfigController configController, ChatState state)
    {
        _chatController = chatController;
        _configController = configController;
        _state = state;
    }

    public ICommand GetCommand(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return null!;

        if (input.Equals("/quit", StringComparison.OrdinalIgnoreCase))
        {
            return new QuitCommand();
        }

        if (input.Equals("/config", StringComparison.OrdinalIgnoreCase))
        {
            return new ConfigCommand(_configController);
        }

        return new MessageCommand(input, _chatController, _state);
    }
}
