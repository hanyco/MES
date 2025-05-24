using System.Windows.Input;

namespace CodeGenerator.Designer.UI.Common;

/// <summary>
/// A simple implementation of ICommand to relay actions.
/// </summary>
public class RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null) : ICommand
{
    private readonly Func<object?, bool>? _canExecute = canExecute;
    private readonly Action<object?> _execute = execute ?? throw new ArgumentNullException(nameof(execute));

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter)
        => this._canExecute?.Invoke(parameter) ?? true;

    public void Execute(object? parameter)
        => this._execute(parameter);

    /// <summary>
    /// Triggers the CanExecuteChanged event to re-evaluate button enabled state.
    /// </summary>
    public void RaiseCanExecuteChanged()
        => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}