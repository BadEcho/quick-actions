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
using BadEcho.QuickActions.Extensibility;

namespace BadEcho.QuickActions;

/// <summary>
/// Provides a mapping between a key combination and an action.
/// </summary>
internal sealed class Mapping
{
    /// <summary>
    /// Gets the set of modifier keys that must be pressed in order to execute the action.
    /// </summary>
    public HashSet<VirtualKey> ModifierKeys
    { get; } = [];

    /// <summary>
    /// Gets the set of non-modifier keys that must be pressed in order to execute the action.
    /// </summary>
    public HashSet<VirtualKey> Keys
    { get; } = [];

    /// <summary>
    /// Gets or sets the action executed if the keys defined in <see cref="ModifierKeys"/> and <see cref="Keys"/>
    /// are pressed.
    /// </summary>
    public IAction? Action
    { get; set; }
}
