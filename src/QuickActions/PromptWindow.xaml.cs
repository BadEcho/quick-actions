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

using BadEcho.Interop;
using BadEcho.Presentation.Extensions;
using BadEcho.Presentation.Messaging;
using BadEcho.QuickActions.Extensibility;
using BadEcho.QuickActions.Options;
using BadEcho.QuickActions.Services;
using BadEcho.QuickActions.ViewModels;
using Microsoft.Extensions.Logging;
using System.Drawing;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Point = System.Windows.Point;

namespace BadEcho.QuickActions;

/// <summary>
/// Provides a floating window displaying a prompt for entering commands.
/// </summary>
/// <remarks>
/// <para>
/// This floating window disappears when it loses focus, preventing it from taking up space on the user's screen when it isn't in use.
/// Similar "floating command prompt" utilities out there do not exhibit this behavior, and it's quite grating to have them ever-present
/// on the screen. We want this window to close whenever it's deactivated (e.g., when the user clicks another window or Alt-Tab).
/// </para>
/// <para>
/// Instead of handling the Deactivated or LostFocus events, however, we detect whether the user has clicked anywhere outside this window
/// or Alt-Tabbed, both of which require system hooks. We do this because the window will immediately deactivate after executing a command
/// (e.g., opening a folder or file), preventing the animation intended to play after a successful command from completing.
/// </para>
/// </remarks>
internal sealed partial class PromptWindow 
{
    private readonly ILogger<PromptWindow> _logger;
    private readonly NativeWindow _native;
    private readonly AppearanceOptions _appearance;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="PromptWindow"/> class.
    /// </summary>
    public PromptWindow(PromptViewModel viewModel, UserSettingsService settingsService, Mediator mediator, ILogger<PromptWindow> logger)
    {
        _logger = logger;
        DataContext = viewModel;

        _appearance = settingsService.Appearance;

        InitializeComponent();

        _native = new NativeWindow(this.GetSafeHandle());
        _native.RemoveTitleBar();

        mediator.Register(Messages.MouseClicked, MediateMouseClicked);
        mediator.Register(Messages.AltTabbed, MediateAltTabbed);
    }
    
    /// <inheritdoc/>
    protected override void OnActivated(EventArgs e)
    {
        base.OnActivated(e);

        int attempts = 5;
        bool foreground = false;

        // We are essentially stealing focus here. This is the desired action and is akin to hitting the Windows key to open the Start Menu.
        // I've observed that, despite our best efforts, failure may still occur when setting the foreground window. Trying again, however,
        // seems to do the trick, and consistently at that.
        while (!foreground && attempts > 0)
        {
            foreground = _native.SetForegroundWindow();
            attempts--;
        }

        _logger.PromptActivationAttempts(5 - attempts);

        Command.Focus();
        this.Recenter();
    }

    /// <inheritdoc/>
    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);

        bool previouslyOpened = _appearance.PromptArea != default;

        if (!previouslyOpened)
        {
            Rectangle displayArea = this.FindDisplayArea();

            _appearance.PromptArea = new Rect(displayArea.X,
                                              displayArea.Y,
                                              0,
                                              0);
        }

        Left = _appearance.PromptArea.X;
        Top = _appearance.PromptArea.Y;

        if (!previouslyOpened)
            this.Recenter();

        LocationChanged += HandleAreaChanged;
        SizeChanged += HandleAreaChanged;
    }

    /// <inheritdoc/>
    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        
        if (e.Key == Key.Escape)
            Hide();
    }

    private void HideIfVisible(bool clearText = false)
    {
        if (!IsVisible)
            return;

        if (clearText)
            Command.Text = string.Empty;

        Hide();
    }

    private void HandleAreaChanged(object? sender, EventArgs e)
        => _appearance.PromptArea = RestoreBounds;

    private void HandleMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
            DragMove();
    }

    private void HandleSuccessAnimationCompleted(object? sender, EventArgs e)
        => HideIfVisible(true);

    private void HandleCommandTargetUpdated(object? sender, DataTransferEventArgs e)
    {   // If text is being transferred from our view model to the text box (i.e., navigating history), the caret index
        // won't be updated like it normally is when the text box is receiving input.
        Command.CaretIndex = Command.Text.Length;
    }

    private void MediateMouseClicked(Point position)
    {   
        Dispatcher.Invoke(HideIfOutsideBounds);

        void HideIfOutsideBounds()
        {
            Rect bounds = RestoreBounds;
            Display display = Display.FromWindow(_native.Handle);

            bounds.Scale(display.ScaleFactor, display.ScaleFactor);

            if (!bounds.Contains(position))
                HideIfVisible();
        }
    }

    private void MediateAltTabbed() 
        => Dispatcher.Invoke(() => HideIfVisible());
}
