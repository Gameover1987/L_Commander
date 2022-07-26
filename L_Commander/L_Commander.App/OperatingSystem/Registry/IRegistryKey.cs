using System;
using System.Collections.Generic;

namespace L_Commander.App.OperatingSystem.Registry;

public interface IRegistryKey : IDisposable
{
    IRegistryKey OpenSubKey(string subKey);

    object GetValue(string subKey);

    IEnumerable<string> GetValueNames();

    IEnumerable<string> GetSubKeyNames();
}