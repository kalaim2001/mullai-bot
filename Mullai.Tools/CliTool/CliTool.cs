using Microsoft.Extensions.AI;

namespace Mullai.Tools.CliTool;

/// <summary>
/// The agent plugin that provides command-line execution capabilities.
/// </summary>
/// <param name="cliProvider">The CLI provider to execute commands.</param>
public class CliTool(CliProvider cliProvider)
{
    /// <summary>
    /// Executes a command in the system shell and returns its output.
    /// This should be used carefully as it executes on the host machine.
    /// </summary>
    /// <param name="command">The command string to execute.</param>
    /// <returns>The combined standard output and error of the command execution.</returns>
    public async Task<string> ExecuteCommandAsync(string command)
    {
        return await cliProvider.ExecuteCommandAsync(command);
    }

    /// <summary>
    /// Returns the functions provided by this plugin.
    /// </summary>
    /// <returns>The functions provided by this plugin.</returns>
    public IEnumerable<AITool> AsAITools()
    {
        yield return AIFunctionFactory.Create(this.ExecuteCommandAsync);
    }
}
