using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Mullai.Tools.CliTool;

/// <summary>
/// A provider for executing command-line instructions.
/// </summary>
public class CliProvider
{
    /// <summary>
    /// Executes a command in the system shell and returns its standard output and error.
    /// </summary>
    /// <param name="command">The command to execute.</param>
    /// <returns>The standard output and standard error of the command.</returns>
    public async Task<string> ExecuteCommandAsync(string command)
    {
        var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        var shell = isWindows ? "cmd.exe" : "/bin/bash";

        var startInfo = new ProcessStartInfo
        {
            FileName = shell,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        if (isWindows)
        {
            startInfo.ArgumentList.Add("/c");
            startInfo.ArgumentList.Add(command);
        }
        else
        {
            startInfo.ArgumentList.Add("-c");
            startInfo.ArgumentList.Add(command);
        }

        using var process = new Process { StartInfo = startInfo };
        
        try
        {
            process.Start();

            var outputTask = process.StandardOutput.ReadToEndAsync();
            var errorTask = process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync();

            var output = await outputTask;
            var error = await errorTask;

            var result = string.Empty;
            if (!string.IsNullOrWhiteSpace(output))
            {
                result += $"Standard Output:\n{output}\n";
            }
            if (!string.IsNullOrWhiteSpace(error))
            {
                result += $"Standard Error:\n{error}\n";
            }
            
            result += $"Exit Code: {process.ExitCode}";

            return string.IsNullOrWhiteSpace(result) ? "Command executed successfully with no output." : result;
        }
        catch (Exception ex)
        {
            return $"Failed to execute command. Error: {ex.Message}";
        }
    }
}
