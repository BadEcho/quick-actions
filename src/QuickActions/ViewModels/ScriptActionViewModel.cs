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
using BadEcho.Presentation;
using BadEcho.QuickActions.Extensibility;

namespace BadEcho.QuickActions.ViewModels;

/// <summary>
/// Provides a view model that facilitates the display and manipulation of an action that executes a script.
/// </summary>
internal sealed class ScriptActionViewModel : ActionViewModel<ScriptAction>
{
    private string _path = string.Empty;
    private string _arguments = string.Empty;
    
    private ShellType _shellType;
    private string _shellTypeIcon = string.Empty;
    private bool _isShellTypeIconLiteral;
    private bool _isDirty;

    /// <summary>
    /// Initializes a new instance of the <see cref="ScriptActionViewModel"/> class.
    /// </summary>
    public ScriptActionViewModel()
    {
        SaveCommand = new DelegateCommand(SaveAction);
    }

    /// <summary>
    /// Occurs when the user clicks on the Save button.
    /// </summary>
    public event EventHandler<ScriptAction>? SaveRequested;

    /// <summary>
    /// Gets a command that, when executed, saves the bound script action.
    /// </summary>
    public ICommand SaveCommand 
    { get; }

    /// <summary>
    /// Gets or sets the path to the bound action's script.
    /// </summary>
    public string Path
    {
        get => _path;
        set
        {
            NotifyIfChanged(ref _path, value);

            Name = System.IO.Path.GetFileName(value);
        }
    }

    /// <summary>
    /// Gets or sets the arguments passed to the bound action's script.
    /// </summary>
    public string Arguments
    {
        get => _arguments;
        set => NotifyIfChanged(ref _arguments, value);
    }

    /// <summary>
    /// Gets or sets the type of shell program whose script interpreter is used to execute the bound action's script.
    /// </summary>
    public ShellType ShellType
    {
        get => _shellType;
        set
        {
            NotifyIfChanged(ref _shellType, value);

            switch (value)
            {
                case ShellType.Command:
                    IsShellTypeIconLiteral = false;
                    ShellTypeIcon = "\uE756";
                    break;

                case ShellType.PowerShell:
                    IsShellTypeIconLiteral = true;
                    ShellTypeIcon = "Ps";
                    break;
            }
        }
    }

    /// <summary>
    /// Gets or sets text representing the icon for the shell type of the bound action's script.
    /// </summary>
    public string ShellTypeIcon
    {
        get => _shellTypeIcon;
        private set => NotifyIfChanged(ref _shellTypeIcon, value);
    }

    /// <summary>
    /// Gets or sets a value indicating if <see cref="ShellTypeIcon"/> is composed of literal text.
    /// </summary>
    /// <remarks>
    /// If false, <see cref="ShellTypeIcon"/> is treated as a glyph meant to be used with a symbol font family.
    /// </remarks>
    public bool IsShellTypeIconLiteral
    {
        get => _isShellTypeIconLiteral;
        private set => NotifyIfChanged(ref _isShellTypeIconLiteral, value);
    }

    /// <summary>
    /// Gets or sets a value indicating if any of the bound script action's fields contain unsaved changes.
    /// </summary>
    public bool IsDirty
    {
        get => _isDirty;
        set => NotifyIfChanged(ref _isDirty, value);
    }

    /// <inheritdoc/>
    protected override void OnBinding(ScriptAction model)
    {
        base.OnBinding(model);
        
        Path = model.Path;
        ShellType = model.ShellType;
        Arguments = model.Arguments;

        IsDirty = false;
    }

    /// <inheritdoc/>
    protected override void OnUnbound(ScriptAction model)
    {
        base.OnUnbound(model);

        ShellType = default;
        Path = string.Empty;
        Arguments = string.Empty;

        IsDirty = false;
    }

    /// <inheritdoc/>
    protected override void OnPropertyChanged(string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if (propertyName != nameof(IsDirty))
            IsDirty = true;
    }

    private void SaveAction(object? parameter)
    {
        if (ActiveModel == null)
            return;

        ActiveModel.Name = Name;
        ActiveModel.Path = Path;
        ActiveModel.Arguments = Arguments;
        ActiveModel.ShellType = ShellType;

        SaveRequested?.Invoke(this, ActiveModel);

        IsDirty = false;
    }
}
