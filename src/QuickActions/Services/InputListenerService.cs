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

using System.Media;
using System.Windows;
using BadEcho.Hooks;
using BadEcho.Interop;
using BadEcho.Presentation.Messaging;
using BadEcho.QuickActions.Extensibility;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BadEcho.QuickActions.Services;

/// <summary>
/// Provides a service that monitors input, executes actions mapped to detected key combinations,
/// and notifies the application of other events.
/// </summary>
/// <remarks>
/// This implements <see cref="IHostedService"/> instead of deriving from <see cref="BackgroundService"/> 
/// </remarks>
internal sealed class InputListenerService : IHostedService, IAsyncDisposable
{
    private const int CLEAR_KEYS_INTERVAL = 5000;

    private static readonly KeyCombination _AltTabKeys
        = new([VirtualKey.Alt], [VirtualKey.Tab]);

    private readonly HashSet<VirtualKey> _pressedModifierKeys = [];
    private readonly HashSet<VirtualKey> _pressedKeys = [];

    private readonly UserSettingsService _settingsService;
    private readonly Mediator _mediator;
    private readonly ILogger<InputListenerService> _logger;
    private readonly KeyboardSource _keyboard;
    private readonly MouseSource _mouse;
    private readonly Timer _clearKeysTimer;

    private bool _enabled;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="InputListenerService"/> class.
    /// </summary>
    public InputListenerService(UserSettingsService settingsService,
                                Mediator mediator,
                                ILogger<InputListenerService> logger)
    {
        _settingsService = settingsService;
        _mediator = mediator;
        _logger = logger;
        
        _keyboard = new KeyboardSource(KeyboardProcedure);
        _mouse = new MouseSource(MouseProcedure);
        _clearKeysTimer = new Timer(ClearKeysCallback);

        _enabled = _settingsService.ActionsEnabled;
        _mediator.Register(Messages.ChangeListenerStatus, MediateChangeListenerStatus);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// <para>
    /// This installs global keyboard and mouse hook sources which receive messages through message pumps running on other
    /// threads, allowing this to return immediately after hook installation and message pump initialization.
    /// </para>
    /// <para>
    /// This is why it was more appropriate for this service to implement <see cref ="IHostedService"/> directly
    /// rather than derive from <see cref="BackgroundService"/>.
    /// </para>
    /// </remarks> 
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _keyboard.StartAsync();
        await _mouse.StartAsync();
    }

    /// <inheritdoc/>
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _keyboard.StopAsync();
        await _mouse.StopAsync();
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;

        await _keyboard.DisposeAsync();
        await _mouse.DisposeAsync();
        await _clearKeysTimer.DisposeAsync();

        _disposed = true;
    }

    private ProcedureResult MouseProcedure(MouseEvent mouseEvent, int x, int y)
    {
        if (mouseEvent == MouseEvent.LeftButtonDown)
            _mediator.Broadcast(Messages.MouseClicked, new Point(x, y));

        return new ProcedureResult(nint.Zero, true);
    }

    private ProcedureResult KeyboardProcedure(KeyState state, VirtualKey key)
    {
        var result = new ProcedureResult(nint.Zero, true);

        if (!_enabled)
            return result;

        key = key.NormalizeModifiers();

        UpdatePressedKeys(state, key);

        if (state != KeyState.Down || key.IsModifier())
            return result;

        var pressedKeys = new KeyCombination(_pressedModifierKeys, _pressedKeys);

        if (pressedKeys.Equals(_settingsService.PromptKeys))
            _mediator.Broadcast(Messages.ShowPrompt);
        else if (pressedKeys.Equals(_AltTabKeys))
            _mediator.Broadcast(Messages.AltTabbed);
        else
            ProcessPressedKeys(pressedKeys);

        return result;
    }

    private void UpdatePressedKeys(KeyState state, VirtualKey key)
    {
        var keyHash = key.IsModifier() ? _pressedModifierKeys : _pressedKeys;

        if (state == KeyState.Down)
        {
            _clearKeysTimer.Change(CLEAR_KEYS_INTERVAL, Timeout.Infinite);
            keyHash.Add(key);
        }
        else
            keyHash.Remove(key);
    }

    private void ProcessPressedKeys(KeyCombination pressedKeys)
    {
        Mapping? pressedMapping = _settingsService.GetMapping(pressedKeys);

        if (pressedMapping == null)
            return;

        IAction action = _settingsService.GetAction(pressedMapping.ActionId);
        ActionResult actionResult = action.Execute(pressedMapping);

        _logger.ExecutingAction(action.Name);

        if (!actionResult.Success)
            _mediator.Broadcast(Messages.DisplayError, actionResult);
        else if (!string.IsNullOrEmpty(pressedMapping.CompletionSoundPath))
        {
            using (var soundPlayer = new SoundPlayer(pressedMapping.CompletionSoundPath))
            {
                soundPlayer.Load();
                soundPlayer.Play();
            }
        }
    }

    private void ClearKeysCallback(object? state)
    {   // There's always a chance another installed hook may eat a key release event.
        // So, to prevent an instance where a key press never gets released, we periodically
        // check if keys being tracked as pressed are still being pressed.
        ClearReleasedKeys(_pressedKeys);
        ClearReleasedKeys(_pressedModifierKeys);

        if (_pressedKeys.Count != 0 || _pressedModifierKeys.Count != 0)
            _clearKeysTimer.Change(CLEAR_KEYS_INTERVAL, Timeout.Infinite);
        
        static void ClearReleasedKeys(HashSet<VirtualKey> keys)
        {
            foreach (VirtualKey key in keys)
            {
                if (!Keyboard.IsPressed(key))
                    keys.Remove(key);
            }
        }
    }

    private void MediateChangeListenerStatus(bool enabled)
        => _enabled = enabled;
}
