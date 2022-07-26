using Microsoft.Win32;

namespace L_Commander.App.OperatingSystem.Registry;

public interface IRegistryProvider
{
    IRegistryKey GetRegistryKey(RootRegistryKey rootRegistryKey);
}