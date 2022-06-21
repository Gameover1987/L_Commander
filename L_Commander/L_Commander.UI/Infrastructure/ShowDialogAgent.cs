using System;
using System.Windows;
using L_Commander.UI.ViewModels;
using L_Commander.UI.Windows;
using Microsoft.Win32;

namespace L_Commander.UI.Infrastructure
{
    /// <summary>
    /// Реализация поднятия диалогов
    /// </summary>
    public class ShowDialogAgent : IShowDialogAgent
    {
        private readonly IDispatcher _dispatcher;
        private Window _theActiveWindow;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="dispatcher"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public ShowDialogAgent(IDispatcher dispatcher)
        {
            if (dispatcher == null) throw new ArgumentNullException("dispatcher");
            _dispatcher = dispatcher;
            DefaultWindowTitle = string.Empty;
        }

        /// <summary>
        /// Заголовок окна по умолчанию для отображения в ShowMessageDialog если не удалось получить активное окно
        /// </summary>
        public string DefaultWindowTitle { get; set; }

        /// <summary>
        /// Главное окно приложения
        /// </summary>
        private Window MainWindow
        {
            get
            {
                if (!_dispatcher.CheckAccess())
                {
                    return _dispatcher.Invoke(new Func<Window>(() => MainWindow)) as Window;
                }
                return Application.Current == null ? null : Application.Current.MainWindow;
            }
        }

        /// <summary>
        /// Текущее активное окно исходя из открытых через этот интерфейс. 
        /// Если нет открытых то MainWindow
        /// </summary>
        protected virtual Window ActiveWindow
        {
            get { return _theActiveWindow ?? (_theActiveWindow = MainWindow); }
            set { _theActiveWindow = value; }
        }

        /// <summary>
        /// Показать диалог.
        /// </summary>
        /// <param name="dialogViewModel">ViewModel диалога.</param>		
        /// <typeparam name="T">Тип View диалога.</typeparam>
        /// <returns>
        /// Возвращаемое значение такое же как у System.Windows.Window.ShowDialog().
        /// </returns>
        public bool? ShowDialog<T>(object dialogViewModel) where T : Window, new()
        {
            return ShowDialogInternal<T>(dialogViewModel, ActiveWindow);
        }

        /// <summary>
        /// Показать диалог.
        /// </summary>
        /// <param name="dialogViewModel">ViewModel диалога.</param>
        /// <param name="owner">Владелец создаваемого окна</param>
        /// <typeparam name="T">Тип View диалога.</typeparam>
        /// <returns>
        /// Возвращаемое значение такое же как у System.Windows.Window.ShowDialog().
        /// </returns>
        public bool? ShowDialog<T>(object dialogViewModel, IWindow owner) where T : Window, new()
        {
            ThrowArgumentExceptionInNotWindow(owner);

            return ShowDialogInternal<T>(dialogViewModel, (Window) owner);
        }

        /// <summary>
        /// Показать окно в немодальном режиме, owner у окна указывается текущее активное окно
        /// </summary>
        /// <param name="viewModel">ViewModel окна.</param>
        /// <returns>Интерфейс окна для управления закрытием</returns>
        public IWindow Show<T>(object viewModel)
            where T : Window, IWindow, new()
        {
            return ShowInternal<T>(viewModel, ActiveWindow);
        }
		
        /// <summary>
        /// Показать окно в немодальном режиме
        /// </summary>
        /// <param name="viewModel">ViewModel окна.</param>
        /// <param name="owner">Владелец создаваемого окна</param>
        /// <returns>Интерфейс окна для управления закрытием</returns>
        public IWindow Show<T>(object viewModel, IWindow owner)
            where T : Window, IWindow, new()
        {
            ThrowArgumentExceptionInNotWindow(owner);

            return ShowInternal<T>(viewModel, (Window) owner);
        }

