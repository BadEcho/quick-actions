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
/// Provides an action defined in code, imported as a plugin.
/// </summary>
public abstract class CodeAction : IAction
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CodeAction"/> class.
    /// </summary>
    /// <param name="id">The unique identifier of the action.</param>
    /// <param name="name">The name of the action.</param>
    protected CodeAction(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    /// <inheritdoc/>
    public Guid Id 
    { get; }

    /// <inheritdoc/>
    public string Name 
    { get; }

    /// <summary>
    /// Gets a description from the plugin as to what the action does.
    /// </summary>
    public abstract string Description { get; }

    /// <inheritdoc/>
    public abstract bool Execute();
}
