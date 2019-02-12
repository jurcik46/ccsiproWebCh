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
using WebChromiumCcsipro.Controls.Extensions;
using WebChromiumCcsipro.Controls.Interfaces;
using WebChromiumCcsipro.Controls.Interfaces.IServices;
using WebChromiumCcsipro.Controls.Messages;
using WebChromiumCcsipro.Resources;
using WebChromiumCcsipro.Resources.Language;
using WebChromiumCcsipro.Resources.Settings;
using WebChromiumCcsipro.UI.Views.SettingsWindow;
using Path = System.IO.Path;

namespace WebChromiumCcsipro.UI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
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
            UrlAddress =
                @"https://stackoverflow.com/questions/6925584/the-name-initializecomponent-does-not-exist-in-the-current-context";
            ToolTipText = Resources.Language.lang.TrayIconToolTipDefault;
        }


        #region Commands

        private void CommandInit()
        {
            //this.Options = new RelayCommand(this.ShowOptionsLogin, this.CanShowOptionsLogin);
            SettingsCommand = new RelayCommand(OpenSetting, CanOpenSetting);
            RestartCommand = new RelayCommand<IClosable>(RestartApplication, CanRestartRestartApplication);
            ExitCommand = new RelayCommand<IClosable>(ExitApplication, CanExitApplication);

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
