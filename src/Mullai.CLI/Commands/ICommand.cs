namespace Mullai.CLI.Commands;

public interface ICommand
{
    Task ExecuteAsync();
}
