// -----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2026 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
// -----------------------------------------------------------------------

using System.Windows;
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
    public static MediatorMessage DisplayError
    { get; } = new (nameof(DisplayError), typeof(Action<ActionResult>));

    /// <summary>
    /// Gets a message requesting that the input listener be either enabled or disabled.
    /// </summary>
    /// <remarks>A parameter value of false will disable the listener, whereas true will enable the listener.</remarks>
    public static MediatorMessage ChangeListenerStatus
    { get; } = new(nameof(ChangeListenerStatus), typeof(Action<bool>));

    /// <summary>
    /// Gets a message requesting to show the Command Prompt.
    /// </summary>
    public static MediatorMessage ShowPrompt
    { get; } = new(nameof(ShowPrompt), typeof(Action));

    /// <summary>
    /// Gets a message indicating that the key combination for launching the Command Prompt changed.
    /// </summary>
    public static MediatorMessage PromptKeysChanged
    { get; } = new(nameof(PromptKeysChanged), typeof(Action));

    /// <summary>
    /// Gets a message indicating that the mouse was clicked...somewhere...anywhere.
    /// </summary>
    public static MediatorMessage MouseClicked
    { get; } = new(nameof(MouseClicked), typeof(Action<Point>));

    /// <summary>
    /// Gets a message indicating that the user has pressed Alt+Tab.
    /// </summary>
    public static MediatorMessage AltTabbed
    { get; } = new(nameof(AltTabbed), typeof(Action));
}
