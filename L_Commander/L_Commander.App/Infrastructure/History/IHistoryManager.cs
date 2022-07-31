using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L_Commander.App.Infrastructure.History
{
    public interface IHistoryManager
    {
        public void Add(string name, string description);

        public HistoryItem[] GetHistory();

        void DeleteFromHistory(int id);
    }
}
