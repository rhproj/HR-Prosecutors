using System;
using System.Windows.Input;

namespace HR_Prosecutors.Commands
{
    internal class RelayCommand : ICommand
    {
        private readonly Action<object> _Execute;
        private readonly Func<object, bool> _CanExecute;

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _Execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _CanExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        { 
            return _CanExecute?.Invoke(parameter) ?? true;
        }

        public void Execute(object parameter)
        {
            _Execute(parameter);
        }
    }
}
