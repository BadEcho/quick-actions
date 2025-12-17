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

using BadEcho.Presentation.ViewModels;

namespace BadEcho.QuickActions.ViewModels;

/// <summary>
/// Provides a view model that display individual mappings, allowing for their manipulation as
/// well as the creation of new ones.
/// </summary>
internal sealed class MappingsViewModel : CollectionViewModel<Mapping, MappingViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MappingsViewModel"/> class.
    /// </summary>
    public MappingsViewModel()
        : base(new CollectionViewModelOptions { OffloadBatchBindings = false })
    { }

    /// <inheritdoc/>
    public override MappingViewModel CreateItem(Mapping model)
    {
        var viewModel = new MappingViewModel();

        viewModel.Bind(model);

        return viewModel;
    }

    /// <inheritdoc/>
    public override void UpdateItem(Mapping model)
    {
        var existingChild = FindItem<MappingViewModel>(model);

        existingChild?.Bind(model);
    }
}
