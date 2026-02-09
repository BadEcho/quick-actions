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
using BadEcho.Interop.Audio;
using BadEcho.Logging;
using BadEcho.QuickActions.SystemSettings.Properties;
using BadEcho.QuickActions.Extensibility;

namespace BadEcho.QuickActions.SystemSettings;

/// <summary>
/// Provides an action that toggle the mute state of the user's default input audio device.
/// </summary>
[Export(typeof(IAction))]
internal sealed class ToggleMicrophoneMute : CodeAction
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ToggleMicrophoneMute"/> class.
    /// </summary>
    public ToggleMicrophoneMute() 
        : base(new Guid("{E120FDB5-8385-4C8D-8BC5-93FD64466E56}"), Strings.ToggleMicrophoneMuteName)
    { }

    /// <inheritdoc/>
    public override string Description
        => Strings.ToggleMicrophoneMuteDescription;

    /// <inheritdoc/>
    public override ActionResult Execute()
    {
        var soundManager = new SoundManager();

        try
        {
            AudioDevice? defaultInput = soundManager.DefaultInputDevice;

            if (defaultInput == null)
                return ActionResult.Fail(this, Strings.NoMicrophoneFoundToToggle);

            defaultInput.Mute = !defaultInput.Mute;
        }
        catch (Win32Exception winEx)
        {
            string error = Strings.ToggleMicrophoneMuteFailed;

            Logger.Error(error, winEx);
            return ActionResult.Fail(this, error);
        }

        return ActionResult.Ok(this);
    }
}
