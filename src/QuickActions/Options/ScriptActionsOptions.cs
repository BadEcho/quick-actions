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

using System.Collections.ObjectModel;
using BadEcho.QuickActions.Extensibility;

namespace BadEcho.QuickActions.Options;

/// <summary>
/// Provides the user's configuration settings regarding script actions.
/// </summary>
internal class ScriptActionsOptions : Collection<ScriptAction>
{
    /// <summary>
    /// The name of the configuration section the script actions are sourced from.
    /// </summary>
    public static string SectionName
        => "Scripts";
}
