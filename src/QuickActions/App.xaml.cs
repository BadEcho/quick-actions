// -----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2026 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
// -----------------------------------------------------------------------

using BadEcho.Presentation.Messaging;
using BadEcho.QuickActions.Services;
using System.ComponentModel;
using System.Windows;
using BadEcho.QuickActions.Properties;
using Microsoft.Extensions.DependencyInjection;

namespace BadEcho.QuickActions;

/// <summary>
/// Provides the Quick Actions application.
/// </summary>
internal sealed partial class App : IDisposable
{
    private readonly Mediator _mediator = new();
    private readonly IServiceProvider? _serviceProvider;
    private readonly UserSettingsService? _settingsService;

    private NotificationArea? _notificationArea;
    private bool _exiting;

    /// <summary>
    /// Initializes a new instance of the <see cref="App"/> class.
    /// </summary>
    public App(UserSettingsService settingsService, Mediator mediator, IServiceProvider serviceProvider)
    {
        _settingsService = settingsService;
        _mediator = mediator;
        _serviceProvider = serviceProvider;

        _mediator.Register(Messages.ShowPrompt, MediateShowPrompt);

        InitializeComponent();
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

    /// <inheritdoc/>
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        if (MainWindow == null)
            throw new InvalidOperationException(Strings.AppInvalidHost);

        MainWindow.Closing += HandleMainWindowClosing;

        bool startMinimized = _settingsService?.StartMinimized ?? false;

        if (!startMinimized)
            MainWindow.Show();

        _notificationArea = new NotificationArea(MainWindow, _mediator);
        _notificationArea.QuitClicked += HandleQuitClicked;

        if (startMinimized)
            _notificationArea.EnableOpen();
    }

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

    private void MediateShowPrompt()
    {
        if (_serviceProvider == null)
            throw new InvalidOperationException(Strings.AppInvalidHost);

        Dispatcher.Invoke(() =>
        {
            var prompt = _serviceProvider.GetRequiredService<PromptWindow>();

            prompt.Show();
            
            // Manual activation is required for preexisting, hidden windows.
            prompt.Activate();
        });
    }
}