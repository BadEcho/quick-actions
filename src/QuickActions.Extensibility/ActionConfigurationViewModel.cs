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

using System.Text.Json;
using BadEcho.Presentation.ViewModels;

namespace BadEcho.QuickActions.Extensibility;

/// <summary>
/// Provides a view model that facilitates the display and manipulation of an action's configuration.
/// </summary>
/// <typeparam name="TConfiguration">The type of object containing the action's configuration.</typeparam>
public abstract class ActionConfigurationViewModel<TConfiguration> : ViewModel<TConfiguration>, IActionConfigurationViewModel
    where TConfiguration : new()
{
    private readonly IActionExecutionContext _executionContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="ActionConfigurationViewModel{TConfiguration}"/> class.
    /// </summary>
    /// <param name="executionContext">The context that will be provided to the action upon its execution.</param>
    protected ActionConfigurationViewModel(IActionExecutionContext executionContext)
    {
        Require.NotNull(executionContext, nameof(executionContext));

        _executionContext = executionContext;

        TConfiguration? actionConfiguration = default;

        if (!string.IsNullOrEmpty(executionContext.ActionConfiguration))
            actionConfiguration = JsonSerializer.Deserialize<TConfiguration>(executionContext.ActionConfiguration);

        actionConfiguration ??= new TConfiguration();
        
        Bind(actionConfiguration);
    }

    /// <inheritdoc/>
    public event EventHandler? ConfigurationChanged;

    /// <inheritdoc/>
    protected override void OnPropertyChanged(string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if (ActiveModel == null)
            return;

        _executionContext.ActionConfiguration = JsonSerializer.Serialize(ActiveModel);

        ConfigurationChanged?.Invoke(this, EventArgs.Empty);
    }
}
