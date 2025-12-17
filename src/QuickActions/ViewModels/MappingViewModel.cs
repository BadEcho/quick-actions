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

using System.Windows.Input;
using BadEcho.Interop;
using BadEcho.Presentation;
using BadEcho.Presentation.ViewModels;

namespace BadEcho.QuickActions.ViewModels;

/// <summary>
/// Provides a view model that facilitates the display and manipulation of a mapping between a key combination and an action.
/// </summary>
internal sealed class MappingViewModel : ViewModel<Mapping>
{
    public MappingViewModel()
    {
        KeyInputCommand = new DelegateCommand(ProcessKeyInput);
    }

    /// <summary>
    /// Gets a command that, when executed, adds or removes a key press to the bound mapping based on the incoming input.
    /// </summary>
    public ICommand KeyInputCommand
    { get; }

    /// <summary>
    /// Gets or sets the text describing the bound mapping's key combination.
    /// </summary>
    public string MappingText
    {
        get;
        set => NotifyIfChanged(ref field, value);
    } = string.Empty;

    /// <inheritdoc/>
    protected override void OnBinding(Mapping model)
    {
        MappingText = DescribeMapping(model);
    }

    /// <inheritdoc/>
    protected override void OnUnbound(Mapping model)
    {
        MappingText = string.Empty;
    }

    private void ProcessKeyInput(object? parameter)
    {
        if (ActiveModel == null || parameter is not KeyEventArgs keyArgs)
            return;

        keyArgs.Handled = true;
        
        VirtualKey key = (VirtualKey) KeyInterop.VirtualKeyFromKey(keyArgs.Key);

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

            switch (key)
            {
                case VirtualKey.Alt:
                case VirtualKey.Control:
                case VirtualKey.Shift:
                    ActiveModel.ModifierKeys.Add(key);
                    break;

                default:
                    ActiveModel.Keys.Add(key);
                    break;
            }
        }
        
        MappingText = DescribeMapping(ActiveModel);
    }

    private static string DescribeMapping(Mapping mapping)
    {
        var keySequence = mapping.ModifierKeys
                                 .Select(Enum.GetName)
                                 .Concat(mapping.Keys.Select(Enum.GetName));
        
        string description = string.Join(" + ", keySequence);

        return description;
    }
}
