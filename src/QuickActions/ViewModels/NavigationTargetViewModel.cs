﻿// -----------------------------------------------------------------------
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
/// Provides a view model for a navigation target.
/// </summary>
internal sealed class NavigationTargetViewModel : ViewModel<NavigationTarget>
{
    private string _name = string.Empty;
    private string _iconGlyph = string.Empty;
    private Type? _viewModelType;

    /// <summary>
    /// Gets or sets the name of the navigation target.
    /// </summary>
    public string Name
    {
        get => _name;
        set => NotifyIfChanged(ref _name, value);
    }

    /// <summary>
    /// Gets or sets the glyph used for the target's icon.
    /// </summary>
    /// <remarks>This is mapped to a Unicode PUA range used by the active symbol icon font.</remarks>
    public string IconGlyph
    {
        get => _iconGlyph;
        set => NotifyIfChanged(ref _iconGlyph, value);
    }

    /// <summary>
    /// Gets or sets the type of view model associated with the target's content.
    /// </summary>
    public Type? ViewModelType
    {
        get => _viewModelType;
        set => NotifyIfChanged(ref _viewModelType, value);
    }

    /// <inheritdoc/>
    protected override void OnBinding(NavigationTarget model)
    {
        Require.NotNull(model, nameof(model));

        Name = model.Name;
        IconGlyph = model.IconGlyph;
        ViewModelType = model.ViewModelType;
    }

    /// <inheritdoc/>
    protected override void OnUnbound(NavigationTarget model)
    {
        Name = string.Empty;
        IconGlyph = string.Empty;
        ViewModelType = null;
    }
}
