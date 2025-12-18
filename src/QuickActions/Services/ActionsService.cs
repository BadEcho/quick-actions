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
using BadEcho.QuickActions.Extensibility;

namespace BadEcho.QuickActions.Services;

/// <summary>
/// Provides a service for efficiently looking up actions and their mappings.
/// </summary>
internal sealed class ActionsService
{
    private readonly UserSettingsService _userSettingsService;
    private readonly List<IAction> _codeActions;
    private readonly Dictionary<Guid, IAction> _actionMap;

    public ActionsService(UserSettingsService userSettingsService)
    {
        _userSettingsService = userSettingsService;

        _codeActions = [..PluginHost.Load<IAction>()];

        _actionMap = userSettingsService.Scripts.Concat(_codeActions).ToDictionary(kv => kv.Id);
    }

    public IEnumerable<IAction> Actions
        => _actionMap.Values;

    public IAction GetAction(Guid id)
        => _actionMap[id];
}