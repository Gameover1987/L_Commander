using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows.Data;
using L_Commander.App.Infrastructure;
using L_Commander.App.Infrastructure.History;
using L_Commander.Common.Extensions;
using L_Commander.UI.Commands;
using L_Commander.UI.ViewModels;
using MahApps.Metro.Controls.Dialogs;

namespace L_Commander.App.ViewModels.History
{
    public class HistoryViewModel : ViewModelBase, IHistoryViewModel
    {
        private readonly IHistoryManager _historyManager;
        private readonly IExceptionHandler _exceptionHandler;
        private readonly IWindowManager _windowManager;
        private bool _isBusy;

        private readonly ICollectionView _historyView;
        private HistoryItem _selectedHistoryItem;
        private string _searchString;

        public HistoryViewModel(IHistoryManager historyManager, IExceptionHandler exceptionHandler, IWindowManager windowManager)
        {
            _historyManager = historyManager;
            _exceptionHandler = exceptionHandler;
            _windowManager = windowManager;

            _historyView = CollectionViewSource.GetDefaultView(History);
            _historyView.GroupDescriptions.Add(new PropertyGroupDescription(nameof(HistoryItem.DateTime.Date)));
            _historyView.SortDescriptions.Add(new SortDescription(nameof(HistoryItem.DateTime), ListSortDirection.Descending));
            _historyView.Filter = Filter;

            DeleteFromHistoryCommand = new DelegateCommand(DeleteFromHistoryCommandHandler, CanDeleteFromHistoryCommandHandler);
        }

        public string SearchString
        {
            get => _searchString;
            set
            {
                if (_searchString == value)
                    return;
                _searchString = value;
                OnPropertyChanged(() => SearchString);

                _historyView.Refresh();
            }
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

        public HistoryItem SelectedHistoryItem
        {
            get { return _selectedHistoryItem; }
            set
            {
                if (_selectedHistoryItem == value)
                    return;
                _selectedHistoryItem = value;
                OnPropertyChanged(() => SelectedHistoryItem);
            }
        }

        public IDelegateCommand DeleteFromHistoryCommand { get; }

        public async void Initialize()
        {
            IsBusy = true;

            try
            {
                History.Clear();
                
                var items = await ThreadTaskExtensions.Run(() =>
                {
                    Thread.Sleep(2000);
                    return _historyManager.GetHistory();
                });

                foreach (var historyItem in items)
                {
                    History.Add(historyItem);
                }

                SelectedHistoryItem = History.FirstOrDefault();

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

        public HistoryItem[] SelectedHistoryItems { get; set; }

        private bool CanDeleteFromHistoryCommandHandler()
        {
            return SelectedHistoryItem != null;
        }

        private async void DeleteFromHistoryCommandHandler()
        {
            var questionSettings = new MetroDialogSettings { DefaultButtonFocus = MessageDialogResult.Affirmative };
            var result = await _windowManager.ShowQuestion("Delete from history",
                $"Do you want delete selected history data?", 
                questionSettings);
            if (result != MessageDialogResult.Affirmative)
                return;

            foreach (var selectedHistoryItem in SelectedHistoryItems)
            {
                _historyManager.DeleteFromHistory(selectedHistoryItem.Id);
                History.Remove(selectedHistoryItem);
            }
        }

        private bool Filter(object obj)
        {
            if (SearchString.IsNullOrWhiteSpace())
                return true;

            var historyItem = (HistoryItem)obj;
            if (historyItem.Name.ToLower().Contains(SearchString.ToLower()))
                return true;

            return historyItem.Description.ToLower().Contains(SearchString.ToLower());
        }
    }
}
