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

using BadEcho.Presentation.ViewModels;

namespace BadEcho.QuickActions.Extensibility;

/// <summary>
/// Defines a view model that facilitates the display and manipulation of an action's configuration.
/// </summary>
public interface IActionConfigurationViewModel : IViewModel
{
    /// <summary>
    /// Occurs when the action's configuration changes.
    /// </summary>
    event EventHandler ConfigurationChanged;
}

