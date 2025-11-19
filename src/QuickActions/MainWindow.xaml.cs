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

using System.Drawing;
using System.Windows;
using BadEcho.Presentation.Extensions;
using BadEcho.QuickActions.Options;
using BadEcho.QuickActions.Services;
using BadEcho.QuickActions.ViewModels;

namespace BadEcho.QuickActions;

/// <summary>
/// Provides the main window for the Quick Actions application.
/// </summary>
internal sealed partial class MainWindow
{
    private readonly AppearanceOptions _appearance;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow(MainViewModel viewModel, UserSettingsService userSettingsService)
    {
        DataContext = viewModel;

        _appearance = userSettingsService.Appearance;
    }

    /// <inheritdoc/>
    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);

        bool previouslyOpened = _appearance.WindowArea != default;

        if (!previouslyOpened)
        {
            Rectangle displayArea = this.FindDisplayArea();

            displayArea.Width /= 3;
            displayArea.Height /= 3;

            _appearance.WindowArea = new Rect(displayArea.X,
                                              displayArea.Y,
                                              displayArea.Width,
                                              displayArea.Height);
        }

        Width = _appearance.WindowArea.Width;
        Height = _appearance.WindowArea.Height;
        Left = _appearance.WindowArea.X;
        Top = _appearance.WindowArea.Y;
        
        if (!previouslyOpened)
            this.Recenter();

        LocationChanged += HandleLocationChanged;
    }

    private void HandleLocationChanged(object? sender, EventArgs e) 
        => _appearance.WindowArea = new Rect(Left, Top, ActualWidth, ActualHeight);
}
