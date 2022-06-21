using System.Windows;
using L_Commander.UI.Commands;

namespace L_Commander.UI.ViewModels
{
    public class MessageBoxViewModel : ViewModelBase
    {
        public MessageBoxViewModel(string title, string message, MessageBoxButton messageBoxButton, MessageBoxImage messageBoxImage)
        {
            Title = title;
            Message = message;
            MessageBoxButton = messageBoxButton;
            MessageBoxImage = messageBoxImage;

            switch (messageBoxButton)
            {
                case MessageBoxButton.YesNo:
                    Result = MessageBoxResult.No;
                    break;
                case MessageBoxButton.OK:
                    Result = MessageBoxResult.OK;
                    break;
                case MessageBoxButton.OKCancel:
                    Result = MessageBoxResult.Cancel;
                    break;
                case MessageBoxButton.YesNoCancel:
                    Result = MessageBoxResult.Cancel;
                    break;
            }

            YesCommand = new DelegateCommand(YesCommandHandler);
            NoCommand = new DelegateCommand(NoCommandHandler);
            OkCommand = new DelegateCommand(OkCommandHandler);
            CancelCommand = new DelegateCommand(CancelCommandHandler);
        }

        public string Title { get; }

        public string Message { get; }

        public MessageBoxButton MessageBoxButton { get; }

        public MessageBoxImage MessageBoxImage { get; }

        public MessageBoxResult Result { get; set; } = MessageBoxResult.None;

        public IDelegateCommand YesCommand { get; }

        public IDelegateCommand NoCommand { get; }

        public IDelegateCommand OkCommand { get; }

        public IDelegateCommand CancelCommand { get; }

        private void YesCommandHandler()
        {
            Result = MessageBoxResult.Yes;
        }

        private void NoCommandHandler()
        {
            Result = MessageBoxResult.No;
        }

        private void OkCommandHandler()
        {
            Result = MessageBoxResult.OK;
        }

        private void CancelCommandHandler()
        {
            Result = MessageBoxResult.Cancel;
        }
    }
}
