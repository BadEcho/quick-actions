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
using BadEcho.Presentation.Navigation;

namespace BadEcho.QuickActions.ViewModels;

/// <summary>
/// Provides the root view model for the Quick Actions application.
/// </summary>
internal sealed class MainViewModel : ViewModel, INavigationHost
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainViewModel"/> class.
    /// </summary>
    public MainViewModel(NavigationService navigationService, NavigationPaneViewModel navigationViewModel)
        : this(navigationViewModel)
    {
        navigationService.SetHost(this);
        navigationService.Navigate<HomeViewModel>();

        NavigateBackCommand = new DelegateCommand(_ => navigationService.NavigateBack());
        navigationService.Navigating += HandleNavigating;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MainViewModel"/> class.
    /// </summary>
    public MainViewModel()
        : this(new NavigationPaneViewModel())
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MainViewModel"/> class.
    /// </summary>
    private MainViewModel(NavigationPaneViewModel navigationViewModel)
    {
        NavigationViewModel = navigationViewModel;
        NavigationViewModel.Bind(new NavigationTarget
                                  {
                                      Name = "home",
                                      IconGlyph = "\uE8F2",
                                      ViewModelType = typeof(HomeViewModel)
                                  });
        NavigationViewModel.Bind(new NavigationTarget
                                  {
                                      Name = "actions",
                                      IconGlyph = "\uE700",
                                      ViewModelType = typeof(ActionsViewModel)
                                  });
        NavigationViewModel.Bind(new NavigationTarget
                                 {
                                     Name = "mappings",
                                     IconGlyph = "\uE707",
                                     ViewModelType = typeof(MappingsViewModel)
                                 });
    }

    /// <inheritdoc/>
    public IViewModel? CurrentViewModel
    {
        get;
        set
        {
            NotifyIfChanged(ref field, value);

            if (field == null)
                return;

            var selectedVm = NavigationViewModel.Items.FirstOrDefault(n => n.ViewModelType == field.GetType());

            NavigationViewModel.ChangeSelection(selectedVm);
        }
    }

    /// <summary>
    /// Gets the view model for the navigation pane.
    /// </summary>
    public NavigationPaneViewModel NavigationViewModel 
    { get; }

    /// <summary>
    /// Gets a command that, when executed, will navigate to the previous view.
    /// </summary>
    public ICommand? NavigateBackCommand
    { get; }

    /// <summary>
    /// Gets or sets a value indicating if it is possible to navigate backwards.
    /// </summary>
    public bool CanNavigateBack
    {
        get;
        set => NotifyIfChanged(ref field, value);
    }

    /// <inheritdoc/>
    public override void Disconnect()
    { }

    private void HandleNavigating(object? sender, EventArgs<Type> e)
    {
        if (sender is not NavigationService navigationService)
            return;
        
        CanNavigateBack = navigationService.CanNavigateBack;
    }
}
