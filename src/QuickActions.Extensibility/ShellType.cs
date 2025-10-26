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

namespace BadEcho.QuickActions.Extensibility;

/// <summary>
/// Specifies the type of command interpreter used to execute a script.
/// </summary>
public enum ShellType
{
    /// <summary>
    /// Windows Command shell (cmd.exe) is used to execute the script.
    /// </summary>
    Command,
    /// <summary>
    /// PowerShell is used to execute the script.
    /// </summary>
    PowerShell
}
