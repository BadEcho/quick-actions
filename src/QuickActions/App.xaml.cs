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

using System.ComponentModel;
using BadEcho.Presentation.Messaging;
using BadEcho.QuickActions.Services;

namespace BadEcho.QuickActions;

/// <summary>
/// Provides the Quick Actions application.
/// </summary>
internal sealed partial class App : IDisposable
{
    private readonly UserSettingsService? _settingsService;
    private readonly NotificationArea? _notificationArea;

    private bool _exiting;

    /// <summary>
    /// Initializes a new instance of the <see cref="App"/> class.
    /// </summary>
    public App(MainWindow window, UserSettingsService settingsService, Mediator mediator)
    {
        _settingsService = settingsService;
        InitializeComponent();
        
        MainWindow = window;
        MainWindow.Closing += HandleMainWindowClosing;
        
        window.InitializeComponent();

        IEnumerable<string> args = Environment.GetCommandLineArgs()
                                              .Skip(1);

        bool silentStartup = args.Contains("--silent", StringComparer.OrdinalIgnoreCase);
        
        if (!silentStartup)
            window.Show();

        _notificationArea = new NotificationArea(MainWindow, mediator);
        _notificationArea.QuitClicked += HandleQuitClicked;

        if (silentStartup)
            _notificationArea.EnableOpen();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="App"/> class.
    /// </summary>
    /// <remarks>
    /// This is required because WPF's MSBuild tasks will generate an application entry point that expects App
    /// to have a default constructor, even though our own application entry point in Program.cs is what will actually
    /// be called.
    /// </remarks>
    private App() 
        => InitializeComponent();

    /// <inheritdoc/>
    public void Dispose() 
        => _notificationArea?.Dispose();

    private void HandleMainWindowClosing(object? sender, CancelEventArgs e)
    {
        bool minimizeToTray = _settingsService is { MinimizeToTrayOnClose: true };

        if (_exiting || !minimizeToTray)
            return;

        e.Cancel = true;

        MainWindow?.Hide();
        _notificationArea?.EnableOpen();
    }

    private void HandleQuitClicked(object? sender, EventArgs e)
    {
        _exiting = true;
        MainWindow?.Close();
    }
}
