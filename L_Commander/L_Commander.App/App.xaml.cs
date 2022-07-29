using System;
using System.IO;
using System.Windows;
using L_Commander.App.Infrastructure;
using L_Commander.App.Infrastructure.Db;
using L_Commander.App.Infrastructure.History;
using L_Commander.App.OperatingSystem;
using L_Commander.App.OperatingSystem.Operations;
using L_Commander.App.OperatingSystem.Registry;
using L_Commander.App.ViewModels;
using L_Commander.App.ViewModels.Factories;
using L_Commander.App.ViewModels.Filtering;
using L_Commander.App.ViewModels.History;
using L_Commander.App.ViewModels.Settings;
using L_Commander.App.Views;
using L_Commander.App.Views.DesignMock;
using L_Commander.UI.Commands;
using L_Commander.UI.Infrastructure;
using Microsoft.EntityFrameworkCore;
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
        private static ServiceProvider _serviceProvider;
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

        public static ServiceProvider ServiceProvider => _serviceProvider;

        private void CommandException(object? sender, ExceptionEventArgs e)
        {
            var exceptionHandler = _serviceProvider.GetService<IExceptionHandler>();
            exceptionHandler?.HandleExceptionWithMessageBox(e.Exception);
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

                // Database
                .AddDbContext<LCommanderDbContext>((app, opt) =>
                { })

                // Infrastructure
                .AddSingleton<IDispatcher>(dispatcherAdapter)
                .AddSingleton<IWindowManager, WindowManager>()
                .AddSingleton<IExceptionHandler, ExceptionHandler>()
                .AddSingleton<ISettingsManager, SettingsManager>()
                .AddSingleton<ITagRepository, TagRepository>()
                .AddSingleton<IClipBoardProvider, ClipBoardProvider>()
                .AddSingleton<IHistoryManager, DesignMockHistoryManager>()
                .AddLogging(x =>
                {
                    x.SetMinimumLevel(LogLevel.Information);
                    x.AddSerilog(logger: serilogLogger, dispose: true);
                })

                // System
                .AddSingleton<IProcessProvider, ProcessProvider>()
                .AddSingleton<IIconCache, IconCache>()
                .AddSingleton<IFileSystemProvider, FileSystemProvider>()
                .AddSingleton<IRegistryProvider, RegistryProvider>()
                .AddSingleton<IApplicationsProvider, ApplicationsProvider>()
                .AddTransient<IFolderWatcher, FolderWatcher>()
                .AddSingleton<ICopyOperation, CopyOperation>()
                .AddSingleton<IMoveOperation, MoveOperation>()
                .AddSingleton<IDeleteOperation, DeleteOperation>()

                .AddSingleton<ISettingsFiller, MainViewModel>(x => (MainViewModel)x.GetService<IMainViewModel>())
                .AddSingleton<ISettingsFiller, SettingsViewModel>(x => (SettingsViewModel)x.GetService<ISettingsViewModel>())

                // Factories
                .AddSingleton<IFileManagerTabViewModelFactory, FileManagerTabViewModelFactory>()
                .AddSingleton<ISettingsItemsViewModelFactory, SettingsItemsViewModelFactory>()
                .AddSingleton<IFileSystemEntryViewModelFactory, FileSystemEntryViewModelFactory>()

                // ViewModels
                .AddSingleton<IAddTagViewModel, AddTagViewModel>()
                .AddTransient<IFolderFilterViewModel, FolderFilterViewModel>()
                .AddSingleton<IMainViewModel, MainViewModel>()
                .AddSingleton<ISettingsViewModel, SettingsViewModel>()
                .AddSingleton<IContextMenuItemProvider, ContextMenuItemProvider>()
                .AddSingleton<IOpenWithViewModel, OpenWithViewModel>()
                .AddSingleton<IHistoryViewModel, HistoryViewModel>()
                .AddTransient<IFileManagerViewModel, FileManagerViewModel>();


            return serviceCollection.BuildServiceProvider();
        }
    }
}
