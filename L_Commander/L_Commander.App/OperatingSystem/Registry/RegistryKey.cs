using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;

namespace L_Commander.App.OperatingSystem.Registry;

[SupportedOSPlatform("windows")]
public class RegistryKey : IRegistryKey
{
    private readonly Microsoft.Win32.RegistryKey _winRegistryKey;

    public RegistryKey(Microsoft.Win32.RegistryKey winRegistryKey)
    {
        _winRegistryKey = winRegistryKey;
    }

    public IRegistryKey OpenSubKey(string subKey) => new RegistryKey(_winRegistryKey?.OpenSubKey(subKey));

    public object GetValue(string subKey) => _winRegistryKey?.GetValue(subKey);

    public IEnumerable<string> GetValueNames() => _winRegistryKey?.GetValueNames() ?? Enumerable.Empty<string>();

    public IEnumerable<string> GetSubKeyNames() => _winRegistryKey?.GetSubKeyNames() ?? Enumerable.Empty<string>();

    public void Dispose() => _winRegistryKey?.Dispose();
}