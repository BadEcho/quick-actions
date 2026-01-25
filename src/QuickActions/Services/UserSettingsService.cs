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

using BadEcho.Extensibility.Hosting;
using BadEcho.Extensions;
using BadEcho.Interop;
using BadEcho.QuickActions.Extensibility;
using BadEcho.QuickActions.Options;
using Microsoft.Extensions.Hosting;

namespace BadEcho.QuickActions.Services;

/// <summary>
/// Provide a service for retrieving and updating the user's configuration settings.
/// </summary>
internal sealed class UserSettingsService
{
    private readonly Dictionary<HashSet<VirtualKey>, Mapping> _mappingsMap =
        new(HashSet<VirtualKey>.CreateSetComparer());

    private readonly Dictionary<Guid, IAction> _actionsMap;

    private readonly IWritableOptions<ScriptActionsOptions> _scriptOptions;
    private readonly IWritableOptions<MappingOptions> _mappingOptions;
    private readonly IWritableOptions<AppearanceOptions> _appearanceOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserSettingsService"/> class.
    /// </summary>
    public UserSettingsService(IHostApplicationLifetime hostLifetime,
                               IWritableOptions<ScriptActionsOptions> scriptOptions,
                               IWritableOptions<MappingOptions> mappingOptions,
                               IWritableOptions<AppearanceOptions> appearanceOptions)
    {
        _scriptOptions = scriptOptions;
        _mappingOptions = mappingOptions;
        _appearanceOptions = appearanceOptions;

        hostLifetime.ApplicationStopping.Register(OnApplicationStopping);

        IEnumerable<IAction> codeActions = PluginHost.Load<IAction>();
        _actionsMap = Scripts.Concat(codeActions).ToDictionary(kv => kv.Id);

        BuildMappingsMap();
    }

    public IEnumerable<IAction> Actions
        => _actionsMap.Values;

    /// <summary>
    /// Gets the script actions configured by the user.
    /// </summary>
    public IEnumerable<ScriptAction> Scripts
        => _scriptOptions.CurrentValue;

    public IEnumerable<Mapping> Mappings
        => _mappingOptions.CurrentValue;

    /// <summary>
    /// Gets the configuration settings for the user interface.
    /// </summary>
    public AppearanceOptions Appearance
        => _appearanceOptions.CurrentValue;

    /// <summary>
    /// Gets an action by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the action to get.</param>
    /// <returns>The <see cref="IAction"/> instance identified by <c>id</c>.</returns>
    public IAction GetAction(Guid id)
        => _actionsMap[id];

    /// <summary>
    /// Adds a new script action to the user's configuration settings.
    /// </summary>
    /// <param name="action">The script action to add.</param>
    public void Add(ScriptAction action)
    {
        _scriptOptions.CurrentValue.Add(action);
        _actionsMap.Add(action.Id, action);
    }

    /// <summary>
    /// Deletes a script action from the user's configuration settings.
    /// </summary>
    /// <param name="action">The script action to delete.</param>
    public void Delete(ScriptAction action)
    {
        if (_scriptOptions.CurrentValue.Remove(action))
        {
            _actionsMap.Remove(action.Id);
            SaveScripts();
        }
    }

    /// <summary>
    /// Persists the current configuration for the user's script actions.
    /// </summary>
    public void SaveScripts()
        => _scriptOptions.Save(null);

    /// <summary>
    /// Retrieves the mapping associated with the specified keys, if one exists.
    /// </summary>
    /// <param name="modifierKeys">The set of modifier keys associated with the mapping.</param>
    /// <param name="keys">The set of non-modifier keys associated with the mapping.</param>
    /// <returns>
    /// The <see cref="Mapping"/> instance associated with <c>modifierKeys</c> and <c>keys</c> if one exists;
    /// otherwise, null.
    /// </returns>
    public Mapping? GetMapping(HashSet<VirtualKey> modifierKeys, HashSet<VirtualKey> keys)
        => _mappingsMap.GetValueOrDefault([.. modifierKeys, .. keys]);

    /// <summary>
    /// Adds a new mapping to the user's configuration settings.
    /// </summary>
    /// <param name="mapping">The mapping to add.</param>
    public void Add(Mapping mapping)
        => _mappingOptions.CurrentValue.Add(mapping);

    /// <summary>
    /// Deletes a mapping from the user's configuration settings.
    /// </summary>
    /// <param name="mapping">The mapping to delete.</param>
    public void Delete(Mapping mapping)
    {
        if (_mappingOptions.CurrentValue.Remove(mapping))
            SaveMappings();
    }

    /// <summary>
    /// Persists the current configuration for the user's mappings.
    /// </summary>
    public void SaveMappings()
    {
        _mappingOptions.Save(null);
        BuildMappingsMap();
    }

    /// <summary>
    /// Persists the current configuration for the user interface.
    /// </summary>
    public void SaveAppearance()
        => _appearanceOptions.Save(null);

    private void BuildMappingsMap()
    {
        _mappingsMap.Clear();

        foreach (var mapping in _mappingOptions.CurrentValue)
        {
            _mappingsMap.Add([.. mapping.ModifierKeys, .. mapping.Keys], mapping);
        }
    }

    private void OnApplicationStopping()
        => SaveAppearance();
}
