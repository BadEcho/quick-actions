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

using System.IO;
using System.Windows.Input;
using BadEcho.Extensibility.Hosting;
using BadEcho.Presentation;
using BadEcho.Presentation.Messaging;
using BadEcho.QuickActions.Extensibility;
using BadEcho.QuickActions.Properties;
using BadEcho.QuickActions.Services;

namespace BadEcho.QuickActions.ViewModels;

/// <summary>
/// Provides a view model that facilitates the display and manipulation of a mapping between a key combination and an action.
/// </summary>
internal sealed class MappingViewModel : KeysViewModel<Mapping>
{
    private const string NO_COMPLETION_SOUND = "None";

    private static readonly string _MediaFolder =
        Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Media");

    /// <summary>
    /// Initializes a new instance of the <see cref="MappingViewModel"/> class.
    /// </summary>
    /// <param name="settingsService">Service for the user's settings.</param>
    /// <param name="mediator">A mediator for passing messages to other components.</param>
    public MappingViewModel(UserSettingsService? settingsService, Mediator? mediator)
        : base(settingsService, mediator)
    {
        Actions = [.. settingsService?.Actions ?? []];
        DeleteCommand = new DelegateCommand(DeleteMapping);

        CompletionSounds = [NO_COMPLETION_SOUND, ..Directory.GetFiles(_MediaFolder, "*.wav").Select(GetFileName)];
        SelectedCompletionSound = NO_COMPLETION_SOUND;

        // This gets around a current issue with nullability attributes being ignored inside method groups.
        static string GetFileName(string path)
        {
            return Path.GetFileName(path);
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MappingViewModel"/> class.
    /// </summary>
    public MappingViewModel()
        : this(null, null)
    { }

    /// <summary>
    /// Occurs when the user clicks on the Delete button.
    /// </summary>
    public event EventHandler? DeleteRequested;

    /// <summary>
    /// Gets a command that, when executed, deletes the bound mapping.
    /// </summary>
    public ICommand DeleteCommand
    { get; }

    /// <summary>
    /// Gets a collection of all actions available for selection.
    /// </summary>
    public ICollection<IAction> Actions
    { get; init; }

    /// <summary>
    /// Gets or sets the selected action.
    /// </summary>
    public IAction? SelectedAction
    {
        get;
        set
        {
            NotifyIfChanged(ref field, value);

            if (field == null)
                MarkInvalid(Strings.NoActionSelected);
            else
                MarkValid();

            ActionConfigurationViewModel = LoadConfigurationViewModel(ActiveModel);
        }
    }

    /// <summary>
    /// Gets or sets the view model providing additional configuration options for the selected action, if one exists.
    /// </summary>
    public IActionConfigurationViewModel? ActionConfigurationViewModel
    {
        get;
        set
        {
            field?.ConfigurationChanged -= HandleActionConfigurationChanged;
            NotifyIfChanged(ref field, value);
            field?.ConfigurationChanged += HandleActionConfigurationChanged;
        }
    }

    /// <summary>
    /// Gets or sets the selected completion sound.
    /// </summary>
    public string SelectedCompletionSound
    {
        get;
        set => NotifyIfChanged(ref field, value);
    }

    /// <summary>
    /// Gets a collection of possible completion sounds.
    /// </summary>
    public ICollection<string> CompletionSounds
    { get; init; }

    /// <summary>
    /// Gets or sets a value indicating if any of the bound mapping's fields contain unsaved changes.
    /// </summary>
    public bool IsDirty
    {
        get;
        set => NotifyIfChanged(ref field, value);
    }

    /// <summary>
    /// Commits changes made to the bound mapping.
    /// </summary>
    public void SaveMapping()
    {
        if (ActiveModel == null)
            return;

        ActiveModel.ActionId = SelectedAction?.Id ?? Guid.Empty;
        string? soundPath = SelectedCompletionSound == NO_COMPLETION_SOUND
            ? null
            : Path.Combine(_MediaFolder, SelectedCompletionSound);

        ActiveModel.CompletionSoundPath = soundPath;
        ActiveModel.KeyCombination = KeyCombination;
        ActiveModel.ActionConfiguration = ActionConfigurationViewModel?.ActionConfiguration;

        IsDirty = false;
    }

    /// <inheritdoc/>
    protected override KeyCombination ReadKeyCombination(Mapping model) 
        => model.KeyCombination;

    /// <inheritdoc/>
    protected override void OnBinding(Mapping model)
    {
        base.OnBinding(model);
        
        SelectedAction = Actions.FirstOrDefault(a => a.Id == model.ActionId);
        ActionConfigurationViewModel = LoadConfigurationViewModel(model);

        string completionSound = NO_COMPLETION_SOUND;

        if (!string.IsNullOrEmpty(model.CompletionSoundPath))
            completionSound = Path.GetFileName(model.CompletionSoundPath);

        SelectedCompletionSound = completionSound;

        IsDirty = false;
    }

    /// <inheritdoc/>
    protected override void OnUnbound(Mapping model)
    {
        base.OnUnbound(model);
        
        SelectedAction = null;
        ActionConfigurationViewModel = null;
        SelectedCompletionSound = NO_COMPLETION_SOUND;

        IsDirty = false;
    }

    /// <inheritdoc/>
    protected override void OnPropertyChanged(string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if (propertyName != nameof(IsDirty))
            IsDirty = true;
    }

    private static IActionConfigurationViewModel? LoadConfigurationViewModel(Mapping? mapping)
    {
        if (mapping == null || !PluginHost.IsFilterable(mapping.ActionId))
            return null;

        return PluginHost.ArmedLoad<IActionConfigurationViewModel, IActionExecutionContext>(mapping, mapping.ActionId);
    }

    private void DeleteMapping(object? parameter)
        => DeleteRequested?.Invoke(this, EventArgs.Empty);

    private void HandleActionConfigurationChanged(object? sender, EventArgs e)
        => IsDirty = true;
}
