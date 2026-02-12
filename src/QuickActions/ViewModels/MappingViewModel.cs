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

using System.IO;
using System.Windows.Input;
using BadEcho.Interop;
using BadEcho.Presentation;
using BadEcho.Presentation.ViewModels;
using BadEcho.QuickActions.Extensibility;
using BadEcho.QuickActions.Properties;

namespace BadEcho.QuickActions.ViewModels;

/// <summary>
/// Provides a view model that facilitates the display and manipulation of a mapping between a key combination and an action.
/// </summary>
internal sealed class MappingViewModel : ViewModel<Mapping>
{
    private const string NO_COMPLETION_SOUND = "None";

    private static readonly string _MediaFolder =
        Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Media");

    /// <summary>
    /// Initializes a new instance of the <see cref="MappingViewModel"/> class.
    /// </summary>
    public MappingViewModel(IEnumerable<IAction> actions)
        : this()
    {
        Actions = actions.ToList();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MappingViewModel"/> class.
    /// </summary>
    public MappingViewModel()
    {
        KeyInputCommand = new DelegateCommand(ProcessKeyInput);
        DeleteCommand = new DelegateCommand(DeleteMapping);
        Actions = [];

        CompletionSounds = [NO_COMPLETION_SOUND, ..Directory.GetFiles(_MediaFolder, "*.wav").Select(GetFileName)];
        SelectedCompletionSound = NO_COMPLETION_SOUND;

        // This gets around a current issue with nullability attributes being ignored inside method groups.
        static string GetFileName(string path)
        {
            return Path.GetFileName(path);
        }
    }

    /// <summary>
    /// Occurs when the user clicks on the Delete button.
    /// </summary>
    public event EventHandler? DeleteRequested; 

    /// <summary>
    /// Gets a command that, when executed, adds or removes a key press to the bound mapping based on the incoming input.
    /// </summary>
    public ICommand KeyInputCommand
    { get; }

    /// <summary>
    /// Gets a command that, when executed, deletes the bound mapping.
    /// </summary>
    public ICommand DeleteCommand
    { get; }

    /// <summary>
    /// Gets or sets the text describing the bound mapping's key combination.
    /// </summary>
    public string KeysText
    {
        get;
        set
        {
            NotifyIfChanged(ref field, value);

            if (string.IsNullOrEmpty(field))
                MarkInvalid(Strings.KeysTextEmpty);
            else
                MarkValid();
        }
    } = string.Empty;

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

            ActiveModel?.ActionId = value?.Id ?? Guid.Empty;
        }
    }

    /// <summary>
    /// Gets a collection of all actions available for selection.
    /// </summary>
    public ICollection<IAction> Actions
    { get; init; }

    /// <summary>
    /// Gets or sets the selected completion sound.
    /// </summary>
    public string SelectedCompletionSound
    {
        get;
        set
        {
            NotifyIfChanged(ref field, value);

            string? soundPath = value == NO_COMPLETION_SOUND ? null : Path.Combine(_MediaFolder, value);

            ActiveModel?.CompletionSoundPath = soundPath;
        }
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

    /// <inheritdoc/>
    protected override void OnBinding(Mapping model)
    {
        KeysText = DescribeMapping(model);
        SelectedAction = Actions.FirstOrDefault(a => a.Id == model.ActionId);

        string completionSound = NO_COMPLETION_SOUND;

        if (!string.IsNullOrEmpty(model.CompletionSoundPath))
            completionSound = Path.GetFileName(model.CompletionSoundPath);

        SelectedCompletionSound = completionSound;

        IsDirty = false;
    }

    /// <inheritdoc/>
    protected override void OnUnbound(Mapping model)
    {
        KeysText = string.Empty;
        SelectedAction = null;
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

    private static string DescribeMapping(Mapping mapping)
    {
        var keySequence = mapping.ModifierKeys
                                 .Select(Enum.GetName)
                                 .Concat(mapping.Keys.Select(Enum.GetName));
        
        string description = string.Join(" + ", keySequence);

        return description;
    }

    private void ProcessKeyInput(object? parameter)
    {
        if (ActiveModel == null || parameter is not KeyEventArgs keyArgs)
            return;

        VirtualKey key = (VirtualKey) KeyInterop.VirtualKeyFromKey(keyArgs.Key);

        if (key == VirtualKey.Tab)
            return;

        keyArgs.Handled = true;

        if (key == VirtualKey.Backspace)
        {
            if (ActiveModel.Keys.Count > 0)
                ActiveModel.Keys.Remove(ActiveModel.Keys.Last());
            else
                ActiveModel.ModifierKeys.Remove(ActiveModel.ModifierKeys.Last());
        }
        else
        {
            key = key.NormalizeModifiers();

            if (key.IsModifier())
                ActiveModel.ModifierKeys.Add(key);
            else
                ActiveModel.Keys.Add(key);
        }
        
        KeysText = DescribeMapping(ActiveModel);
    }

    private void DeleteMapping(object? parameter)
        => DeleteRequested?.Invoke(this, EventArgs.Empty);
}
