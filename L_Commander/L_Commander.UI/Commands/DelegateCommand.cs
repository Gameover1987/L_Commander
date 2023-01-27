using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace L_Commander.UI.Commands
{
    public class DelegateCommand : IDelegateCommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;
        private readonly Action<Exception> _exceptionAction;
        private event EventHandler _canExecuteChanged;

        public DelegateCommand(Action<object> execute)
            : this(execute, null, DefaultExceptionHandler)
        {
        }

        public DelegateCommand(Action execute)
            : this(x => execute(), null, DefaultExceptionHandler)
        {
        }


        public DelegateCommand(Action<object> execute, Func<object, bool> canExecute)
            : this(execute, canExecute, DefaultExceptionHandler)
        {
        }

        public DelegateCommand(Action execute, Func<bool> canExecute)
            : this(x => execute(), x => canExecute(), DefaultExceptionHandler)
        {
        }

        public DelegateCommand(Func<bool> canExecute)
            : this(() => { }, canExecute)
        {

        }

        public DelegateCommand(Action<object> execute, Func<object, bool> canExecute, Action<Exception> exceptionAction)
        {
            if (exceptionAction == null) throw new ArgumentNullException(nameof(exceptionAction));
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
            _exceptionAction = exceptionAction ?? DefaultExceptionHandler;
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                _canExecuteChanged += value;
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                _canExecuteChanged -= value;
                CommandManager.RequerySuggested -= value;
            }
        }

        public event EventHandler Executed;

        public bool TryExecute()
        {
            return TryExecute(null);
        }

        public bool TryExecute(object obj)
        {
            if (CanExecute(obj))
            {
                Execute(obj);
                return true;
            }

            return false;
        }

        public bool CanExecute()
        {
            return CanExecute(null);
        }

        public void Execute()
        {
            Execute(null);
        }

        public bool CanExecute(object parameter)
        {
            try
            {
                return _canExecute == null || _canExecute(parameter);
            }
            catch (Exception e)
            {
                _exceptionAction(e);
                return false;
            }
        }

        public void Execute(object parameter)
        {
            try
            {
                _execute(parameter);

                if (Executed != null)
                    Executed(this, new EventArgs());
            }
            catch (Exception e)
            {
                _exceptionAction(e);
            }
        }

        public void NotifyCanExecuteChanged()
        {
            if (_canExecuteChanged != null)
            {
                if (Application.Current?.Dispatcher.CheckAccess() == true)
                    _canExecuteChanged(this, EventArgs.Empty);
                else
                    Application.Current?.Dispatcher.InvokeAsync(() => _canExecuteChanged(this, EventArgs.Empty), DispatcherPriority.Normal);
            }
        }

        public static EventHandler<ExceptionEventArgs> CommandException;

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void DefaultExceptionHandler(Exception ex)
        {
            if (!Application.Current?.Dispatcher.CheckAccess() == true)
            {
                Application.Current?.Dispatcher.InvokeAsync(() => DefaultExceptionHandler(ex), DispatcherPriority.Normal);
                return;
            }

            Exception initialException = ex;

            while (initialException.InnerException != null)
                initialException = initialException.InnerException;

            CommandException?.Invoke(null, new ExceptionEventArgs(ex));
        }

        public static void NotifyCanExecuteChangedForAll()
        {
            if (Application.Current?.Dispatcher.CheckAccess() == true)
                CommandManager.InvalidateRequerySuggested();
            else
                Application.Current?.Dispatcher.InvokeAsync(CommandManager.InvalidateRequerySuggested, DispatcherPriority.Normal);
        }
    }
}
