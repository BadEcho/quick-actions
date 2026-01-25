// -----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2025 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics;
using BadEcho.Extensions;
using BadEcho.QuickActions.Extensibility;

namespace BadEcho.QuickActions;

/// <summary>
/// Provides an action that executes a script residing on the file system.
/// </summary>
internal sealed class ScriptAction : IAction
{
    /// <inheritdoc/>
    /// <remarks>Script actions generate their own ID when initially created.</remarks>
    public Guid Id
    { get; init; } = Guid.NewGuid();

    /// <inheritdoc/>
    public string Name 
    { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the type of shell program whose script interpreter is used to execute the action's script.
    /// </summary>
    public ShellType ShellType 
    { get; set; }
    
    /// <summary>
    /// Gets or sets the path to the action's script.
    /// </summary>
    public string Path 
    { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the arguments passed to the action's script.
    /// </summary>
    public string Arguments 
    { get; set; } = string.Empty;

    /// <summary>
    /// Executes the action's script.
    /// </summary>
    /// <returns>Value indicating if the action's script executed successfully.</returns>
    public bool Execute()
    {
        var startInfo = ShellType switch
        {
            ShellType.Command => new ProcessStartInfo
                                 {
                                     FileName = "cmd.exe",
                                     Arguments = $"/c \"\"{Path}\" {Arguments}\""
                                 },
            ShellType.PowerShell => new ProcessStartInfo
                                    {
                                        FileName = "powershell.exe",
                                        Arguments = $"-NoProfile -ExecutionPolicy Bypass -File \"{Path}\" {Arguments}",
                                    },
            _ => throw new InvalidOperationException("Invalid shell type for action.")
        };

        return ExecuteScript(startInfo);
    }

    /// <inheritdoc/>
    public override string ToString()
        => $"{Name} (Script)";

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj is not ScriptAction other)
            return false;

        return Id == other.Id;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
        => this.GetHashCode(Id);

    private static bool ExecuteScript(ProcessStartInfo startInfo)
    {
        startInfo.UseShellExecute = false;
        startInfo.RedirectStandardError = true;
        startInfo.CreateNoWindow = true;
        
        using (var process = Process.Start(startInfo))
        {
            if (process == null)
                return false;

            string errors = process.StandardError.ReadToEnd();
            process.WaitForExit();

            return string.IsNullOrEmpty(errors);
        }
    }
}
