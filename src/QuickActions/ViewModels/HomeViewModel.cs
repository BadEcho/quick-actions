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
using BadEcho.Presentation.Messaging;
using BadEcho.Presentation.Navigation;
using BadEcho.Presentation.ViewModels;
using BadEcho.QuickActions.Services;

namespace BadEcho.QuickActions.ViewModels;

/// <summary>
/// Provides a view model for the home page.
/// </summary>
internal sealed class HomeViewModel : ViewModel
{
    private readonly NavigationService? _navigationService;
    private readonly UserSettingsService? _settingsService;

    /// <summary>
    /// Initializes a new instance of the <see cref="HomeViewModel"/> class.
    /// </summary>
    public HomeViewModel(NavigationService navigationService, UserSettingsService settingsService, Mediator mediator)
        : this()
    {
        _navigationService = navigationService;
        _settingsService = settingsService;

        ActionsDisabled = _settingsService.ActionsDisabled;

        // We use mediator messages as the source of truth for whether actions are enabled or disabled because the settings view, which also offers
        // an option to toggle actions on and off, can be opened while this view is active.
        mediator.Register(Messages.DisableListener, MediateDisableListener);
        mediator.Register(Messages.EnableListener, MediateEnableListener);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HomeViewModel"/> class.
    /// </summary>
    public HomeViewModel()
    {
        NavigateToActionsCommand = new DelegateCommand(NavigateToActions);
        NavigateToMappingsCommand = new DelegateCommand(NavigateToMappings);
        ToggleMappingsCommand = new DelegateCommand(ToggleMappings);
    }

    /// <inheritdoc cref="UserSettingsService.ActionsDisabled"/>
    public bool ActionsDisabled
    {
        get;
        set => NotifyIfChanged(ref field, value);
    }

    /// <summary>
    /// Gets a command that, when executed, will navigate to the actions view.
    /// </summary>
    public ICommand NavigateToActionsCommand
    { get; }

    /// <summary>
    /// Gets a command that, when executed, will navigate to the mappings view.
    /// </summary>
    public ICommand NavigateToMappingsCommand
    { get; }

    /// <summary>
    /// Gets a command that, when executed, toggles the mapping on and off.
    /// </summary>
    public ICommand ToggleMappingsCommand
    { get; }

    /// <inheritdoc/>
    public override void Disconnect()
    { }

    private void NavigateToActions(object? _) 
        => _navigationService?.Navigate<ActionsViewModel>();

    private void NavigateToMappings(object? _) 
        => _navigationService?.Navigate<MappingsViewModel>();

    private void ToggleMappings(object? _)
        => _settingsService?.ActionsDisabled = !_settingsService.ActionsDisabled;

    private void MediateDisableListener()
        => ActionsDisabled = true;

    private void MediateEnableListener()
        => ActionsDisabled = false;
}
