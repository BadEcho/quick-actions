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
using BadEcho.Presentation.Navigation;

namespace BadEcho.QuickActions.ViewModels;

/// <summary>
/// Provides the root view model for the Quick Actions application.
/// </summary>
internal sealed class MainViewModel : ViewModel, INavigationHost
{
    private IViewModel? _currentViewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainViewModel"/> class.
    /// </summary>
    public MainViewModel(NavigationService navigationService, NavigationPaneViewModel navigationViewModel)
        : this(navigationViewModel)
    {
        navigationService.SetHost(this);
        navigationService.Navigate<HomeViewModel>();
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
    }

    /// <inheritdoc/>
    public IViewModel? CurrentViewModel
    {
        get => _currentViewModel;
        set
        {
            NotifyIfChanged(ref _currentViewModel, value);

            if (_currentViewModel == null)
                return;

            NavigationViewModel.SelectedViewModel = 
                NavigationViewModel.Items.FirstOrDefault(n => n.ViewModelType == _currentViewModel.GetType());
        }
    }

    /// <summary>
    /// Gets the view model for the navigation pane.
    /// </summary>
    public NavigationPaneViewModel NavigationViewModel 
    { get; }

    /// <inheritdoc/>
    public override void Disconnect()
    { }
}
