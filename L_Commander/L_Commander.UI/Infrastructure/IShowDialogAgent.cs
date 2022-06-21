using System.Windows;

namespace L_Commander.UI.Infrastructure
{
    /// <summary>
    /// Интерфейс агента, поднимающего диалоги.
    /// </summary>
    public interface IShowDialogAgent
	{
        /// <summary>
		/// Показать диалог.
		/// </summary>
		/// <param name="dialogViewModel">ViewModel диалога.</param>		
		/// <typeparam name="T">Тип View диалога.</typeparam>
		/// <returns>
		/// Возвращаемое значение такое же как у System.Windows.Window.ShowDialog().
		/// </returns>
		bool? ShowDialog<T>(object dialogViewModel) where T : Window, new();

        /// <summary>
		/// Показать окно в немодальном режиме, owner у окна указывается текущее активное окно
		/// </summary>
		/// <param name="viewModel">ViewModel окна.</param>
		/// <returns>Интерфейс окна для управления закрытием</returns>
		IWindow Show<T>(object viewModel) where T : Window, IWindow, new();

		/// <summary>
		/// Показать окно в немодальном режиме
		/// </summary>
		/// <typeparam name="T">Тип View для окна.</typeparam>
		/// <param name="viewModel">ViewModel окна.</param>
		/// <param name="owner">Владелец создаваемого окна</param>
		/// <returns>Интерфейс окна для управления закрытием</returns>
		IWindow Show<T>(object viewModel, IWindow owner) where T : Window, IWindow, new();

        /// <summary>
		/// Показать диалог с сообщением об ошибке для активного окна.
		/// </summary>
		void ShowErrorMessageDialog(string message, string details, string caption = null);

        /// <summary>
		/// Показать диалог открытия файла
		/// </summary>
		/// <param name="filter"></param>
		/// <param name="fileName"></param>
		/// <param name="owner">Владелец создаваемого окна</param>
		/// <returns></returns>
		bool? ShowOpenFileDialog(string filter, out string fileName, IWindow owner = null);

		/// <summary>
		/// Показать диалог сохранения файла
		/// </summary>
		/// <param name="filter"></param>
		/// <param name="defFileName"></param>
		/// <param name="fileName"></param>
		/// <param name="owner">Владелец создаваемого окна</param>
		/// <returns></returns>
		bool? ShowSaveFileDialog(string filter, string defFileName, out string fileName, IWindow owner = null);

		/// <summary>
		/// Простое окно для ввода строки
		/// </summary>
		/// <param name="title">Заголовок окна</param>
		/// <param name="parameterName">Название параметра</param>
		/// <param name="inputString">Начальное значение параметра</param>
		/// <returns></returns>
        string ShowInputBox(string title, string parameterName, string inputString = null);

        /// <summary>
        /// Показать MessageBox. Позволяет задать заголовок
        /// </summary>
        MessageBoxResult ShowMessageDialog(string message, string caption, MessageBoxButton button, MessageBoxImage icon);

        /// <summary>
        /// Показать MessageBox.
        /// </summary>
        MessageBoxResult ShowMessageDialog(string message, MessageBoxButton button, MessageBoxImage icon);
	}
}
