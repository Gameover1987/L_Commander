using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using L_Commander.App.Infrastructure;
using L_Commander.App.Infrastructure.History;
using L_Commander.Common.Extensions;
using L_Commander.UI.ViewModels;

namespace L_Commander.App.ViewModels.History
{
    public interface IHistoryViewModel
    {
        void Initialize();
    }

    public class HistoryViewModel : ViewModelBase, IHistoryViewModel
    {
        private readonly IHistoryManager _historyManager;
        private readonly IExceptionHandler _exceptionHandler;
        private bool _isBusy;

        private readonly ICollectionView _historyView;

        public HistoryViewModel(IHistoryManager historyManager, IExceptionHandler exceptionHandler)
        {
            _historyManager = historyManager;
            _exceptionHandler = exceptionHandler;

            _historyView = CollectionViewSource.GetDefaultView(History);
            _historyView.GroupDescriptions.Add(new PropertyGroupDescription(nameof(HistoryItem.DateTime.Date)));
        }

        public bool IsBusy
        {
            get { return _isBusy; }
            private set
            {
                if (_isBusy == value)
                    return;
                _isBusy = value;
                OnPropertyChanged(() => IsBusy);
            }
        }

        public ObservableCollection<HistoryItem> History { get; } = new ObservableCollection<HistoryItem>();


        public async void Initialize()
        {
            IsBusy = true;

            try
            {
                History.Clear();
                var items = await ThreadTaskExtensions.Run(() =>
                {
                    return _historyManager.GetHistory();
                });

                foreach (var historyItem in items)
                {
                    History.Add(historyItem);
                }

            }
            catch (Exception exception)
            {
                _exceptionHandler.HandleExceptionWithMessageBox(exception);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
