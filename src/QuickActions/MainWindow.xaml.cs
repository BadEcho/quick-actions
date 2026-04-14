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

using BadEcho.Presentation.Extensions;
using BadEcho.QuickActions.Options;
using BadEcho.QuickActions.Services;
using BadEcho.QuickActions.ViewModels;
using System.Drawing;
using System.Windows;
using BadEcho.Interop;
using BadEcho.Presentation.Windows;

namespace BadEcho.QuickActions;

/// <summary>
/// Provides the main window for the Quick Actions application.
/// </summary>
internal sealed partial class MainWindow
{
    private readonly UserSettingsService _settingsService;
    private readonly AppearanceOptions _appearance;
    
    private WindowWrapper? _wrapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow(MainViewModel viewModel, UserSettingsService settingsService)
    {
        DataContext = viewModel;

        _settingsService = settingsService;
        _appearance = settingsService.Appearance;

        InitializeComponent();
    }

    /// <inheritdoc/>
    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);

        bool previouslyOpened = _appearance.WindowArea != default;

        if (!previouslyOpened)
        {
            Rectangle displayArea = this.FindDisplayArea();

            _appearance.WindowArea = new Rect(displayArea.X,
                                              displayArea.Y,
                                              displayArea.Width / 2.5,
                                              displayArea.Height / 2.9);
        }

        Width = _appearance.WindowArea.Width;
        Height = _appearance.WindowArea.Height;
        Left = _appearance.WindowArea.X;
        Top = _appearance.WindowArea.Y;
        
        if (!previouslyOpened)
            this.Recenter();

        LocationChanged += HandleAreaChanged;
        SizeChanged += HandleAreaChanged;
    }

    private void HandleAreaChanged(object? sender, EventArgs e) 
        => _appearance.WindowArea = new Rect(Left, Top, ActualWidth, ActualHeight);

    private void HandleSettingsClick(object sender, RoutedEventArgs e)
    {
        _wrapper ??= this.GetWrapper();

        var dialogHost = new DialogHost(this, _wrapper)
                         {
                             Padding = new Thickness(24)
                         };

        var settingsVm = new SettingsViewModel(_settingsService);

        dialogHost.Show(settingsVm);
    }
}
