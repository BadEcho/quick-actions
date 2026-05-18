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

namespace BadEcho.QuickActions;

/// <summary>
/// Specifies the result of executing a command via the Command Prompt.
/// </summary>
internal enum PromptCommandResult
{
    /// <summary>
    /// No command was executed.
    /// </summary>
    None,
    /// <summary>
    /// The command succeeded.
    /// </summary>
    Succeeded,
    /// <summary>
    /// The command failed.
    /// </summary>
    Failed
}
