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

using BadEcho.QuickActions.Extensibility;

namespace BadEcho.QuickActions.ViewModels;

/// <summary>
/// Provides a view model that facilitates the display of an action defined in code, imported as a plugin.
/// </summary>
internal sealed class CodeActionViewModel : ActionViewModel<CodeAction>
{
    /// <summary>
    /// Gets or sets a description from the plugin as to what the bound action does.
    /// </summary>
    public string Description
    {
        get => field ?? string.Empty;
        set => NotifyIfChanged(ref field, value);
    }

    /// <inheritdoc/>
    protected override void OnBinding(CodeAction model)
    {
        base.OnBinding(model);

        Description = model.Description;
    }

    /// <inheritdoc/>
    protected override void OnUnbound(CodeAction model)
    {
        base.OnUnbound(model);

        Description = string.Empty;
    }
}
