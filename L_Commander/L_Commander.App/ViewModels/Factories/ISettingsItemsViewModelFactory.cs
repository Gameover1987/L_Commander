using L_Commander.App.Infrastructure;
using L_Commander.App.Infrastructure.Settings;
using L_Commander.App.ViewModels.Settings;

namespace L_Commander.App.ViewModels.Factories;

public interface ISettingsItemsViewModelFactory
{
    ISettingsItemViewModel[] CreateSettingsItems(ClientSettings clientSettings);
}

public class SettingsItemsViewModelFactory : ISettingsItemsViewModelFactory
{
    private readonly IWindowManager _windowManager;
    private readonly IAddTagViewModel _addTagViewModel;
    private readonly ITagRepository _tagRepository;

    public SettingsItemsViewModelFactory(IWindowManager windowManager, IAddTagViewModel addTagViewModel, ITagRepository tagRepository)
    {
        _windowManager = windowManager;
        _addTagViewModel = addTagViewModel;
        _tagRepository = tagRepository;
    }

    public ISettingsItemViewModel[] CreateSettingsItems(ClientSettings clientSettings)
    {
        var tagSettingsItem = new TagSettingsItemViewModel(_tagRepository, _addTagViewModel, _windowManager);
        var fileSystemViewItem = new FilesAndFoldersSettingsItemViewModel(clientSettings.FilesAndFoldersSettings);

        return new ISettingsItemViewModel[] { tagSettingsItem, fileSystemViewItem };
    }
}