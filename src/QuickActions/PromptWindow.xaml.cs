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
    private readonly AppearanceOptions _appearance;
    private readonly NativeWindow _nativeWindow;
    private bool _hasBeenShown;

    /// <summary>
    /// Initializes a new instance of the <see cref="PromptWindow"/> class.
    /// </summary>
    public PromptWindow(PromptViewModel viewModel, UserSettingsService settingsService, Mediator mediator, ILogger<PromptWindow> logger)
    {
        _logger = logger;
        _appearance = settingsService.Appearance;        
        DataContext = viewModel;

        InitializeComponent();

        WindowWrapper windowWrapper = this.GetWrapper();

        _nativeWindow = new NativeWindow(windowWrapper);
        _nativeWindow.RemoveSystemMenu(false);
        
        mediator.Register(Messages.MouseClicked, MediateMouseClicked);
        mediator.Register(Messages.AltTabbed, MediateAltTabbed);
    }

    /// <summary>
    /// Opens the window if it hasn't been previously; otherwise, uncloaks it.
    /// </summary>
    /// <remarks>
    /// We cloak this window when hiding it from the screen instead of following the normal hide or close routes.
    /// This is to avoid the inevitable white flicker that will occur when we need to show it again, which happens due to
    /// the HWND becoming immediately visible and WPF having to re-enable the render target, re-run layout, render,
    /// and then finally present it.
    /// </remarks>
    public void Reveal()
    {
        if (!_hasBeenShown)
        {
            Show();
            _hasBeenShown = true;
        }
        
        ChangeVisibility(false);

        // Ensure the window receives keyboard input.
        // Either due to something I'm doing, or a bug with the framework, calling Activate() is not sufficient,
        // especially if the application was started when the user logged on via the Run registry key.
        bool foreground = _nativeWindow.SetForegroundWindow();

        if (!foreground)
            _logger.PromptActivationFailed();

        Command.Focus();
    }

    /// <inheritdoc/>
    protected override void OnActivated(EventArgs e)
    {
        base.OnActivated(e);
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
            ChangeVisibility(true);
    }

    private void HideIfVisible(bool clearText = false)
    {
        if (!_hasBeenShown || _nativeWindow.IsCloaked)
            return;

        if (clearText)
            Command.Text = string.Empty;

        ChangeVisibility(true);
    }

    private void ChangeVisibility(bool hide)
    {   // If we're cloaking the window, we update its extended window styles to reflect that it is a tool window.
        // This is so the window is hidden from the Alt-Tab switcher. Merely having ShowInTaskbar = false is not sufficient.
        // Because we aren't forcing a frame recalculation, the visual changes that normally accompany a tool window are not
        // visible to the user -- were we to force one, WindowChrome would measure the frame as a standard window's and push
        // the prompt's content off-center.
        _nativeWindow.ChangeExtendedStyles(ExtendedWindowStyles.ToolWindow, hide);
        _nativeWindow.IsCloaked = hide;
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
            Display display = Display.FromWindow(_nativeWindow.Handle);

            bounds.Scale(display.ScaleFactor, display.ScaleFactor);

            if (!bounds.Contains(position))
                HideIfVisible();
        }
    }

    private void MediateAltTabbed() 
        => Dispatcher.Invoke(() => HideIfVisible());
}
