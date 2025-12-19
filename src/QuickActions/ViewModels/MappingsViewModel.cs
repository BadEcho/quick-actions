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

using BadEcho.Presentation;
using BadEcho.Presentation.ViewModels;
using BadEcho.QuickActions.Services;
using System.ComponentModel;
using System.Windows.Input;

namespace BadEcho.QuickActions.ViewModels;

/// <summary>
/// Provides a view model that display individual mappings, allowing for their manipulation as
/// well as the creation of new ones.
/// </summary>
internal sealed class MappingsViewModel : CollectionViewModel<Mapping, MappingViewModel>
{
    private readonly ActionsService? _actionService;

    /// <summary>
    /// Initializes a new instance of the <see cref="MappingsViewModel"/> class.
    /// </summary>
    public MappingsViewModel(ActionsService actionService)
        : this()
    {
        _actionService = actionService;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MappingsViewModel"/> class.
    /// </summary>
    public MappingsViewModel()
        : base(new CollectionViewModelOptions { OffloadBatchBindings = false })
    {
        AddMappingCommand = new DelegateCommand(AddMapping);
    }

    /// <summary>
    /// Gets a command that, when executed, will create a new mapping.
    /// </summary>
    public ICommand AddMappingCommand
    { get; }

    /// <summary>
    /// Gets or sets a value indicating if any of the bound mappings contain unsaved changes.
    /// </summary>
    public bool IsDirty
    {
        get;
        set => NotifyIfChanged(ref field, value);
    }

    /// <inheritdoc/>
    public override MappingViewModel CreateItem(Mapping model)
    {
        var viewModel = new MappingViewModel
                        {
                            Actions = [.. _actionService?.Actions ?? []]
                        };

        viewModel.Bind(model);

        return viewModel;
    }

    /// <inheritdoc/>
    public override void UpdateItem(Mapping model)
    {
        var existingChild = FindItem<MappingViewModel>(model);

        existingChild?.Bind(model);
    }

    /// <inheritdoc/>
    protected override void OnItemChanged(MappingViewModel? child, PropertyChangedEventArgs e)
    {
        base.OnItemChanged(child, e);

        if (child is { IsDirty: true })
            IsDirty = true;
    }

    private void AddMapping(object? parameter)
    {
        var mapping = new Mapping();

        Bind(mapping);
    }
}
