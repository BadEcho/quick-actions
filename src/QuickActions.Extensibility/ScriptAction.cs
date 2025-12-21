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

namespace BadEcho.QuickActions.Extensibility;

/// <summary>
/// Provides an action that executes a script residing on the file system.
/// </summary>
public sealed class ScriptAction : IAction
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
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public override string? ToString()
        => $"{Name} (Script)";
}
