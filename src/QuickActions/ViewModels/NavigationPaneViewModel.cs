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

using BadEcho.Presentation;
using BadEcho.Presentation.ViewModels;
using System.Windows.Input;
using BadEcho.Presentation.Navigation;

namespace BadEcho.QuickActions.ViewModels;

/// <summary>
/// Provides a view model for a navigation pane.
/// </summary>
internal sealed class NavigationPaneViewModel : CollectionViewModel<NavigationTarget, NavigationTargetViewModel>
{
    private readonly NavigationService? _navigationService;

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationPaneViewModel"/> class.
    /// </summary>
    public NavigationPaneViewModel(NavigationService navigationService)
        : this()
    {
        _navigationService = navigationService;

        SelectionCommand = new DelegateCommand(NavigateToSelection);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationPaneViewModel"/> class.
    /// </summary>
    public NavigationPaneViewModel()
        : base(new CollectionViewModelOptions { OffloadBatchBindings = false })
    { }

    /// <summary>
    /// Gets or sets the selected navigation target view model.
    /// </summary>
    public NavigationTargetViewModel? SelectedViewModel
    {
        get;
        set => NotifyIfChanged(ref field, value);
    }

    /// <summary>
    /// Gets or sets the command execution when a selection is made.
    /// </summary>
    public ICommand? SelectionCommand
    {
        get;
        set => NotifyIfChanged(ref field, value);
    }

    /// <inheritdoc/>
    public override NavigationTargetViewModel CreateItem(NavigationTarget model)
    {
        var viewModel = new NavigationTargetViewModel();

        viewModel.Bind(model);

        return viewModel;
    }

    /// <inheritdoc/>
    public override void UpdateItem(NavigationTarget model)
    {
        var existingChild = FindItem<NavigationTargetViewModel>(model);

        existingChild?.Bind(model);
    }

    private void NavigateToSelection(object? parameter)
    {
        var viewModel = (NavigationTargetViewModel?) parameter;

        if (viewModel is { ViewModelType: not null })
            _navigationService?.Navigate(viewModel.ViewModelType);
    }
}
