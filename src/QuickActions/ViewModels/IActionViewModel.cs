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

using BadEcho.Presentation;
using BadEcho.Presentation.ViewModels;
using BadEcho.QuickActions.Extensibility;

namespace BadEcho.QuickActions.ViewModels;

/// <summary>
/// Defines a view model that facilitates the display of an action that can be executed by Quick Actions.
/// </summary>
internal interface IActionViewModel : IViewModel, IModelProvider<IAction>
{
    /// <summary>
    /// Gets or sets the displayed name of the bound action.
    /// </summary>
    string Name { get; set; }
}
