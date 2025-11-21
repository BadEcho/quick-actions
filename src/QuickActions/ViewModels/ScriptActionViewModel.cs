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
using BadEcho.Presentation;
using BadEcho.QuickActions.Extensibility;
using BadEcho.QuickActions.Properties;
using Microsoft.Win32;

namespace BadEcho.QuickActions.ViewModels;

/// <summary>
/// Provides a view model that facilitates the display and manipulation of an action that executes a script.
/// </summary>
internal sealed class ScriptActionViewModel : ActionViewModel<ScriptAction>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ScriptActionViewModel"/> class.
    /// </summary>
    public ScriptActionViewModel()
    {
        SaveCommand = new DelegateCommand(SaveAction);
        OpenFileCommand = new DelegateCommand(OpenFile);
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
    /// Gets a command that, when executed opens a file dialog allowing the user to locate the script
    /// on the file system.
    /// </summary>
    public ICommand OpenFileCommand
    { get; }

    /// <summary>
    /// Gets or sets the path to the bound action's script.
    /// </summary>
    public string Path
    {
        get => field ?? string.Empty;
        set
        {
            NotifyIfChanged(ref field, value);

            if (!File.Exists(field))
                MarkInvalid(Strings.ScriptFileNotExist);
            else
                MarkValid();

            Name = System.IO.Path.GetFileName(value);
        }
    }

    /// <summary>
    /// Gets or sets the arguments passed to the bound action's script.
    /// </summary>
    public string Arguments
    {
        get => field ?? string.Empty;
        set => NotifyIfChanged(ref field, value);
    }

    /// <summary>
    /// Gets or sets the type of shell program whose script interpreter is used to execute the bound action's script.
    /// </summary>
    public ShellType ShellType
    {
        get;
        set
        {
            NotifyIfChanged(ref field, value);

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
        get => field ?? string.Empty;
        private set => NotifyIfChanged(ref field, value);
    }

    /// <summary>
    /// Gets or sets a value indicating if <see cref="ShellTypeIcon"/> is composed of literal text.
    /// </summary>
    /// <remarks>
    /// If false, <see cref="ShellTypeIcon"/> is treated as a glyph meant to be used with a symbol font family.
    /// </remarks>
    public bool IsShellTypeIconLiteral
    {
        get;
        private set => NotifyIfChanged(ref field, value);
    }

    /// <summary>
    /// Gets or sets a value indicating if any of the bound script action's fields contain unsaved changes.
    /// </summary>
    public bool IsDirty
    {
        get;
        set => NotifyIfChanged(ref field, value);
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

    private void OpenFile(object? parameter)
    {
        var dialog = new OpenFileDialog
                     {
                         FileName = Path,
                         Filter = "Script Files |*.cmd;*.bat;*.ps1|All files (*.*)|*.*"
                     };

        bool? result = dialog.ShowDialog();

        if (result == true) 
            Path = dialog.FileName;
    }
}
