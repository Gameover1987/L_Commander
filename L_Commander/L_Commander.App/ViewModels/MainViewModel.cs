using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L_Commander.App.ViewModels
{
    public class MainViewModel : IMainViewModel
    {
        private readonly IFileManagerViewModel _leftFileManagerViewModel;
        private readonly IFileManagerViewModel _rightFileManagerViewModel;

        public MainViewModel(IFileManagerViewModel leftFileManagerViewModel, IFileManagerViewModel rightFileManagerViewModel)
        {
            _leftFileManagerViewModel = leftFileManagerViewModel;
            _rightFileManagerViewModel = rightFileManagerViewModel;
        }

        public IFileManagerViewModel LeftFileManagerViewModel => _leftFileManagerViewModel;

        public IFileManagerViewModel RightFileManagerViewModel => _rightFileManagerViewModel;

        public void Initialize()
        {
            _leftFileManagerViewModel.Initialize();
            _rightFileManagerViewModel.Initialize();
        }
    }
}
