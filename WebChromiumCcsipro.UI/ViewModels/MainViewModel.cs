using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Serilog;
using WebChromiumCcsipro.Controls;
using WebChromiumCcsipro.Resources;
using WebChromiumCcsipro.Resources.Enums;
using WebChromiumCcsipro.Resources.Extensions;
using WebChromiumCcsipro.Resources.Interfaces;
using WebChromiumCcsipro.Resources.Interfaces.IServices;
using WebChromiumCcsipro.Resources.Language;
using WebChromiumCcsipro.Resources.Messages;
using WebChromiumCcsipro.Resources.Settings;
using WebChromiumCcsipro.UI.Views.SettingsWindow;
using Path = System.IO.Path;

namespace WebChromiumCcsipro.UI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {

        public string Version
        {
            get
            {
                if (IsInDesignModeStatic)
                {
                    return "1.0.0.0";
                }

                return LoggerInitializer.Version;
            }
        }

        public string VersionDeploy
        {
            get
            {
                if (IsInDesignModeStatic)
                {
                    return "1.0.0.0";
                }

                return LoggerInitializer.VersionDeploy;
            }
        }

        public ILogger Logger => Log.Logger.ForContext<MainViewModel>();

        private string _urlAddress;
        private string _toolTipText;

        public RelayCommand SignatureCommand { get; set; }
        public RelayCommand SettingsCommand { get; set; }
        public RelayCommand<IClosable> RestartCommand { get; set; }
        public RelayCommand<IClosable> ExitCommand { get; set; }

        private Window SettingWindow { get; set; }
        private ISettingsService SettingService { get; set; }
        private IDialogServiceWithOwner DialogService { get; set; }

        public string ToolTipText
        {
            get { return _toolTipText; }
            set
            {
                _toolTipText = value;
                RaisePropertyChanged();
            }
        }

        public string UrlAddress
        {
            get { return _urlAddress; }
            set { _urlAddress = value; }
        }

        public MainViewModel(ISettingsService settingService, IDialogServiceWithOwner dialogService)
        {
            SettingService = settingService;
            DialogService = dialogService;
            CommandInit();
            MessagesInit();
            UrlAddress =
                @"https://stackoverflow.com/questions/6925584/the-name-initializecomponent-does-not-exist-in-the-current-context";
            ToolTipText = Resources.Language.lang.TrayIconToolTipDefault;
        }


        #region Message and Command Init
        private void MessagesInit()
        {
            Messenger.Default.Register<ChangeIconMessage>(this, (message) =>
            {
                switch (message.Icon)
                {
                    case TrayIconsStatus.Online:
                        ToolTipText = lang.TrayIconToolTipDefault;
                        break;
                    case TrayIconsStatus.Offline:
                        ToolTipText = lang.TrayIconToolTipLostConnection;
                        break;
                    case TrayIconsStatus.Working:
                        ToolTipText = lang.TrayIconToolTipSignatureWorking;
                        break;
                    default:
                        break;
                }
            });
        }
        #endregion
        #region Commands

        private void CommandInit()
        {
            SignatureCommand = new RelayCommand(SingDocument, CanSing);
            SettingsCommand = new RelayCommand(OpenSetting, CanOpenSetting);
            RestartCommand = new RelayCommand<IClosable>(RestartApplication, CanRestartRestartApplication);
            ExitCommand = new RelayCommand<IClosable>(ExitApplication, CanExitApplication);

        }

        private bool CanSing()
        {

        }

        private void SingDocument()
        {

        }



        private bool CanExitApplication(IClosable win)
        {
            return true;
        }


        private void ExitApplication(IClosable win)
        {
            if (win != null)
            {
                win.Close();

            }

            Application.Current.Shutdown();
        }

        private bool CanRestartRestartApplication(IClosable win)
        {
            return true;
        }

        private void RestartApplication(IClosable win)
        {
            if (win != null)
            {
                win.Close();
            }
            //TODO restart app 
            //Application.Current.Shutdown();
            //System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
        }

        private bool CanOpenSetting()
        {
            if (SettingWindow != null)
                return (SettingWindow.IsLoaded) ? false : true;
            else
                return true;
        }

        private void OpenSetting()
        {
            if (!CryptoExtension.VerifyPassword(DialogService.EnterSetting(),
                CCSIproChromiumSetting.Default.PasswordSalt,
                CCSIproChromiumSetting.Default.PasswordSetting))
            {
                Messenger.Default.Send(new NotifiMessage() { Title = lang.EnterSettingWindowNotificationTitle, Msg = lang.EnterSettingWindowNotificationFailedLogin, IconType = Notifications.Wpf.NotificationType.Error, ExpTime = 4 });

                return;
            }
            Messenger.Default.Send(new NotifiMessage() { Title = lang.EnterSettingWindowNotificationTitle, Msg = lang.EnterSettingWindowNotificationSuccessLogin, IconType = Notifications.Wpf.NotificationType.Success, ExpTime = 4 });

            SettingWindow = new SettingWindowView();
            var settingViewModel = new SettingViewModel.SettingViewModel(SettingService, DialogService);
            SettingWindow.DataContext = settingViewModel;
            SettingWindow.Show();
        }
        #endregion



    }
}
