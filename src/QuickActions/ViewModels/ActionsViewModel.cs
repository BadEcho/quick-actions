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

using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using BadEcho.Presentation;
using BadEcho.Presentation.ViewModels;
using BadEcho.QuickActions.Extensibility;
using BadEcho.QuickActions.Services;

namespace BadEcho.QuickActions.ViewModels;

/// <summary>
/// Provides a view model that displays individual actions, allowing for their manipulation as
/// well as the creation of new ones.
/// </summary>
internal sealed class ActionsViewModel : PolymorphicCollectionViewModel<IAction, IActionViewModel>
{
    private readonly UserSettingsService? _userSettingsService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ActionsViewModel"/> class.
    /// </summary>
    public ActionsViewModel(UserSettingsService userSettingsService)
        : this()
    {
        _userSettingsService = userSettingsService;

        Bind(userSettingsService.Scripts);

        SelectedScriptViewModel = Items.OfType<ScriptActionViewModel>()
                                       .FirstOrDefault();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ActionsViewModel"/> class.
    /// </summary>
    /// <remarks>Constructor required so view model can be used as design-time data.</remarks>
    public ActionsViewModel() 
        : base(new CollectionViewModelOptions{OffloadBatchBindings = false})
    {
        RegisterDerivation<ScriptAction, ScriptActionViewModel>(vm => vm.SaveRequested += HandleScriptSaveRequested);

        ScriptView = CollectionViewSource.GetDefaultView(Items);
        ScriptView.Filter = o => o is ScriptActionViewModel;
        ScriptView.CollectionChanged += HandleScriptCollectionChanged;

        AddScriptCommand = new DelegateCommand(AddScript);
        DeleteScriptCommand = new DelegateCommand(DeleteScript);
    }

    /// <summary>
    /// Gets a collection view filtered to include only script actions.
    /// </summary>
    public ICollectionView ScriptView
    { get; }

    /// <summary>
    /// Gets or sets the currently selected script action view model.
    /// </summary>
    public ScriptActionViewModel? SelectedScriptViewModel
    {
        get;
        set => NotifyIfChanged(ref field, value);
    }

    /// <summary>
    /// Gets a command that, when executed, will create a new script action.
    /// </summary>
    public ICommand AddScriptCommand 
    { get; }

    /// <summary>
    /// Gets a command that, when executed, will delete an existing script action.
    /// </summary>
    public ICommand DeleteScriptCommand
    { get; }

    private void AddScript(object? parameter)
    {
        var scriptAction = new ScriptAction();

        _userSettingsService?.Add(scriptAction);

        Bind(scriptAction);
    }

    private void DeleteScript(object? parameter)
    {
        var viewModel = (ScriptActionViewModel?) parameter;

        if (viewModel is { ActiveModel: not null })
        {
            _userSettingsService?.Delete(viewModel.ActiveModel);
           
            Unbind(viewModel.ActiveModel);
        }
    }

    private void HandleScriptSaveRequested(object? sender, ScriptAction e) => 
        _userSettingsService?.SaveScripts();

    private void HandleScriptCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                SelectedScriptViewModel = (ScriptActionViewModel?)e.NewItems?[0];

                break;
        }
    }
}
