using System;
using System.Runtime.Versioning;
using Microsoft.Win32;

namespace L_Commander.App.OperatingSystem.Registry;

public class RegistryProvider : IRegistryProvider
{
    public IRegistryKey GetRegistryKey(RootRegistryKey rootRegistryKey)
    {
        var winRegistryKey = rootRegistryKey switch
        {
            RootRegistryKey.CurrentUser => Microsoft.Win32.Registry.CurrentUser,
            RootRegistryKey.ClassesRoot => Microsoft.Win32.Registry.ClassesRoot,
            RootRegistryKey.LocalMachine => Microsoft.Win32.Registry.LocalMachine,
            _ => throw new ArgumentOutOfRangeException(nameof(rootRegistryKey), rootRegistryKey, null)
        };

        return new RegistryKey(winRegistryKey);
    }
}