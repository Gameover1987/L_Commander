using System;
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

    public bool IsReady => _driveInfo.IsReady;

    public DriveType DriveType => _driveInfo.DriveType;

    public string RootPath => _driveInfo.RootDirectory.FullName;

    public long TotalSize => _driveInfo.TotalSize;

    public long FreeSpace => _driveInfo.TotalFreeSpace;

    public long OccupiedSpace => TotalSize - FreeSpace;

    public bool IsSystem
    {
        get
        {
            var winDriveName = Path.GetPathRoot(Environment.SystemDirectory);
            if (winDriveName == null)
                return false;
            return _driveInfo.Name.ToLower() == winDriveName.ToLower();
        }
    }

    public bool IsFreeSpaceLittle
    {
        get
        {
            var bytesPerOnePercent = TotalSize / 100.0;

            var threshold = bytesPerOnePercent * 10;

            return FreeSpace < threshold;
        }
    }
}