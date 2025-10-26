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

using BadEcho.Extensions;
using BadEcho.QuickActions.Extensibility;
using BadEcho.QuickActions.Options;

namespace BadEcho.QuickActions.Services;

/// <summary>
/// Provide a service for retrieving and updating the user's configuration settings.
/// </summary>
internal sealed class UserSettingsService
{
    private readonly IWritableOptions<ScriptActionsOptions> _scriptOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserSettingsService"/> class.
    /// </summary>
    public UserSettingsService(IWritableOptions<ScriptActionsOptions> scriptOptions)
    {
        _scriptOptions = scriptOptions;
    }

    /// <summary>
    /// Gets the script actions configured by the user.
    /// </summary>
    public IEnumerable<ScriptAction> Scripts
    {

        get
        {

            return _scriptOptions.CurrentValue;
        }
    }

    /// <summary>
    /// Adds a new script action to the user's configuration settings.
    /// </summary>
    /// <param name="action">The script action to add.</param>
    public void Add(ScriptAction action)
    {
        _scriptOptions.CurrentValue.Add(action);
    }

    /// <summary>
    /// Deletes a script action from the user's configuration settings.
    /// </summary>
    /// <param name="action">The script action to delete.</param>
    public void Delete(ScriptAction action)
    {
        if (_scriptOptions.CurrentValue.Remove(action))
            Save();
    }

    /// <summary>
    /// Persists the current state of the user's configuration settings.
    /// </summary>
    public void Save() 
        => _scriptOptions.Save(null);
}