        /// <summary>
        /// Показать MessageBox.
        /// </summary>
        /// <param name="message">Текст для отображения.</param>
        /// <param name="caption">Заголовок. если пустой то берётся заголовок активного окна</param>
        /// <param name="button">
        /// Параметр, определяющий какие кнопки должен содержать MessageBox.
        /// </param>
        /// <param name="icon">Иконка для отображения.</param>
        /// <returns>
        /// MessageBoxResult определяет какую кнопку нажал пользователь.
        /// </returns>
        public MessageBoxResult ShowMessageDialog(
            string message, 
            string caption,
            MessageBoxButton button = MessageBoxButton.OK, 
            MessageBoxImage icon = MessageBoxImage.Information)
        {
            return ShowMessageDialogInternal(ActiveWindow, message, caption, button, icon, 
                MessageBoxResult.OK, MessageBoxOptions.None);
        }

        public MessageBoxResult ShowMessageDialog(string message, MessageBoxButton button, MessageBoxImage icon)
        {
            return ShowMessageDialogInternal(ActiveWindow, message, MainWindow.Title, button, icon,
                MessageBoxResult.OK, MessageBoxOptions.None);
        }

        /// <summary>
        /// Показать диалог с сообщением об ошибке.
        /// </summary>
        public void ShowErrorMessageDialog(string message, string details, string caption = null)
        {
            ShowErrorMessageDialogInternal(ActiveWindow, message, details, caption);
        }

        /// <summary>
        /// Показать диалог открытия файла
        /// </summary>
        /// <param name="filter">Фильтр для фильтрации файлов по типам</param>
        /// <param name="fileName">Имя файла</param>
        /// <param name="owner">Владелец создаваемого окна</param>
        /// <returns></returns>
        public virtual bool? ShowOpenFileDialog(string filter, out string fileName, IWindow owner = null)
        {
            ThrowArgumentExceptionInNotWindow(owner);
			
            fileName = null;
            var ownerWindow = (Window)owner ?? ActiveWindow;

            var dlg = new OpenFileDialog
            {
                Filter = filter
            };
            var res = dlg.ShowDialog(ownerWindow);
            if (true == res)
            {
                fileName = dlg.FileName;
            }
            return res;
        }

        /// <summary>
        /// Показать диалог сохранения файла
        /// </summary>
        /// <param name="filter">Фильтр для фильтрации файлов по типам</param>
        /// <param name="defFileName">Первоначальное имя файла</param>
        /// <param name="fileName">Имя файла после закрытия диалога</param>
        /// <param name="owner">Владелец создаваемого окна</param>
        /// <returns></returns>
        public virtual bool? ShowSaveFileDialog(string filter, string defFileName, out string fileName, IWindow owner = null) 
        {
            ThrowArgumentExceptionInNotWindow(owner);

            fileName = null;
            var ownerWindow = (Window)owner ?? ActiveWindow;

            var dlg = new SaveFileDialog
            {
                Filter = filter, 
                FileName = defFileName
            };

            var res = dlg.ShowDialog(ownerWindow);
            if (true == res)
            {
                fileName = dlg.FileName;
            }
            return res;
        }

        public string ShowInputBox(string title, string parameterName, string inputString = null)
        {
            var inputBoxViewModel = new InputBoxViewModel
            {
                Title = title,
                InputString = inputString,
                ParameterName = parameterName
            };

            if (ShowDialog<InputBoxWindow>(inputBoxViewModel) == false)
                return null;

            return inputBoxViewModel.InputString;
        }

        protected virtual bool? ShowDialogInternal<T>(object dialogViewModel, Window owner) where T : Window, new()
        {
            if (!_dispatcher.CheckAccess())
            {
                return (bool?)_dispatcher.Invoke(new Func<bool?>(() => ShowDialogInternal<T>(dialogViewModel, owner)));
            }

            bool? res;

            var prevActiveWindow = ActiveWindow;

            try
            {
                var dialog = new T 
                {
                    Owner = owner != null && owner.IsLoaded ? owner : null,
                    DataContext = dialogViewModel
                };

                ActiveWindow = dialog;
                res = dialog.ShowDialog();
            }
            finally
            {
                ActiveWindow = prevActiveWindow;
            }
            return res;
        }

