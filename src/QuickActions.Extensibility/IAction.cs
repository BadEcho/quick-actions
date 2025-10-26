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
/// Defines an action executed by...well, Quick Actions!
/// </summary>
public interface IAction
{
    /// <summary>
    /// Get the unique identifier of the action.
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// Gets the name of the action.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Executes the action.
    /// </summary>
    /// <returns>Value indicating the success of the action's execution.</returns>
    bool Execute();
}
