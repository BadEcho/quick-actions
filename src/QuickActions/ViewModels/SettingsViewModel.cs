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

using System.Windows.Input;
using BadEcho.Presentation;
using BadEcho.Presentation.ViewModels;
using BadEcho.QuickActions.Services;

namespace BadEcho.QuickActions.ViewModels;

/// <summary>
/// Provides a view model that facilitates the display and manipulation of the user's settings.
/// </summary>
internal sealed class SettingsViewModel : ViewModel
{
    private readonly UserSettingsService? _userSettingsService;

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsViewModel"/> class.
    /// </summary>
    public SettingsViewModel(UserSettingsService userSettingsService)
        : this()
    {
        _userSettingsService = userSettingsService;

        OpenOnStartup = userSettingsService.OpenOnStartup;
        MinimizeToTrayOnClose = userSettingsService.MinimizeToTrayOnClose;

        IsDirty = false;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsViewModel"/> class.
    /// </summary>
    public SettingsViewModel()
    {
        SaveCommand = new DelegateCommand(SaveSettings);
    }

    /// <inheritdoc cref="UserSettingsService.OpenOnStartup"/>
    public bool OpenOnStartup
    {
        get;
        set
        {
            NotifyIfChanged(ref field, value);

            _userSettingsService?.OpenOnStartup = value;
        }
    }

    /// <inheritdoc cref="UserSettingsService.MinimizeToTrayOnClose"/>
    public bool MinimizeToTrayOnClose
    {
        get;
        set
        {
            NotifyIfChanged(ref field, value);

            _userSettingsService?.MinimizeToTrayOnClose = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating if any of the bound settings contain unsaved changes.
    /// </summary>
    public bool IsDirty
    {
        get;
        set => NotifyIfChanged(ref field, value);
    }

    /// <summary>
    /// Gets a command that, when executed, saves changes made to the user's settings.
    /// </summary>
    public ICommand SaveCommand
    { get; }

    /// <inheritdoc/>
    public override void Disconnect()
    { }

    /// <inheritdoc/>
    protected override void OnPropertyChanged(string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if (propertyName != nameof(IsDirty) && propertyName != nameof(OpenOnStartup))
            IsDirty = true;
    }

    private void SaveSettings(object? obj)
    {
        _userSettingsService?.SaveGeneral();
        IsDirty = false;
    }
}
