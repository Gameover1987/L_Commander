using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using L_Commander.UI.Infrastructure;

namespace L_Commander.App.Infrastructure
{
    public interface IExceptionHandler
    {
        void HandleExceptionWithMessageBox(Exception exception);

        void HandleException(Exception exception);
    }
}
