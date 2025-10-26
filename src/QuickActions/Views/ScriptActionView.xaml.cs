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

using System.Windows;
using System.Windows.Input;

namespace BadEcho.QuickActions.Views;

/// <summary>
/// Provides a view for displaying a script action.
/// </summary>
internal sealed partial class ScriptActionView 
{
    /// <inheritdoc/>
    public ScriptActionView() 
        => InitializeComponent();

    /// <inheritdoc/>
    protected override void OnGotFocus(RoutedEventArgs e)
    {
        base.OnGotFocus(e);

        FocusManager.SetFocusedElement(this, PathTextBox);
    }
}
