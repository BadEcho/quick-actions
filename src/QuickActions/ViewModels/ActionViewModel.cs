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
using BadEcho.QuickActions.Extensibility;

namespace BadEcho.QuickActions.ViewModels;

/// <summary>
/// Provides a view model that facilitates the display of an action that can be executed by Quick Actions.
/// </summary>
/// <typeparam name="TAction">The type of action bound to the view model for display on a view.</typeparam>
internal class ActionViewModel<TAction> : ViewModel<IAction, TAction>, IActionViewModel
    where TAction : IAction
{
    /// <inheritdoc/>
    public string Name
    {
        get;
        set => NotifyIfChanged(ref field, value);
    } = string.Empty;

    /// <inheritdoc/>
    protected override void OnBinding(TAction model)
    {
        Name = model.Name;
    }

    /// <inheritdoc/>
    protected override void OnUnbound(TAction model)
    {
        Name = string.Empty;
    }
}
