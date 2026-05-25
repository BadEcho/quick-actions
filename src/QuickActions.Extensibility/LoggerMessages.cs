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

using Microsoft.Extensions.Logging;

namespace BadEcho.QuickActions.Extensibility;

/// <summary>
/// Provides logger message templates.
/// </summary>
/// <remarks>
/// The logger messages used by the main application need to be defined in this assembly due to a bug related to source-generated logging
/// and WPF application projects (https://github.com/dotnet/wpf/issues/9589). This class can be moved to the main assembly whenever this
/// issue is resolved (if it ever gets resolved, that is).
/// </remarks>
public static partial class LoggerMessages
{
    /// <summary>
    /// Logs the execution of an action.
    /// </summary>>
    [LoggerMessage(Level = LogLevel.Debug,
                   Message = "Executing action: {ActionName}")]
    public static partial void ExecutingAction(this ILogger logger, string actionName);

    /// <summary>
    /// Logs the number of attempted activations of the command prompt.
    /// </summary>
    [LoggerMessage(Level = LogLevel.Debug,
                   Message = "Command prompt activation attempted {ForegroundAttempts} times.")]
    public static partial void PromptActivationAttempts(this ILogger logger, int foregroundAttempts);
}
