using System.Windows;
using L_Commander.UI.Infrastructure;

namespace L_Commander.UI.ViewModels.DesignTime
{
	public class DesignMockShowDialogAgent : IShowDialogAgent
	{
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
			return null;
		}

        /// <summary>
		/// Показать окно в немодальном режиме, owner у окна указывается текущее активное окно
		/// </summary>
		/// <param name="viewModel">ViewModel окна.</param>
		/// <returns>Интерфейс окна для управления закрытием</returns>
		public IWindow Show<T>(object viewModel) where T : Window, IWindow, new()
		{
			return null;
		}

		/// <summary>
		/// Показать окно в немодальном режиме
		/// </summary>
		/// <typeparam name="T">Тип View для окна.</typeparam>
		/// <param name="viewModel">ViewModel окна.</param>
		/// <param name="owner">Владелец создаваемого окна</param>
		/// <returns>Интерфейс окна для управления закрытием</returns>
		public IWindow Show<T>(object viewModel, IWindow owner) where T : Window, IWindow, new()
		{
			return null;
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
		public MessageBoxResult ShowMessageDialog(string message, string caption, MessageBoxButton button = MessageBoxButton.OK,
			MessageBoxImage icon = MessageBoxImage.Asterisk)
		{
			return MessageBoxResult.None;
		}

        public MessageBoxResult ShowMessageDialog(string message, MessageBoxButton button, MessageBoxImage icon)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
		/// Показать диалог с сообщением об ошибке для активного окна.
		/// </summary>
		public void ShowErrorMessageDialog(string message, string details, string caption = null)
		{
		}

        /// <summary>
		/// Показать диалог открытия файла
		/// </summary>
		/// <param name="filter"></param>
		/// <param name="fileName"></param>
		/// <param name="owner">Владелец создаваемого окна</param>
		/// <returns></returns>
		public bool? ShowOpenFileDialog(string filter, out string fileName, IWindow owner = null)
		{
			fileName = null;
			return null;
		}

		/// <summary>
		/// Показать диалог сохранения файла
		/// </summary>
		/// <param name="filter"></param>
		/// <param name="defFileName"></param>
		/// <param name="fileName"></param>
		/// <param name="owner">Владелец создаваемого окна</param>
		/// <returns></returns>
		public bool? ShowSaveFileDialog(string filter, string defFileName, out string fileName, IWindow owner = null)
		{
			fileName = null;
			return null;
		}

        public string ShowInputBox(string title, string parameterName, string inputString)
        {
            throw new System.NotImplementedException();
        }
    }
}