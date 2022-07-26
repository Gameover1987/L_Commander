using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using L_Commander.App.OperatingSystem.Registry;
using L_Commander.Common;
using L_Commander.Common.Extensions;

namespace L_Commander.App.OperatingSystem;

public enum RootRegistryKey
{
    CurrentUser,
    ClassesRoot,
    LocalMachine
}

public class ApplicationsProvider : IApplicationsProvider
{
    private readonly IRegistryProvider _registryProvider;

    private const string FileExtensionsX64RegistryKeyName = @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Explorer\FileExts";
    private const string FileExtensionsX32RegistryKeyName = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\FileExts";
    private const string AppPathX32RegistryKeyName = @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths";
    private const string AppPathX64RegistryKeyName = @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\App Paths";

    public ApplicationsProvider(IRegistryProvider registryProvider)
    {
        _registryProvider = registryProvider;
    }

    public ApplicationModel[] GetAssociatedApplications(string fileExtension)
    {
        if (string.IsNullOrWhiteSpace(fileExtension))
        {
            return Array.Empty<ApplicationModel>();
        }

        if (!fileExtension.StartsWith("."))
        {
            fileExtension = $".{fileExtension}";
        }

        var associatedApplications = new Dictionary<string, ApplicationModel>();

        TryAddApplications(fileExtension, associatedApplications);

        return associatedApplications.Values.ToArray();
    }

    public ApplicationModel[] GetInstalledApplications()
    {
        var installedApplications = new Dictionary<string, ApplicationModel>();

        var applicationsFiles = GetApplicationsFiles(RootRegistryKey.ClassesRoot, "Applications")
            .Union(GetApplicationsFiles(RootRegistryKey.LocalMachine, AppPathX32RegistryKeyName))
            .Union(GetApplicationsFiles(RootRegistryKey.CurrentUser, AppPathX32RegistryKeyName))
            .ToImmutableHashSet();

        applicationsFiles = applicationsFiles
            .Union(GetApplicationsFiles(RootRegistryKey.LocalMachine, AppPathX64RegistryKeyName))
            .Union(GetApplicationsFiles(RootRegistryKey.CurrentUser, AppPathX64RegistryKeyName))
            .ToImmutableHashSet();

        foreach (var applicationFile in applicationsFiles)
        {
            TryAddApplication(installedApplications, applicationFile);
        }

        return installedApplications.Values.ToArray();
    }

    private void TryAddApplications(string s, IDictionary<string, ApplicationModel> associatedApplications)
    {
        foreach (var applicationFile in GetOpenWithList(s, RootRegistryKey.CurrentUser, FileExtensionsX32RegistryKeyName))
        {
            TryAddApplication(associatedApplications, applicationFile);
        }

        foreach (var applicationFile in GetOpenWithProgids(s, RootRegistryKey.CurrentUser, FileExtensionsX32RegistryKeyName))
        {
            TryAddApplication(associatedApplications, applicationFile);
        }

        if (Environment.Is64BitProcess)
        {
            foreach (var applicationFile in GetOpenWithList(s, RootRegistryKey.CurrentUser,
                         FileExtensionsX64RegistryKeyName))
            {
                TryAddApplication(associatedApplications, applicationFile);
            }

            foreach (var applicationFile in GetOpenWithProgids(s, RootRegistryKey.CurrentUser,
                         FileExtensionsX64RegistryKeyName))
            {
                TryAddApplication(associatedApplications, applicationFile);
            }
        }

        foreach (var applicationFile in GetOpenWithList(s, RootRegistryKey.ClassesRoot))
        {
            TryAddApplication(associatedApplications, applicationFile);
        }

        foreach (var applicationFile in GetOpenWithProgids(s, RootRegistryKey.ClassesRoot))
        {
            TryAddApplication(associatedApplications, applicationFile);
        }
    }

    private IEnumerable<string> GetApplicationsFiles(RootRegistryKey rootKey, string baseKeyName)
    {
        var applications = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

        using var applicationsKeys = _registryProvider.GetRegistryKey(rootKey).OpenSubKey(baseKeyName);
        foreach (var appExecuteFile in applicationsKeys.GetSubKeyNames())
        {
            applications.Add(appExecuteFile);
        }

        return applications.ToImmutableHashSet();
    }

    private IEnumerable<string> GetOpenWithList(string fileExtension, RootRegistryKey rootKey, string baseKeyName = "")
    {
        var results = new List<string>();

        baseKeyName = @$"{baseKeyName?.TrimEnd('\\')}\{fileExtension}\OpenWithList";

        using var baseKey = _registryProvider.GetRegistryKey(rootKey).OpenSubKey(baseKeyName);
        if (baseKey?.GetValue("MRUList") is not string mruList)
        {
            return results;
        }

        foreach (var mru in mruList)
        {
            if (baseKey.GetValue(mru.ToString()) is string name)
            {
                results.Add(name);
            }
        }

        return results;
    }

    private IEnumerable<string> GetOpenWithProgids(string fileExtension, RootRegistryKey rootKey,
        string baseKeyName = "")
    {
        var results = new List<string>();

        baseKeyName = @$"{baseKeyName?.TrimEnd('\\')}\{fileExtension}\OpenWithProgids";

        using var baseKey = _registryProvider.GetRegistryKey(rootKey).OpenSubKey(baseKeyName);
        if (baseKey is not null)
        {
            results.AddRange(baseKey.GetValueNames());
        }

        return results;
    }

    private void TryAddApplication(IDictionary<string, ApplicationModel> applications, string applicationFile)
    {
        var application = FindApplication(applicationFile);
        if (application is not null)
        {
            applications.TryAdd(application.DisplayName, application);
        }
    }

    private ApplicationModel FindApplication(string applicationFile)
    {
        var info = GetInfo(applicationFile);
        if (info == default)
        {
            return null;
        }

        return new ApplicationModel
        {
            DisplayName = info.Name,
            ExecutePath = info.ExecutePath,
            Arguments = ExtractArguments(info.StartCommand, info.ExecutePath)
        };
    }

    private string ExtractArguments(string path, string executePath)
    {
        path = path
            .Replace("\"", "")
            .Replace(executePath, "")
            .TrimStart();

        var argumentsCount = 0;

        return RegexHelper
            .GetMatches(path, "%.", RegexOptions.Compiled)
            .Aggregate(path, (current, match) => current.Replace(match.Value, $"{{{argumentsCount++}}}"));
    }

    private (string Name, string StartCommand, string ExecutePath) GetInfo(string applicationFile)
    {
        var assocFlag = Win32.AssocF.None;
        if (applicationFile.Contains(".exe"))
        {
            assocFlag = Win32.AssocF.OpenByExeName;
        }

        var displayName = Win32.AssocQueryString(assocFlag, Win32.AssocStr.FriendlyAppName, applicationFile);
        var startCommand = Win32.AssocQueryString(assocFlag, Win32.AssocStr.Command, applicationFile);

        if (string.IsNullOrWhiteSpace(displayName) || string.IsNullOrWhiteSpace(startCommand))
        {
            return default;
        }

        var executePath = Win32.AssocQueryString(assocFlag, Win32.AssocStr.Executable, applicationFile);

        return (displayName, startCommand, executePath);
    }
}