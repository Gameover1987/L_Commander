using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using L_Commander.UI.Infrastructure;
using Microsoft.Extensions.Logging;

namespace L_Commander.App.Infrastructure
{
    public interface IExceptionHandler
    {
        void HandleExceptionWithMessageBox(Exception exception);

        void HandleException(Exception exception);
    }

    public class ExceptionHandler : IExceptionHandler
    {
        private readonly IWindowManager _windowManager;
        private readonly ILogger<ExceptionHandler> _logger;

        public ExceptionHandler(IWindowManager windowManager, ILogger<ExceptionHandler> logger)
        {
            _windowManager = windowManager;
            _logger = logger;
        }

        public void HandleExceptionWithMessageBox(Exception exception)
        {
            _logger.LogError(new EventId(0, "ExceptionHandler"), exception, exception.StackTrace);
            _windowManager.ShowMessage(Resources.MainWindowTitle, exception.Message);
        }

        public void HandleException(Exception exception)
        {
            _logger.LogError(new EventId(0, "ExceptionHandler"), exception, exception.StackTrace);
        }
    }
}
