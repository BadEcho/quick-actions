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

namespace BadEcho.QuickActions.Options;

/// <summary>
/// Provides the user's configuration settings pertaining to action mappings.
/// </summary>
internal sealed class MappingOptions : Collection<Mapping>
{
    /// <summary>
    /// The name of the configuration section the mappings are sourced from.
    /// </summary>
    public static string SectionName
        => "Mappings";
}
