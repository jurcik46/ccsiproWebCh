using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using Serilog;
using Serilog.Events;
using WebChromiumCcsipro.Controls;
using WebChromiumCcsipro.Controls.Enums;
using WebChromiumCcsipro.Controls.Extensions;
using WebChromiumCcsipro.UI.ViewModels;
using LoggerExtensions = WebChromiumCcsipro.Controls.Extensions.LoggerExtensions;

namespace WebChromiumCcsipro.V1
{
    public static class ApplicationExtensions
    {

        public static void InitializeApplication(string appName, Assembly assembly, Application app, ILogger logger, Func<Window> newWindowFunc, Action<CultureInfo> appResourcesAction)
        {
            LoggerInitializer.ApplicationName = appName;
            LoggerExtensions.DiagnosticsFunc = ErrorExtensions.DiagnosticsFunc;

            ViewModelLocator.LoggingLevelSwitch.MinimumLevel = LogEventLevel.Information;
            //            var applicationDeployment = ApplicationDeployment.IsNetworkDeployed ? ApplicationDeployment.CurrentDeployment : null;
            var currentCulture = Thread.CurrentThread.CurrentCulture;
            var currentUICulture = Thread.CurrentThread.CurrentUICulture;
            Log.Logger = LoggerInitializer.InitializeApplicationLogger(ViewModelLocator.SettingsService, ViewModelLocator.LoggingLevelSwitch, currentCulture, currentUICulture, app.GetType());
            logger.Debug(ApplicationEvents.DispatcherThread, "Dispatcher thread: {ThreadId}", DispatcherHelper.UIDispatcher.Thread.ManagedThreadId);
            var errorsTimer = new DispatcherTimer(Constants.ErrorCountInterval, DispatcherPriority.ApplicationIdle, (sender, args) => LoggerExtensions.SendErrorsWarningMessage(logger), DispatcherHelper.UIDispatcher);
            errorsTimer.Start();

            //var settingsService = ViewModelLocator.SettingsService;
            //if (settingsService != null)
            //{
            //    SetLanguage(logger, settingsService.UserSettings.Culture, false, app, null, appResourcesAction);
            //}
            //Messenger.Default.Register<ChangeLanguageMessage>(app, message =>
            //{
            //    logger.LogMessage(message, app);
            //    var culture = message.Content;
            //    SetLanguage(logger, culture, true, app, newWindowFunc, appResourcesAction);
            //});
            //            SetLanguage(logger, culture, true, app, newWindowFunc, appResourcesAction);

            //TODO Regiter messages for change language and add to Setting culture
        }

        public static void SetLanguage(ILogger logger, CultureInfo culture, bool reloadWindow, Application application, Func<Window> newWindowFunc, Action<CultureInfo> appResourcesAction)
        {
            logger.Information(ApplicationEvents.SetLanguage, "{culture}, {reloadWindow}", culture, reloadWindow);
            if (Thread.CurrentThread.CurrentUICulture.Name == culture.Name)
            {
                return;
            }
            Thread.CurrentThread.CurrentUICulture = culture;
            Thread.CurrentThread.CurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            appResourcesAction(culture);
            Resources.Language.lang.Culture = culture;
            if (!reloadWindow)
            {
                return;
            }
            SimpleIoc.Default.Unregister<MainViewModel>();
            SimpleIoc.Default.Register<MainViewModel>();
            //            ViewModelLocator.MainViewModel.IsBusy = false;
            var newWindow = newWindowFunc();
            newWindow.Show();
            foreach (var window in application.Windows.Cast<Window>().Where(win => !Equals(newWindow, win)))
            {
                window.Close();
            }
        }

        public static void OnExit(ILogger logger)
        {
            logger.Information(ApplicationEvents.ApplicationEnded, "Application ended at {DateTime}", DateTime.Now);
            ((IDisposable)Log.Logger).Dispose();
        }


    }
}


