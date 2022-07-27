using L_Commander.App.OperatingSystem;
using L_Commander.UI.Commands;
using L_Commander.UI.ViewModels;

namespace L_Commander.App.ViewModels;

public class FileSystemPathPartViewModel : ViewModelBase
{
    public FileSystemPathPartViewModel(FileSystemEntryDescriptor descriptor, IDelegateCommand command)
    {
        Name = descriptor.Name;
        Path = descriptor.Path;

        Command = command;
    }

    public string Name { get; set; }

    public string Path { get; set; }

    public IDelegateCommand Command { get; }
}