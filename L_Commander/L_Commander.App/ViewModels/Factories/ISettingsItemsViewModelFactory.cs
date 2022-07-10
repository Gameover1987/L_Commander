using L_Commander.App.Infrastructure;
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

    public SettingsItemsViewModelFactory(IWindowManager windowManager, IAddTagViewModel addTagViewModel)
    {
        _windowManager = windowManager;
        _addTagViewModel = addTagViewModel;
    }

    public ISettingsItemViewModel[] CreateSettingsItems(ClientSettings clientSettings)
    {
        var tagSettingsItem = new TagSettingsItemViewModel(clientSettings.TagSettings, _addTagViewModel, _windowManager);

        return new ISettingsItemViewModel[] { tagSettingsItem };
    }
}