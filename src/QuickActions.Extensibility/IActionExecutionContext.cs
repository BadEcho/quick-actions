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
/// Defines the context that drives an action's execution.
/// </summary>
public interface IActionExecutionContext
{
    /// <summary>
    /// Gets or sets a serialized set of additional configuration for the action.
    /// </summary>
    string? ActionConfiguration { get; set; }
}
