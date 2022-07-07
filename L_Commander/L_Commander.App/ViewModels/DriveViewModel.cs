using System.IO;
using L_Commander.Common.Extensions;
using L_Commander.UI.ViewModels;

namespace L_Commander.App.ViewModels;

public class DriveViewModel : ViewModelBase
{
    private readonly DriveInfo _driveInfo;

    public DriveViewModel(DriveInfo driveInfo)
    {
        _driveInfo = driveInfo;
    }

    public string DisplayName
    {
        get
        {
            if (!_driveInfo.IsReady)
                return _driveInfo.Name;

            if (_driveInfo.VolumeLabel.IsNullOrWhiteSpace())
                return _driveInfo.Name;

            return $"{_driveInfo.Name} - {_driveInfo.VolumeLabel}";
        }
    }

    public bool IsReady
    {
        get { return _driveInfo.IsReady; }
    }

    public string RootPath
    {
        get { return _driveInfo.RootDirectory.FullName; }
    }
}