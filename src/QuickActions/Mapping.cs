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

using BadEcho.Extensions;
using BadEcho.QuickActions.Extensibility;

namespace BadEcho.QuickActions;

/// <summary>
/// Provides a mapping between a key combination and an action.
/// </summary>
internal sealed class Mapping : IActionExecutionContext
{
    /// <summary>
    /// Gets the unique identifier of the mapping.
    /// </summary>
    public Guid Id 
    { get; init; } = Guid.NewGuid();

    /// <summary>
    /// Gets or sets the combination of keys that must be pressed in order to execute the action.
    /// </summary>
    public KeyCombination KeyCombination
    { get; set; } = new();

    /// <summary>
    /// Gets or sets the identifier for the action executed if the keys defined in <see cref="KeyCombination"/> are pressed.
    /// </summary>
    public Guid ActionId
    { get; set; }
    
    /// <summary>
    /// Gets or sets a path to a .wav file to play following the successful execution of the action.
    /// </summary>
    public string? CompletionSoundPath 
    { get; set; }

    /// <inheritdoc/>
    public string? ActionConfiguration
    { get; set; }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj is not Mapping other)
            return false;

        return Id == other.Id;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
        => this.GetHashCode(Id);
}
