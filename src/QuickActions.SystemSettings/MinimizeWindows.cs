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

using System.Composition;
using System.Diagnostics;
using System.Text.Json;
using BadEcho.Interop;
using BadEcho.QuickActions.Extensibility;
using BadEcho.QuickActions.SystemSettings.Properties;

namespace BadEcho.QuickActions.SystemSettings;

/// <summary>
/// Provides an action that minimizes either all windows on the screen, or if a process name is specified,
/// all windows belonging to the named process.
/// </summary>
[Export(typeof(IAction))]
internal sealed class MinimizeWindows : CodeAction
{
    internal const string ActionId = "{FF28EEBB-A8BC-4249-B514-869CDA03787B}";

    /// <summary>
    /// Initializes a new instance of the <see cref="MinimizeWindows"/> class.
    /// </summary>
    public MinimizeWindows() 
        : base(new Guid(ActionId), Strings.MinimizeWindowsName)
    { }

    /// <inheritdoc/>
    public override string Description
        => Strings.MinimizeWindowsDescription;

    /// <inheritdoc/>
    public override ActionResult Execute(IActionExecutionContext executionContext)
    {
        foreach (Process process in GetProcesses(executionContext))
        {
            foreach (NativeWindow window in NativeWindow.FromProcessId(process.Id))
            {
                if (window.IsVisible)
                    window.Minimize();
            }
        }
        
        return ActionResult.Ok(this);
    }

    private static Process[] GetProcesses(IActionExecutionContext executionContext)
    {
        string? processName = null;

        if (!string.IsNullOrEmpty(executionContext.ActionConfiguration))
        {
            var configuration = 
                JsonSerializer.Deserialize<MinimizeWindowsConfiguration>(executionContext.ActionConfiguration);

            if (configuration != null)
                processName = configuration.ProcessName;
        }

        return Process.GetProcessesByName(processName);
    }
}
