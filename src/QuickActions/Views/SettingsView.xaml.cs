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

using System.Windows;
using System.Windows.Controls;

namespace BadEcho.QuickActions.Views;

/// <summary>
/// Provides a view for displaying the user's settings.
/// </summary>
internal sealed partial class SettingsView
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsView"/> class.
    /// </summary>
    public SettingsView() 
        => InitializeComponent();

    private void HandleKeysTextChanged(object sender, TextChangedEventArgs e)
        => Keys.CaretIndex = Keys.Text.Length;

    private void HandleKeysGotFocus(object sender, RoutedEventArgs e)
        => Keys.CaretIndex = Keys.Text.Length;
}
