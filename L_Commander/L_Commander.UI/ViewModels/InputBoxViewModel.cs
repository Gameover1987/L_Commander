using L_Commander.UI.Commands;

namespace L_Commander.UI.ViewModels
{
    public class InputBoxViewModel : ViewModelBase
    {
        private string _inputString;

        public InputBoxViewModel()
        {
            OKCommand = new DelegateCommand(OkCommandHandler, CanOkCommandHandler);
        }

        public string Title { get; set; }

        public string ParameterName { get; set; }

        public string InputString
        {
            get { return _inputString; }
            set
            {
                if (_inputString == value)
                    return;
                _inputString = value;
                OnPropertyChanged(() => InputString);
            }
        }

        public IDelegateCommand OKCommand { get; }

        private void OkCommandHandler()
        {

        }

        private bool CanOkCommandHandler()
        {
            return !string.IsNullOrEmpty(InputString);
        }
    }
}
