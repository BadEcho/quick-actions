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

using System.Media;
using BadEcho.Hooks;
using BadEcho.Interop;
using BadEcho.Presentation.Messaging;
using BadEcho.QuickActions.Extensibility;
using Microsoft.Extensions.Hosting;

namespace BadEcho.QuickActions.Services;

/// <summary>
/// Provides a service that monitors keyboard input and executes actions mapped to detected key combinations.
/// </summary>
/// <remarks>
/// This implements <see cref="IHostedService"/> instead of deriving from <see cref="BackgroundService"/> 
/// </remarks>
internal sealed class KeyboardListenerService : IHostedService, IAsyncDisposable
{
    private readonly KeyboardSource _keyboard;
    private readonly HashSet<VirtualKey> _pressedModifierKeys = [];
    private readonly HashSet<VirtualKey> _pressedKeys = [];
    private readonly UserSettingsService _userSettingsService;
    private readonly Mediator _mediator;

    private bool _paused;
    private bool _disposed;

    public KeyboardListenerService(UserSettingsService userSettingsService, Mediator mediator)
    {
        _userSettingsService = userSettingsService;
        _mediator = mediator;
        _keyboard = new KeyboardSource(KeyboardProcedure);

        _mediator.Register(Messages.PauseListener, MediatePauseListener);
        _mediator.Register(Messages.ResumeListener, MediateResumeListener);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// <para>
    /// This installs a global keyboard hook source that receives messages through a message pump running on another
    /// thread, allowing this to return immediately after hook installation and message pump initialization.
    /// </para>
    /// <para>
    /// This is why it was more appropriate for this service to implement <see cref ="IHostedService"/> directly
    /// rather than derive from <see cref="BackgroundService"/>.
    /// </para>
    /// </remarks> 
    public async Task StartAsync(CancellationToken cancellationToken) 
        => await _keyboard.StartAsync();

    /// <inheritdoc/>
    public async Task StopAsync(CancellationToken cancellationToken) 
        => await _keyboard.StopAsync();

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;

        await _keyboard.DisposeAsync();

        _disposed = true;
    }

    private ProcedureResult KeyboardProcedure(KeyState state, VirtualKey key)
    {
        if (_paused)
            return new ProcedureResult(nint.Zero, true);

        key = key.NormalizeModifiers();

        UpdatePresses(state, key);

        if (state == KeyState.Down && !key.IsModifier())
        {
            Mapping? pressedMapping = _userSettingsService.GetMapping(_pressedModifierKeys, _pressedKeys);

            if (pressedMapping != null)
            {
                IAction action = _userSettingsService.GetAction(pressedMapping.ActionId);
                ActionResult result = action.Execute();

                if (!result.Success)
                {
                    _mediator.Broadcast(Messages.DisplayError, result);
                }
                else if(!string.IsNullOrEmpty(pressedMapping.CompletionSoundPath))
                {
                    using (var soundPlayer = new SoundPlayer(pressedMapping.CompletionSoundPath))
                    {
                        soundPlayer.Load();
                        soundPlayer.Play();
                    }
                }
            }
        }
        
        return new ProcedureResult(nint.Zero, true);
    }

    private void UpdatePresses(KeyState state, VirtualKey key)
    {
        var keyHash = key.IsModifier() ? _pressedModifierKeys : _pressedKeys;

        if (state == KeyState.Down)
            keyHash.Add(key);
        else
            keyHash.Remove(key);
    }

    private void MediatePauseListener()
        => _paused = true;

    private void MediateResumeListener()
        => _paused = false;
}
