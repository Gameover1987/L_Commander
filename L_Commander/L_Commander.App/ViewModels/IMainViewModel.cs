using System;
using L_Commander.App.Infrastructure;
using L_Commander.App.Views;
using L_Commander.UI.Commands;

namespace L_Commander.App.ViewModels;

public interface IMainViewModel : ISettingsFiller
{
    IFileManagerViewModel ActiveFileManager { get; set; }

    IFileManagerViewModel LeftFileManager { get; }

    IFileManagerViewModel RightFileManager { get; }

    IDelegateCommand DeleteCommand { get; }

    IDelegateCommand CopyCommand { get; }

    IDelegateCommand MoveCommand { get; }

    void Initialize();
}