using System;
using L_Commander.App.Infrastructure;
using L_Commander.App.Views;

namespace L_Commander.App.ViewModels;

public interface IMainViewModel
{
    void Initialize(IWindow window);

    void SaveSettings();

    MainWindowSettings GetMainWindowSettings();
}