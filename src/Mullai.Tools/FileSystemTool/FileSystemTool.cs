using Microsoft.Extensions.AI;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Mullai.Tools.FileSystemTool;

/// <summary>
/// The agent plugin that provides file system capabilities.
/// </summary>
[Description("A tool for performing file system operations like reading from and writing to files.")]
public class FileSystemTool(FileSystemProvider fileSystemProvider)
{
    /// <summary>
    /// Reads the content of a file.
    /// </summary>
    /// <param name="filePath">The absolute or relative path to the file.</param>
    /// <returns>The content of the file or an error message.</returns>
    [Description("Reads the entire content of a specified file and returns it as a string.")]
    public async Task<string> ReadFileSystemFile(
        [Description("The absolute or relative path to the file to be read.")] string filePath)
    {
        return await fileSystemProvider.ReadFileAsync(filePath);
    }

    /// <summary>
    /// Writes content to a file, overwriting the existing file or creating a new one.
    /// </summary>
    /// <param name="filePath">The absolute or relative path to the file.</param>
    /// <param name="content">The content to write to the file.</param>
    /// <returns>A success or error message.</returns>
    [Description("Writes the provided content to a file. Warning: This will overwrite the existing file or create a new one if it does not exist.")]
    public async Task<string> WriteFileSystemFile(
        [Description("The absolute or relative path to the file where content will be written.")] string filePath,
        [Description("The text content to be written to the file.")] string content)
    {
        return await fileSystemProvider.WriteFileAsync(filePath, content);
    }

    /// <summary>
    /// Returns the functions provided by this plugin.
    /// </summary>
    /// <returns>The functions provided by this plugin.</returns>
    public IEnumerable<AITool> AsAITools()
    {
        yield return AIFunctionFactory.Create(this.ReadFileSystemFile);
        yield return AIFunctionFactory.Create(this.WriteFileSystemFile);
    }
}
