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

using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows.Input;
using BadEcho.Interop;
using BadEcho.Presentation;
using BadEcho.Presentation.ViewModels;
using Microsoft.Extensions.Hosting;

namespace BadEcho.QuickActions.ViewModels;

/// <summary>
/// Provides the view model that facilitates the display and execution of a command via Quick Action's Command Prompt.
/// </summary>
internal sealed class PromptViewModel : ViewModel, IDisposable
{
    private readonly StreamWriter _historyWriter = StreamWriter.Null;
    private readonly List<string> _history = [];

    private int _historyIndex;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="PromptViewModel"/> class.
    /// </summary>
    public PromptViewModel(IHostEnvironment environment)
        : this()
    {
        string appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                          environment.ApplicationName);

        string historyPath = Path.Combine(appDataPath, "history.txt");
        
        if (File.Exists(historyPath))
            _history.AddRange(File.ReadAllLines(historyPath));

        _historyIndex = _history.Count;
        _historyWriter = new StreamWriter(new FileStream(historyPath, FileMode.Append, FileAccess.Write, FileShare.Read))
                         {   // A lot of commands would need to be written before the default buffer fills up.
                             // We'll just flush to disk after each write so that the history won't be lost in case the worst happens.
                             AutoFlush = true
                         };
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PromptViewModel"/> class.
    /// </summary>
    public PromptViewModel()
    {
        ExecuteCommand = new DelegateCommand(Execute);
        ResetResultCommand = new DelegateCommand(ResetResult);
        NavigateHistoryCommand = new DelegateCommand(NavigateHistory, CanNavigateHistory);
    }

    /// <summary>
    /// Gets or sets the command to execute if submitted.
    /// </summary>
    public string CommandText
    {
        get;
        set => NotifyIfChanged(ref field, value);
    } = string.Empty;

    /// <summary>
    /// Gets the result of the last command executed via the Command Prompt.
    /// </summary>
    /// <remarks>
    /// The Command Prompt binds to this property to detect when a command has been executed so it can play the appropriate
    /// animation in response.
    /// </remarks>
    public PromptCommandResult CommandResult
    {
        get;
        set => NotifyIfChanged(ref field, value);
    }

    /// <summary>
    /// Gets a command that, when executed, will execute the command described by <see cref="CommandText"/>.
    /// </summary>
    public ICommand ExecuteCommand
    { get; }

    /// <summary>
    /// Gets a command that, when executed, will reset the value <see cref="CommandResult"/> is set to.
    /// </summary>
    /// <remarks>
    /// The Command Prompt executes this to reset the result value after playing a result animation so that it can detect subsequent
    /// command execution outcomes.
    /// </remarks>
    public ICommand ResetResultCommand
    { get; }

    /// <summary>
    /// Gets a command that, when executed, will navigate through the history of commands executed via the Command Prompt.
    /// </summary>
    /// <remarks>
    /// This accepts a parameter that, when <c>true</c>, results in forward navigation; otherwise, backward navigation occurs.
    /// </remarks>
    public ICommand NavigateHistoryCommand
    { get; }

    /// <inheritdoc/>
    public override void Disconnect()
    { }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_disposed)
            return;

        _historyWriter.Dispose();
        _disposed = true;
    }
    
    private void Execute(object? _)
    {
        ShellResult result = Explorer.Open(CommandText);
        
        CommandResult = result == ShellResult.Success ? PromptCommandResult.Succeeded : PromptCommandResult.Failed;
        
        if (result != ShellResult.Success)
            return;

        // Mimics standard terminal behavior; commands already in the history get shifted to the bottom of the pile.
        _history.Remove(CommandText);
        _history.Add(CommandText);
        _historyWriter.WriteLine(CommandText);

        _historyIndex = _history.Count;
    }

    private void ResetResult(object? _)
    {
        CommandResult = PromptCommandResult.None;
    }
    
    private bool CanNavigateHistory([NotNullWhen(true)]object? parameter)
    {
        return parameter != null;
    }

    public void NavigateHistory(object? parameter)
    {
        if (!CanNavigateHistory(parameter))
            return;

        if (_history.Count == 0)
            return;

        bool forward = (bool) parameter;

        // Constrain the index within valid bounds.
        int historyIndex = forward
            ? Math.Min(_history.Count - 1, _historyIndex + 1)
            : Math.Max(0, _historyIndex - 1);

        _historyIndex = historyIndex;
        CommandText = _history[_historyIndex];
    }
}
