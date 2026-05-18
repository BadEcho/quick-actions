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

using BadEcho.Interop;
using BadEcho.Presentation;
using BadEcho.Presentation.Messaging;
using BadEcho.Presentation.ViewModels;
using BadEcho.QuickActions.Properties;
using BadEcho.QuickActions.Services;
using System.Windows.Input;

namespace BadEcho.QuickActions.ViewModels;

/// <summary>
/// Provides a view model that facilitates the display and manipulation of a key combination.
/// </summary>
/// <inheritdoc cref="ViewModel{T}" path="/typeparam[@name='T']"/>
internal abstract class KeysViewModel<T> : ViewModel<T>
{
    private readonly UserSettingsService? _settingsService;
    private readonly Mediator? _mediator;

    /// <summary>
    /// Initializes a new instance of the <see cref="KeysViewModel{T}"/> class.
    /// </summary>
    protected KeysViewModel(UserSettingsService? settingsService, Mediator? mediator)
        : this()
    {
        _settingsService = settingsService;
        _mediator = mediator;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="KeysViewModel{T}"/> class.
    /// </summary>
    protected KeysViewModel()
    {
        KeyInputCommand = new DelegateCommand(ProcessKeyInput);
        PauseListenerCommand = new DelegateCommand(PauseListener);
        ResumeListenerCommand = new DelegateCommand(ResumeListener);
    }

    /// <summary>
    /// Gets a command that, when executed, adds or removes a key press to or from the key combination based on the incoming input.
    /// </summary>
    public ICommand KeyInputCommand
    { get; }

    /// <summary>
    /// Gets a command that, when executed, pauses the input listener service, preventing the execution of any mapped actions.
    /// </summary>
    public ICommand PauseListenerCommand
    { get; }

    /// <summary>
    /// Gets a command that, when executed, resumes the input listener service, allowing the execution of any mapped actions.
    /// </summary>
    public ICommand ResumeListenerCommand
    { get; }

    /// <summary>
    /// Gets or sets the text describing the key combination.
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
    /// Gets a copy of the key combination provided during binding.
    /// </summary>
    /// <remarks>
    /// Because this is a separate instance from the <see cref="KeyCombination"/>, any changes to it need
    /// to be propagated to the original instance to save the updated combination; this is done
    /// purposely so that changes are not committed until an explicit save happens.
    /// </remarks>
    protected KeyCombination KeyCombination
    { get; private set; } = new();

    /// <summary>
    /// Reads the key combination from the model data.
    /// </summary>
    /// <param name="model">Data associated with the view model.</param>
    /// <returns>The <see cref="KeyCombination"/> instance read from <c>model</c>.</returns>
    protected abstract KeyCombination ReadKeyCombination(T model);

    /// <inheritdoc/>
    protected override void OnBinding(T model)
    {
        KeyCombination keyCombination = ReadKeyCombination(model);
        KeyCombination = new KeyCombination(keyCombination.ModifierKeys, keyCombination.Keys);

        KeysText = KeyCombination.ToString();
    }

    /// <inheritdoc/>
    protected override void OnUnbound(T model)
    {
        KeyCombination = new KeyCombination();
        KeysText = string.Empty;
    }

    private void ProcessKeyInput(object? parameter)
    {
        if (ActiveModel == null || parameter is not KeyEventArgs keyArgs)
            return;

        VirtualKey key =
            (VirtualKey) KeyInterop.VirtualKeyFromKey(keyArgs.Key == Key.System ? keyArgs.SystemKey : keyArgs.Key);

        if (key == VirtualKey.Tab)
            return;

        keyArgs.Handled = true;
        
        if (key == VirtualKey.Backspace)
        {
            if (KeyCombination.Keys.Count > 0)
                KeyCombination.Keys.Remove(KeyCombination.Keys.Last());
            else if(KeyCombination.ModifierKeys.Count > 0)
                KeyCombination.ModifierKeys.Remove(KeyCombination.ModifierKeys.Last());
        }
        else
        {
            key = key.NormalizeModifiers();

            if (key.IsModifier())
                KeyCombination.ModifierKeys.Add(key);
            else
                KeyCombination.Keys.Add(key);
        }

        KeysText = KeyCombination.ToString();
    }

    private void PauseListener(object? _)
        => _mediator?.Broadcast(Messages.ChangeListenerStatus, false);

    private void ResumeListener(object? _)
    {   // Only re-enable if input listening isn't explicitly disabled in the settings.
        if (_settingsService?.ActionsEnabled ?? true)
            _mediator?.Broadcast(Messages.ChangeListenerStatus, true);
    }
}
