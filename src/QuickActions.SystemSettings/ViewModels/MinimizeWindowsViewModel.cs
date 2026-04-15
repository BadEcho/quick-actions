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
using BadEcho.Extensibility;
using BadEcho.Extensibility.Hosting;
using BadEcho.QuickActions.Extensibility;

namespace BadEcho.QuickActions.SystemSettings.ViewModels;

/// <summary>
/// Provides a configuration view model for the <see cref="MinimizeWindows"/> action.
/// </summary>
[Export(typeof(IActionConfigurationViewModel))]
[Filterable(MinimizeWindows.ActionId, typeof(MinimizeWindowsViewModel))]
internal sealed class MinimizeWindowsViewModel : ActionConfigurationViewModel<MinimizeWindowsConfiguration>
{
    private const string DEPENDENCY_CONTRACT
        = nameof(MinimizeWindowsViewModel) + nameof(LocalDependency);

    /// <summary>
    /// Initializes a new instance of the <see cref="MinimizeWindowsViewModel"/> class.
    /// </summary>
    [ImportingConstructor]
    public MinimizeWindowsViewModel([Import(DEPENDENCY_CONTRACT)] IActionExecutionContext executionContext)
        : base(executionContext)
    { }

    /// <summary>
    /// Gets or sets the name of the process whose windows should be minimized. If this is not specified,
    /// all windows on the current desktop will be minimized.
    /// </summary>
    public string ProcessName
    {
        get;
        set
        {
            ActiveModel?.ProcessName = value;

            NotifyIfChanged(ref field, value);
        }
    } = string.Empty;

    /// <inheritdoc/>
    protected override void OnBinding(MinimizeWindowsConfiguration model)
    {
        ProcessName = model.ProcessName;
    }

    /// <inheritdoc/>
    protected override void OnUnbound(MinimizeWindowsConfiguration model)
    {
        ProcessName = string.Empty;
    }

    /// <summary>
    /// Provides a convention provider that allows for an armed context in which this view model can receive the required
    /// execution context.
    /// </summary>
    [Export(typeof(IConventionProvider))]
    [Filterable(MinimizeWindows.ActionId, typeof(LocalDependency))]
    private sealed class LocalDependency : DependencyRegistry<IActionExecutionContext>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocalDependency"/> class.
        /// </summary>
        public LocalDependency()
            : base(DEPENDENCY_CONTRACT)
        { }

        /// <inheritdoc/>
        public override IActionExecutionContext Dependency
            => LoadDependency();
    }
}
