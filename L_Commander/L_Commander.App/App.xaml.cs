using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using L_Commander.App.Infrastructure;
using L_Commander.App.OperatingSystem;
using L_Commander.App.ViewModels;
using L_Commander.App.ViewModels.Filtering;
using L_Commander.App.Views;
using L_Commander.UI.Commands;
using L_Commander.UI.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace L_Commander.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ServiceProvider _serviceProvider;
        private IConfiguration _configuration;

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            _configuration = CreateConfiguration();
            _serviceProvider = RegisterDependencies(_configuration);

            MainWindow = new MainWindow();
            MainWindow.Closed += MainWindowOnClosed;
            DelegateCommand.CommandException += CommandException;
            MainWindow.DataContext = _serviceProvider.GetService<IMainViewModel>();
            MainWindow.Show();
        }

        private void CommandException(object? sender, ExceptionEventArgs e)
        {
            var exceptionHandler = _serviceProvider.GetService<IExceptionHandler>();
            exceptionHandler?.HandleCommandException(e.Exception);
        }

        private void MainWindowOnClosed(object sender, EventArgs e)
        {
            _serviceProvider.Dispose();

            App.Current.Shutdown(0);
        }

        private IConfiguration CreateConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("AppSettings.json", optional: false, reloadOnChange: true);

            return builder.Build();
        }

        private ServiceProvider RegisterDependencies(IConfiguration configuration)
        {
            IServiceCollection serviceCollection = new ServiceCollection();

            var dispatcherAdapter = new DispatcherAdapter(Current.Dispatcher);

            var serilogLogger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            serviceCollection
                .AddSingleton(_configuration)

                .AddSingleton<IConfiguration>(_configuration)

                // Infrastructure
                .AddSingleton<IDispatcher>(dispatcherAdapter)
                .AddSingleton<IWindowManager, WindowManager>()
                .AddSingleton<IExceptionHandler, ExceptionHandler>()
                .AddSingleton<ISettingsProvider, ClientSettingsProvider>()
                .AddSingleton<IClipBoardProvider, ClipBoardProvider>()
                .AddLogging(x =>
                {
                    x.SetMinimumLevel(LogLevel.Information);
                    x.AddSerilog(logger: serilogLogger, dispose: true);
                })

                // System
                .AddSingleton<IOperatingSystemProvider, OperatingSystemProvider>()
                .AddSingleton<IIconCache, IconCache>()
                .AddSingleton<IFileSystemProvider, FileSystemProvider>()
                .AddTransient<IFolderWatcher, FolderWatcher>()

                // ViewModels
                .AddTransient<IFolderFilterViewModel, FolderFilterViewModel>()
                .AddSingleton<IMainViewModel, MainViewModel>()
                .AddTransient<IFileManagerViewModel, FileManagerViewModel>();

            return serviceCollection.BuildServiceProvider();
        }
    }
}
