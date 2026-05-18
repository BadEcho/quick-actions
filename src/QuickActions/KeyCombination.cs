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

using System.Text.Json.Serialization;
using BadEcho.Extensions;
using BadEcho.Interop;

namespace BadEcho.QuickActions;

/// <summary>
/// Provides a combination of keys.
/// </summary>
internal sealed class KeyCombination
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KeyCombination"/> class.
    /// </summary>
    /// <param name="modifierKeys">The set of modifier keys.</param>
    /// <param name="keys">The set of non-modifier keys.</param>
    public KeyCombination(IEnumerable<VirtualKey> modifierKeys, IEnumerable<VirtualKey> keys)
    {
        ModifierKeys = [..modifierKeys];
        Keys = [..keys];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="KeyCombination"/> class.
    /// </summary>
    public KeyCombination()
    { }

    /// <summary>
    /// Gets the set of modifier keys.
    /// </summary>
    public HashSet<VirtualKey> ModifierKeys
    { get; } = [];

    /// <summary>
    /// Gets the set of non-modifier keys.
    /// </summary>
    public HashSet<VirtualKey> Keys
    { get; } = [];

    /// <summary>
    /// Gets a value indicating if the combination contains no keys.
    /// </summary>
    [JsonIgnore]
    public bool IsEmpty
        => ModifierKeys.IsEmpty() && Keys.IsEmpty();

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj is not KeyCombination other)
            return false;

        return ModifierKeys.SetEquals(other.ModifierKeys)
               && Keys.SetEquals(other.Keys);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        int hashCode = 0;
        var stack = new Stack<VirtualKey>([..ModifierKeys.Order(), ..Keys.Order()]);

        while (stack.TryPop(out VirtualKey key))
        {
            hashCode = HashCode.Combine(hashCode, key);
        }

        return hashCode;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        var keySequence = ModifierKeys.Select(GetKeyName)
                                      .Concat(Keys.Select(Enum.GetName));

        string description = string.Join(" + ", keySequence);

        return description;

        static string GetKeyName(VirtualKey key)
        {
            return key switch
            {
                VirtualKey.Control => "Ctrl",
                VirtualKey.LeftWindows or VirtualKey.RightWindows => "Windows",
                _ => Enum.GetName(key) ?? string.Empty
            };
        }
    }
}
