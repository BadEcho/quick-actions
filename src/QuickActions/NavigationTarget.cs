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

namespace BadEcho.QuickActions;

/// <summary>
/// Provides information for a navigation target.
/// </summary>
internal sealed class NavigationTarget
{
    /// <summary>
    /// Gets the name of this navigation target.
    /// </summary>
    public string Name
    { get; init; } = string.Empty;

    /// <summary>
    /// Gets the glyph used for this target's icon.
    /// </summary>
    /// <remarks>This is mapped to a Unicode PUA range used by the active symbol icon font.</remarks>
    public string IconGlyph
    { get; init; } = string.Empty;

    /// <summary>
    /// Gets the type of view model associated with this target's content.
    /// </summary>
    public Type? ViewModelType
    { get; init; }
}
