using L_Commander.App.Infrastructure.History;

namespace L_Commander.App.ViewModels.History;

public interface IHistoryViewModel
{
    void Initialize();

    HistoryItem[] SelectedHistoryItems { get; set; }
}