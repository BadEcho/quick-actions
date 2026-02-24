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

using BadEcho.Interop;

namespace BadEcho.QuickActions;

/// <summary>
/// Provides a set of static methods intended to aid in matters related to virtual-key codes.
/// </summary>
internal static class VirtualKeyExtensions
{
    /// <summary>
    /// Removes any directional component from this key if it is a modifier key.
    /// </summary>
    /// <param name="source">An enumeration value that specifies a virtual-key code.</param>
    /// <returns><c>source</c> with its directional component removed if it was a modifier key.</returns>
    public static VirtualKey NormalizeModifiers(this VirtualKey source)
        => source switch
        {
            VirtualKey.LeftAlt or VirtualKey.RightAlt => VirtualKey.Alt,
            VirtualKey.LeftShift or VirtualKey.RightShift => VirtualKey.Shift,
            VirtualKey.LeftControl or VirtualKey.RightControl => VirtualKey.Control,
            _ => source
        };

    /// <summary>
    /// Determines if this key is a (normalized) modifier key.
    /// </summary>
    /// <param name="source">An enumeration value that specifies a virtual-key code.</param>
    /// <returns>True if <c>source</c> is a modifier key; otherwise, false.</returns>
    public static bool IsModifier(this VirtualKey source)
        => source is VirtualKey.Alt or VirtualKey.Shift or VirtualKey.Control or VirtualKey.LeftWindows or VirtualKey.RightWindows;
}