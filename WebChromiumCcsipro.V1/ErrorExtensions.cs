using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Serilog;
using WebChromiumCcsipro.BusinessLogic;
using WebChromiumCcsipro.Domain.Enums;
using WebChromiumCcsipro.Domain.Extensions;
using WebChromiumCcsipro.Domain.Interfaces;
using WebChromiumCcsipro.UI.ViewModels;


namespace WebChromiumCcsipro.V1
{
    public static class ErrorExtensions
    {

        public static IDiagnostics DiagnosticsFunc(bool includeStackTrace)
        {
            var result = new Diagnostics
            {
                CommandLine = Environment.CommandLine,
                Version = ViewModelLocator.MainViewModel?.Version,
                VersionDeploy = ViewModelLocator.MainViewModel?.VersionDeploy,
                //                IsNetworkDeployed = ApplicationDeployment.IsNetworkDeployed,
                CurrentDirectory = Environment.CurrentDirectory,
                UserDomainName = Environment.UserDomainName,
                MachineName = Environment.MachineName,
                UserName = Environment.UserName,
                OSVersion = Environment.OSVersion,
                SystemDirectory = Environment.SystemDirectory,
                Is64BitOperatingSystem = Environment.Is64BitOperatingSystem,
                Is64BitProcess = Environment.Is64BitProcess,
                ProcessorCount = Environment.ProcessorCount,
                StackTrace = includeStackTrace ? Environment.StackTrace : string.Empty,
                UpTime = TimeSpan.FromMilliseconds(Environment.TickCount),
                CLRVersion = Environment.Version.ToString(),
                //                WorkingSet = string.Format(CultureInfo.InvariantCulture, "{0} MB", Environment.WorkingSet.Bytes().Megabytes)
            };
            return result;
        }

        public static void CurrentDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e, ILogger logger)
        {
            logger.Fatal(e.Exception, ApplicationEvents.DispatcherUnhandledException, "Dispatcher unhandled exception: {ErrorMessage}", true, e.Exception.Message);
            var errorMessage = string.Format(CultureInfo.CurrentCulture, Resources.Language.lang.ErrorMessageFormat, e.Exception.Message);
            var dialogService = ViewModelLocator.DialogService;
            if (dialogService != null)
            {
                dialogService.ShowError(errorMessage, Resources.Language.lang.ErrorCaption, null, null, true);
            }
            else
            {
                MessageBox.Show(errorMessage, Resources.Language.lang.ErrorCaption, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            ((IDisposable)Log.Logger).Dispose();
            Thread.Sleep(6000);
            Environment.Exit(1);
        }

        public static void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e, ILogger logger)
        {
            var ex = e.ExceptionObject as Exception;
            if (!e.IsTerminating)
            {
                if (ex != null)
                {
                    logger.Error(ex, ApplicationEvents.CurrentDomainUnhandledException, "CurrentDomain unhandled exception, but not terminating.");
                }
                else
                {
                    logger.Error(ApplicationEvents.CurrentDomainUnhandledException, "CurrentDomain unhandled exception, but not terminating. ExceptionObject: {ExceptionObject}", e.ExceptionObject);
                }
                return;
            }
            if (ex != null)
            {
                logger.Fatal(ex, ApplicationEvents.CurrentDomainUnhandledException, "CurrentDomain unhandled exception, terminating.", true);
            }
            else
            {
                logger.Fatal(ApplicationEvents.CurrentDomainUnhandledException, "CurrentDomain unhandled exception, terminating. ExceptionObject: {ExceptionObject}", true, e.ExceptionObject);
            }
            var errorMessage = Resources.Language.lang.ErrorCaption;
            if (ex != null)
            {
                errorMessage = string.Format(CultureInfo.CurrentCulture, Resources.Language.lang.ErrorMessageFormat, ex.Message);
            }
            var dialogService = ViewModelLocator.DialogService;
            if (dialogService != null)
            {
                dialogService.ShowError(errorMessage, Resources.Language.lang.ErrorCaption, null, null);
            }
            else
            {
                MessageBox.Show(errorMessage, Resources.Language.lang.ErrorCaption, MessageBoxButton.OK, MessageBoxImage.Error);
            }
                ((IDisposable)Log.Logger).Dispose();
        }

        public static void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e, ILogger logger)
        {
            logger.Error(e.Exception, ApplicationEvents.UnobservedTaskException, "TaskException marked with {Observered} as observerd.", e.Observed);
        }
    }
}
