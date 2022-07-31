using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using L_Commander.App.Infrastructure;
using L_Commander.App.Infrastructure.History;
using L_Commander.App.ViewModels.History;
using L_Commander.Common.Extensions;

namespace L_Commander.App.Views.DesignMock
{
    internal sealed class DesignMockHistoryManager : IHistoryManager
    {
        private const string LoremIpsum =
            "Lorem ipsum — классический текст-«рыба» (условный, зачастую бессмысленный текст-заполнитель, вставляемый в макет страницы). Является искажённым отрывком из философского трактата Марка Туллия Цицерона «О пределах добра и зла[en]», написанного в 45 году до н. э. на латинском языке, обнаружение сходства приписывается Ричарду МакКлинтоку[1]. Распространился в 1970-х годах из-за трафаретов компании Letraset, a затем — из-за того, что служил примером текста в программе PageMaker. Испорченный текст, вероятно, происходит от его издания в Loeb Classical Library 1914 года, в котором слово dolorem разбито переносом так, что страница 36 начинается с lorem ipsum… (do- осталось на предыдущей)[2].";
        

        public void Add(string name, string description)
        {
            throw new NotImplementedException();
        }

        public HistoryItem[] GetHistory()
        {
            var now = DateTime.Now;
            var yesterDay = DateTime.Now.AddDays(-1);
            return new HistoryItem[]
            {
                new HistoryItem
                {
                    Name = "Copy operation",
                    Description = LoremIpsum,
                    DateTime = now
                },
                new HistoryItem
                {
                    Name = "Move operation",
                    Description = LoremIpsum,
                    DateTime = now
                },
                new HistoryItem
                {
                    Name = "Copy operation",
                    Description = LoremIpsum,
                    DateTime = now
                },
                new HistoryItem
                {
                    Name = "Delete operation",
                    Description = LoremIpsum,
                    DateTime = now
                },
                new HistoryItem
                {
                    Name = "Move operation",
                    Description = LoremIpsum,
                    DateTime = yesterDay
                },
                new HistoryItem
                {
                    Name = "Copy operation",
                    Description = "From here to eternity ))",
                    DateTime = yesterDay
                },
                new HistoryItem
                {
                    Name = "Delete operation",
                    Description = LoremIpsum,
                    DateTime = yesterDay
                },
                new HistoryItem
                {
                    Name = "Copy operation",
                    Description = "From here to eternity ))",
                    DateTime = yesterDay.AddDays(-1)
                },
                new HistoryItem
                {
                    Name = "Delete operation",
                    Description = LoremIpsum,
                    DateTime = yesterDay.AddDays(-1)
                },
                new HistoryItem
                {
                    Name = "Copy operation",
                    Description = "From here to eternity ))",
                    DateTime = yesterDay.AddDays(-2)
                },
                new HistoryItem
                {
                    Name = "Delete operation",
                    Description = LoremIpsum,
                    DateTime = yesterDay.AddDays(-3)
                },
            };
        }

        public void DeleteFromHistory(int id)
        {
            throw new NotImplementedException();
        }
    }

    public class DesignMockHistoryViewModel : HistoryViewModel
    {
        public DesignMockHistoryViewModel()
            : base(new DesignMockHistoryManager(), new DesignMockExceptionHandler(), new DesignMockWindowManager())
        {
            ThreadTaskExtensions.IsSyncRun = true;
            Initialize();
        }
    }
}
