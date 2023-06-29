using System;
using System.Linq;
using L_Commander.Database;

namespace L_Commander.App.Infrastructure.History;

public class HistoryManager : IHistoryManager
{
    private readonly LCommanderDbContext _dbContext;

    public HistoryManager(LCommanderDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(string name, string description)
    {
        _dbContext.History.Add(new HistoryItemEntity
        {
            Name = name,
            DateTime = DateTime.Now,
            Description = description
        });
        _dbContext.SaveChanges();
    }

    public HistoryItem[] GetHistory()
    {
        return _dbContext.History.Select(x => new HistoryItem
        {
            Id = x.Id,
            Name = x.Name,
            DateTime = x.DateTime,
            Description = x.Description
        }).ToArray();
    }

    public void DeleteFromHistory(int id)
    {
        var historyItem = _dbContext.History.FirstOrDefault(x => x.Id == id);
        if (historyItem == null)
            return;

        _dbContext.History.Remove(historyItem);
        _dbContext.SaveChanges();
    }
}