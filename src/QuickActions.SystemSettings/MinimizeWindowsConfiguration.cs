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

namespace BadEcho.QuickActions.SystemSettings;

/// <summary>
/// Provides additional configuration for the <see cref="MinimizeWindows"/> action.
/// </summary>
internal sealed class MinimizeWindowsConfiguration
{
    /// <summary>
    /// Gets or sets the name of the process whose windows should be minimized. If this is not specified,
    /// all windows on the current desktop will be minimized.
    /// </summary>
    public string ProcessName 
    { get; set; } = string.Empty;
}
