using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using L_Commander.App.OperatingSystem;
using L_Commander.Common.Extensions;
using L_Commander.UI.Commands;
using L_Commander.UI.ViewModels;

namespace L_Commander.App.ViewModels
{
    public interface IOpenWithViewModel
    {
        ApplicationModel SelectedApp { get; }

        void Initialize(string filePath);

        bool IsCancelled { get; }
    }

    public class OpenWithViewModel : ViewModelBase, IOpenWithViewModel
    {
        private readonly IFileSystemProvider _fileSystemProvider;
        private readonly IApplicationsProvider _applicationsProvider;

        private readonly ICollectionView _appsView;
        private ApplicationModel _selectedApp;

        public OpenWithViewModel(IFileSystemProvider fileSystemProvider, IApplicationsProvider applicationsProvider)
        {
            _fileSystemProvider = fileSystemProvider;
            _applicationsProvider = applicationsProvider;

            _appsView = CollectionViewSource.GetDefaultView(Apps);
            _appsView.GroupDescriptions.Add(new PropertyGroupDescription(nameof(ApplicationModel.Group)));
            _appsView.SortDescriptions.Add(new SortDescription(nameof(ApplicationModel.Order), ListSortDirection.Ascending));
            _appsView.SortDescriptions.Add(new SortDescription(nameof(ApplicationModel.DisplayName), ListSortDirection.Ascending));

            SelectCommand = new DelegateCommand(SelectCommandHandler, CanSelectCommandHandler);
            CancelCommand = new DelegateCommand(CancelCommandHandler);
        }

        public void Initialize(string filePath)
        {
            Title = $"Open '{filePath}' with ...";
            IsCancelled = false;
            SelectedApp = null;

            var fileExtension = _fileSystemProvider.GetFileExtension(filePath);
            var associatedApps = _applicationsProvider.GetAssociatedApplications(fileExtension);
            associatedApps.ForEach(x =>
            {
                x.Group = "Associated apps";
                x.Order = 0;
            });
            var installedApps = _applicationsProvider.GetInstalledApplications();
            installedApps.ForEach(x =>
            {
                x.Group = "Installed apps";
                x.Order = 1;
            });

            Apps.Clear();
            Apps.AddRange(associatedApps.Concat(installedApps));
        }

        public string Title { get; private set; }

        public bool IsCancelled { get; private set; }

        public ObservableCollection<ApplicationModel> Apps { get; } = new ObservableCollection<ApplicationModel>();

        public ApplicationModel SelectedApp
        {
            get { return _selectedApp; }
            set
            {
                if (_selectedApp == value)
                    return;
                _selectedApp = value;
                OnPropertyChanged(() => SelectedApp);
            }
        }

        public IDelegateCommand SelectCommand { get; }

        public IDelegateCommand CancelCommand { get; }

        public bool CanSelectCommandHandler()
        {
            return SelectedApp != null;
        }

        private void SelectCommandHandler()
        {

        }

        private void CancelCommandHandler()
        {
            IsCancelled = true;
        }
    }
}
