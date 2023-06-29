using L_Commander.App.Infrastructure.Settings;

namespace L_Commander.App.Infrastructure;

public interface ISettingsFiller
{
    void FillSettings(ClientSettings settings);
}