        protected virtual IWindow ShowInternal<T>(object viewModel, Window owner)
            where T : Window, IWindow, new()
        {
            if (!_dispatcher.CheckAccess())
            {
                return (IWindow)_dispatcher.Invoke(new Func<IWindow>(() => ShowInternal<T>(viewModel, owner)));
            }

            var window = new T
            {
                Owner = owner != null && owner.IsLoaded ? owner : null,
                DataContext = viewModel
            };
            window.Show();

            return window;
        }

        protected virtual void ShowErrorMessageDialogInternal(Window owner, string message, string details, string caption)
        {
            if (!_dispatcher.CheckAccess())
            {
                _dispatcher.Invoke(() => ShowErrorMessageDialogInternal(owner, message, details, caption));
                return;
            }

            var messageCaption = caption;
            try
            {
                messageCaption = GetMessageDialogCaption(owner, caption);
                ShowDialogInternal<ErrorMessageWindow>(new ErrorMessageViewModel(messageCaption, message, details), owner);
            }
            catch (Exception ex)
            {
                Console.WriteLine(@"Ошибка отображения окна об ошибке: " + ex);
                MessageBox.Show(message + Environment.NewLine + Environment.NewLine + details, messageCaption, 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Реализация отображения Message диалога
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="messageBoxText"></param>
        /// <param name="caption">может быть пустым</param>
        /// <param name="button"></param>
        /// <param name="icon"></param>
        /// <param name="options"></param>
        /// <param name="defaultButton"></param>
        /// <returns>MessageBoxResult</returns>
        protected virtual MessageBoxResult ShowMessageDialogInternal(
            Window owner, 
            string messageBoxText, 
            string caption,
            MessageBoxButton button, 
            MessageBoxImage icon, 
            MessageBoxResult defaultButton,
            MessageBoxOptions options)
        {		
            if (icon == MessageBoxImage.Error && button == MessageBoxButton.OK)
            {
                ShowErrorMessageDialogInternal(owner, messageBoxText, null, caption);
                return MessageBoxResult.OK;
            }

            if (!_dispatcher.CheckAccess())
            {
                return (MessageBoxResult)_dispatcher.Invoke(
                    new Func<MessageBoxResult>(() => ShowMessageDialogInternal(owner, messageBoxText, caption, button, icon, defaultButton, options)));
            }

            var messageBoxCaption = GetMessageDialogCaption(owner, caption);

            var messageBoxWindow = new MessageBoxWindow();
            messageBoxWindow.Owner = owner;
            var messageBoxViewModel = new MessageBoxViewModel(messageBoxCaption, messageBoxText, button, icon);
            messageBoxWindow.DataContext = messageBoxViewModel;
            messageBoxWindow.ShowDialog();

            return messageBoxViewModel.Result;
        }

        /// <summary>
        /// Реализация получения Caption для Message диалога.
        /// Если передали пустой, то возвращает Title текущего активного окна
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="caption"></param>
        /// <returns></returns>
        protected virtual string GetMessageDialogCaption(Window owner, string caption)
        {
            if (!string.IsNullOrWhiteSpace(caption))
                return caption;

            if (owner != null)
                return owner.Title;

            try
            {
                return ActiveWindow == null || string.IsNullOrWhiteSpace(ActiveWindow.Title)
                    ? DefaultWindowTitle
                    : ActiveWindow.Title;
            }
            catch
            {
                return DefaultWindowTitle;
            }
        }

        private static void ThrowArgumentExceptionInNotWindow(IWindow owner)
        {
            if (owner != null && !(owner is Window))
                throw new ArgumentException("owner должен быть наследником класса Window", "owner");
        }
    }
}