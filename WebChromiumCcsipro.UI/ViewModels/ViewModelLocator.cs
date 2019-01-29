using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Threading;
using Serilog.Core;
using WebChromiumCcsipro.Controls.Interfaces.IServices;
using WebChromiumCcsipro.Controls.Services;

namespace WebChromiumCcsipro.UI.ViewModels
{
    public static class ViewModelLocator
    {
        private static LoggingLevelSwitch _loggingLevelSwitch;
        //        private static bool _initialized;

        static ViewModelLocator()
        {
            //            _initialized = false;
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            if (ViewModelBase.IsInDesignModeStatic)
            {
                // Create design time view services and models
                //SimpleIoc.Default.Register<ITestService, DesignTestService>();
                //                SimpleIoc.Default.Register<IVideoService, VideoService>();
            }
            else
            {
                // Create run time view services and models
                //SimpleIoc.Default.Register<ITestService, TestService>();
                SimpleIoc.Default.Register<ISettingsService, SettingsService>();
            }
            RegisterViewModels();

        }

        internal static void RegisterViewModels()
        {
            if (!SimpleIoc.Default.IsRegistered<MainViewModel>())
            {
                SimpleIoc.Default.Register<MainViewModel>();
            }
        }

        public static LoggingLevelSwitch LoggingLevelSwitch => _loggingLevelSwitch ?? (_loggingLevelSwitch = new LoggingLevelSwitch());
        public static SplashScreen SplashScreen { get; set; }

        public static ISettingsService SettingsService => ServiceLocator.Current.GetInstance<ISettingsService>();
        public static MainViewModel MainViewModel => ServiceLocator.Current.GetInstance<MainViewModel>();




    }
}
