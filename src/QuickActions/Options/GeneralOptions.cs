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

namespace BadEcho.QuickActions.Options;

/// <summary>
/// Provides the user's configuration for general application settings.
/// </summary>
internal sealed class GeneralOptions
{
    /// <summary>
    /// Gets or sets a value indicating if the application should minimize to the system tray when the
    /// main window is closed.
    /// </summary>
    public bool MinimizeToTrayOnClose
    { get; set; }

    /// <summary>
    /// Gets or sets a value indicating if the application should be minimized to the system tray on launch.
    /// </summary>
    public bool StartMinimized
    { get; set; }

    /// <summary>
    /// Gets or sets a value indicating if mapped actions are enabled globally.
    /// </summary>
    public bool ActionsEnabled
    { get; set; } = true;

    /// <summary>
    /// Gets or sets the key combination that, when pressed, will open the Command Prompt.
    /// </summary>
    public KeyCombination PromptKeys
    { get; set; } = new();
}
