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
using BadEcho.Extensibility.Configuration;
using BadEcho.Interop;
using BadEcho.Presentation;
using BadEcho.Presentation.ViewModels;
using BadEcho.QuickActions.Extensibility;
using BadEcho.QuickActions.Services;
using Microsoft.Extensions.Configuration;

namespace BadEcho.QuickActions.ViewModels;

/// <summary>
/// Provides a view model that displays individual actions, allowing for their manipulation as
/// well as the creation of new ones.
/// </summary>
internal sealed class ActionsViewModel : PolymorphicCollectionViewModel<IAction, IActionViewModel>
{
    private readonly string _pluginDirectory = string.Empty;
    private readonly UserSettingsService? _userSettingsService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ActionsViewModel"/> class.
    /// </summary>
    public ActionsViewModel(UserSettingsService userSettingsService, IConfiguration configuration)
        : this()
    {
        _userSettingsService = userSettingsService;

        Bind(userSettingsService.Scripts);
        Bind(userSettingsService.Actions.OfType<CodeAction>());

        SelectedScriptViewModel = Items.OfType<ScriptActionViewModel>()
                                       .FirstOrDefault();

        SelectedCodeViewModel = Items.OfType<CodeActionViewModel>()
                                     .FirstOrDefault();

        var extensibilityConfiguration = configuration.GetSection(ExtensibilityConfiguration.SectionName)
                                                      .Get<ExtensibilityConfiguration>();
        if (extensibilityConfiguration != null)
            _pluginDirectory = extensibilityConfiguration.GetFullPathToPlugins();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ActionsViewModel"/> class.
    /// </summary>
    /// <remarks>Constructor required so view model can be used as design-time data.</remarks>
    public ActionsViewModel() 
        : base(new CollectionViewModelOptions{OffloadBatchBindings = false})
    {
        RegisterDerivation<ScriptAction, ScriptActionViewModel>(vm => vm.SaveRequested += HandleScriptSaveRequested);
        RegisterDerivation<CodeAction, CodeActionViewModel>();

        ScriptView = new CollectionViewSource { Source = Items }.View;
        ScriptView.Filter = o => o is ScriptActionViewModel;

        CodeView = new CollectionViewSource { Source = Items }.View;
        CodeView.Filter = o => o is CodeActionViewModel;
        
        AddScriptCommand = new DelegateCommand(AddScript);
        DeleteScriptCommand = new DelegateCommand(DeleteScript);
        OpenPluginFolderCommand = new DelegateCommand(OpenPluginFolder);
    }

    /// <summary>
    /// Gets a collection view filtered to include only script actions.
    /// </summary>
    public ICollectionView ScriptView
    { get; }

    /// <summary>
    /// Gets a collection view filtered to include only code actions.
    /// </summary>
    public ICollectionView CodeView
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
    /// Gets or sets the currently selected code action view model.
    /// </summary>
    public CodeActionViewModel? SelectedCodeViewModel
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

    /// <summary>
    /// Gets a command that, when executed, will open up the folder containing code action plugins.
    /// </summary>
    public ICommand OpenPluginFolderCommand
    { get; }

    /// <summary>
    /// Gets a value indicating if this collection view model contains any script actions.
    /// </summary>
    public bool HasScriptItems
    {
        get;
        private set => NotifyIfChanged(ref field, value);
    }

    /// <summary>
    /// Gets a value indicating if this collection view model contains any code actions.
    /// </summary>
    public bool HasCodeItems
    {
        get;
        private set => NotifyIfChanged(ref field, value);
    }

    /// <inheritdoc/>
    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        base.OnCollectionChanged(e);

        switch (e.NewItems?[0])
        {   
            case ScriptActionViewModel newScriptVm:
                // Only script actions can be added or removed by the user via the interface.
                SelectedScriptViewModel = newScriptVm;
                break;
            
            case CodeActionViewModel newCodeVm:
                // This is mainly for design-time support.
                SelectedCodeViewModel = newCodeVm;
                break;
        }

        CheckCollectionItems();
    }

    private void CheckCollectionItems()
    {
        HasScriptItems = Items.OfType<ScriptActionViewModel>().Any();
        HasCodeItems = Items.OfType<CodeActionViewModel>().Any();
    }

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

            viewModel.SaveRequested -= HandleScriptSaveRequested;

            Unbind(viewModel.ActiveModel);
        }
    }

    private void OpenPluginFolder(object? parameter) 
        => Explorer.OpenFolder(_pluginDirectory);

    private void HandleScriptSaveRequested(object? sender, ScriptAction e) 
        => _userSettingsService?.SaveScripts();
}
