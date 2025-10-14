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

using BadEcho.Presentation.ViewModels;

namespace BadEcho.QuickActions.ViewModels;

/// <summary>
/// Provides a view model for the home page.
/// </summary>
internal sealed class HomeViewModel : ViewModel
{
    private int _actionsCount;
    private int _mappingsCount;

    /// <summary>
    /// Gets or sets the number of available actions.
    /// </summary>
    public int ActionsCount
    {
        get => _actionsCount;
        set => NotifyIfChanged(ref _actionsCount, value);
    }

    /// <summary>
    /// Gets or sets the number of defined mappings.
    /// </summary>
    public int MappingsCount
    {
        get => _mappingsCount;
        set => NotifyIfChanged(ref _mappingsCount, value);
    }

    /// <inheritdoc/>
    public override void Disconnect()
    {
        ActionsCount = 0;
        MappingsCount = 0;
    }
}
