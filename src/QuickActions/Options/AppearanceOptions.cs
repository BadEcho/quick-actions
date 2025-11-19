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

using System.Text.Json.Serialization;
using System.Windows;
using BadEcho.Presentation;

namespace BadEcho.QuickActions.Options;

/// <summary>
/// Provides the user's configuration settings pertaining to the user interface.
/// </summary>
internal sealed class AppearanceOptions
{
    /// <summary>
    /// The name of the configuration section the appearance options are sourced from.
    /// </summary>
    public static string SectionName
        => "Appearance";

    /// <summary>
    /// Gets or sets the last position and size of the main window.
    /// </summary>
    [JsonConverter(typeof(JsonRectConverter))]
    public Rect WindowArea
    { get; set; }
}
