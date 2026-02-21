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

using BadEcho.Presentation.Messaging;
using BadEcho.QuickActions.Extensibility;

namespace BadEcho.QuickActions;

/// <summary>
/// Provides a collection of mediator messages various Quick Actions components use to communicate with each other.
/// </summary>
internal static class Messages
{
    /// <summary>
    /// Gets a message requesting that an error notification be displayed.
    /// </summary>
    public static MediatorMessage DisplayErrorRequested
    { get; } = new (nameof(DisplayErrorRequested), typeof(Action<ActionResult>));

    /// <summary>
    /// Gets a message requesting that all keyboard monitoring be paused.
    /// </summary>
    public static MediatorMessage PauseListener
    { get; } = new(nameof(PauseListener), typeof(Action));

    /// <summary>
    /// Gets a message requesting that all keyboard monitoring be resumed.
    /// </summary>
    public static MediatorMessage ResumeListener
    { get; } = new(nameof(ResumeListener), typeof(Action));
}
