using P43.Wpf.Infrastructure.Commands.Base;

namespace P43.Wpf.Infrastructure.Commands;
public class RelayCommand(Action<object?> execute, Func<object?, bool> canExecute) : Command
{
    private readonly Action<object?> _execute = execute;
    private readonly Func<object?, bool> _canExecute = canExecute;

    public override bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? false;
    public override void Execute(object? parameter) => _execute(parameter);
}