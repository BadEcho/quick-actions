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

using BadEcho.Interop;
using BadEcho.Presentation.Extensions;
using System.Drawing;
using System.IO;
using BadEcho.Presentation.Windows;
using BadEcho.QuickActions.Properties;
using Window = System.Windows.Window;

namespace BadEcho.QuickActions;

/// <summary>
/// Provides support for the notification area of the Windows taskbar.
/// </summary>
internal sealed class NotificationArea : IDisposable
{
    private readonly NotifyIcon _icon;
    private readonly PopupMenu _menu;
    private readonly MenuItem _open;
    private readonly MenuItem _quit;
    private readonly Window _window;

    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationArea"/> class.
    /// </summary>
    /// <param name="window">The main window of the application.</param>
    public NotificationArea(Window window)
    {
        Require.NotNull(window, nameof(window));
        
        _window = window;

        nint handle = window.GetHandle();
        var windowWrapper = new PresentationWindowWrapper(handle);

        _icon = new NotifyIcon(windowWrapper, "Quick Actions", Images.Icon);
        _menu = new PopupMenu(windowWrapper);

        using (var logoStream = new MemoryStream(Images.LogoMenu))
        {
            using (var logo = (Bitmap) Image.FromStream(logoStream))
            {
                var hLogo = logo.GetHbitmap(Color.FromArgb(0));

                _open = _menu.AddItem("Open Quick Actions", hLogo);
                _menu.DisableItem(_open);
            }
        }

        _menu.AddSeparator();
        _quit = _menu.AddItem("Quit");

        _icon.LeftClicked += HandleIconLeftClicked;
        _icon.RightClicked += HandleIconRightClicked;

        _icon.Show();
    }

    /// <summary>
    /// Occurs when the user has clicked on the Quit context menu item.
    /// </summary>
    public event EventHandler? QuitClicked;

    /// <summary>
    /// Enables the Open context menu item.
    /// </summary>
    public void EnableOpen()
        => _menu.EnableItem(_open);

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_disposed)
            return;

        _menu.Dispose();
        _icon.Dispose();

        _disposed = true;
    }

    private void HandleIconLeftClicked(object? sender, EventArgs e) 
        => _window.Show();

    private void HandleIconRightClicked(object? sender, EventArgs e)
    {
        MenuItem? selection = _menu.Open();

        if (selection == null)
            return;

        if (selection == _open)
        {
            _window.Show();
            _menu.DisableItem(_open);
        }
        else if (selection == _quit)
        {
            QuitClicked?.Invoke(this, EventArgs.Empty);
        }
    }
}
