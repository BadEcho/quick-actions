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

using System.ComponentModel;
using System.Composition;
using BadEcho.Extensions;
using BadEcho.Interop;
using BadEcho.Logging;
using BadEcho.QuickActions.DisplaySettings.Properties;
using BadEcho.QuickActions.Extensibility;

namespace BadEcho.QuickActions.DisplaySettings;


[Export(typeof(IAction))]
internal sealed class ChangePrimaryDisplay : CodeAction
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChangePrimaryDisplay"/> class.
    /// </summary>
    public ChangePrimaryDisplay() 
        : base(new Guid("{8630C7CA-0E38-4135-A53E-64E352AE5006}"), Strings.ChangePrimaryDisplayName)
    { }

    /// <inheritdoc/>
    public override string Description
        => Strings.ChangePrimaryDisplayDescription;

    /// <inheritdoc/>
    public override ActionResult Execute()
    {
        List<Display> displays = Display.Devices.ToList();

        int primaryIndex = displays.IndexOf(Display.Primary);
        Display nextDisplay = displays[(primaryIndex + 1) % displays.Count];

        try
        {
            nextDisplay.MakePrimary();
        }
        catch (Win32Exception winEx)
        {
            string error = Strings.ChangePrimaryDisplayFailed.InvariantFormat(nextDisplay.Name);

            Logger.Error(error, winEx);
            return ActionResult.Fail(this, error);
        }

        return ActionResult.Ok(this);
    }
}
