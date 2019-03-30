using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Shell;
using Serilog;
using WebChromiumCcsipro.BusinessLogic.Services;
using WebChromiumCcsipro.Domain.Interfaces.IServices;
using WebChromiumCcsipro.UI.ViewModels;

namespace WebChromiumCcsipro.V1
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, ISingleInstanceApp
    {

        public ILogger Logger => Log.Logger.ForContext<App>();

        [STAThread]
        public static void Main()
        {
            if (SingleInstance<App>.InitializeAsFirstInstance("CCSIPro.sk Web Chromium Application"))
            {
                var application = new App();
                application.InitializeComponent();
                application.Run();
                SingleInstance<App>.Cleanup();
            }
        }

        static App()
        {
            DispatcherHelper.Initialize();
            SimpleIoc.Default.Register<ISettingsService, SettingsService>();

        }




        public App()
        {
            InitializeComponent();
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException; //all threads in appdomain
            Dispatcher.UnhandledException += CurrentDispatcherUnhandledException; //single specific ui dispatcher thread
            //Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException; // main ui dispatcher thread in application
            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException; // from each appdomain for async ops
            ViewModelLocator.SplashScreen = new SplashScreen(@"ccsi.png");
            ViewModelLocator.SplashScreen.Show(false, true);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var appName = V1.Properties.Resources.ApplicationName;
            var assembly = Assembly.GetAssembly(typeof(App));
            var app = Current;
            var logger = Logger;
            ApplicationExtensions.InitializeApplication(appName, assembly, app, logger, () => new UI.Views.MainWindow.MainWindowView(), c => V1.Properties.Resources.Culture = c);

        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            ApplicationExtensions.OnExit(Logger);
        }

        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            MainWindow?.Activate();
            return true;

        }

        private void CurrentDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            ErrorExtensions.CurrentDispatcherUnhandledException(sender, e, Logger);
        }

        private void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ErrorExtensions.CurrentDomainUnhandledException(sender, e, Logger);
        }

        private void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            ErrorExtensions.TaskSchedulerOnUnobservedTaskException(sender, e, Logger);
        }
    }
}
