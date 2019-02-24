﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
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
        public RelayCommand HomeCommand { get; set; }
        public RelayCommand SettingsCommand { get; set; }
        public RelayCommand<IClosable> RestartCommand { get; set; }
        public RelayCommand<IClosable> ExitCommand { get; set; }

        private Window SettingWindow { get; set; }
        private ISettingsService SettingService { get; set; }
        private IDialogServiceWithOwner DialogService { get; set; }

        private ISignatureService SignatureService { get; set; }

        public string ToolTipText
        {
            get { return _toolTipText; }
            set
            {
                _toolTipText = value;
                RaisePropertyChanged();
            }
        }

        public string HomeUrl { get; set; }

        public string UrlAddress
        {
            get { return _urlAddress; }
            set
            {
                //if (value.Contains(SettingService.AllowedSite))
                _urlAddress = value;

                //else
                //{
                //    _urlAddress = SettingService.HomePage;
                //}
                Console.WriteLine(value);
                RaisePropertyChanged();

            }
        }

        public MainViewModel(ISettingsService settingService, IDialogServiceWithOwner dialogService, ISignatureService signatureService)
        {
            Logger.Information(MainViewModelEvents.CreateInstance);
            SettingService = settingService;
            DialogService = dialogService;
            SignatureService = signatureService;
            CommandInit();
            MessagesInit();
            UrlAddress = SettingService.AllowedSite;
            HomeUrl = SettingService.HomePage;
            ToolTipText = lang.TrayIconToolTipDefault;
        }


        #region Message and Command Init
        private void MessagesInit()
        {
            Messenger.Default.Register<TrayIconsStatusMessage>(this, (message) =>
            {
                Logger.Information(MainViewModelEvents.TrayIconStatus, $"Status: {message.IconStatus}");
                switch (message.IconStatus)
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
            HomeCommand = new RelayCommand(() => UrlAddress = HomeUrl, () => true);
            RestartCommand = new RelayCommand<IClosable>(RestartApplication, CanRestartRestartApplication);
            ExitCommand = new RelayCommand<IClosable>(ExitApplication, CanExitApplication);

        }


        private bool CanSing()
        {
            if (!SignatureService.InProcces)
            {
                return true;
            }
            Messenger.Default.Send(new NotifiMessage() { Title = lang.SignatureServiceNotificationTitle, Msg = lang.SignatureServiceNotificationInProccess, IconType = Notifications.Wpf.NotificationType.Error, ExpTime = 5 });
            return false;
        }

        private void SingDocument()
        {
            Logger.Information(MainViewModelEvents.SingDocumentCommand);
            Task.Run(() =>
            {
                SignatureService.StartSign();

            });
        }



        private bool CanExitApplication(IClosable win)
        {
            return true;
        }


        private void ExitApplication(IClosable win)
        {
            Logger.Information(MainViewModelEvents.ExitApplicationCommand);
            if (win != null)
            {
                win.Close();

            }
            foreach (var window in Application.Current.Windows.Cast<Window>())
            {
                window.Close();
            }
            Application.Current.Shutdown();
        }

        private bool CanRestartRestartApplication(IClosable win)
        {
            return true;
        }

        private void RestartApplication(IClosable win)
        {
            Logger.Information(MainViewModelEvents.RestartApplicationCommand);
            if (win != null)
            {
                win.Close();
            }

            Application.Current.Shutdown();
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
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
            Logger.Information(MainViewModelEvents.OpenSettingCommand);

            if (!CryptoExtension.VerifyPassword(DialogService.EnterSetting(),
                CCSIproChromiumSetting.Default.PasswordSalt,
                CCSIproChromiumSetting.Default.PasswordSetting))
            {
                Messenger.Default.Send(new NotifiMessage() { Title = lang.EnterSettingWindowNotificationTitle, Msg = lang.EnterSettingWindowNotificationFailedLogin, IconType = Notifications.Wpf.NotificationType.Error, ExpTime = 4 });
                Logger.Debug(MainViewModelEvents.BadPasswordToOptions);

                return;
            }
            Messenger.Default.Send(new NotifiMessage() { Title = lang.EnterSettingWindowNotificationTitle, Msg = lang.EnterSettingWindowNotificationSuccessLogin, IconType = Notifications.Wpf.NotificationType.Success, ExpTime = 4 });

            SettingWindow = new SettingWindowView();
            var settingViewModel = new SettingViewModel.SettingViewModel(SettingService, DialogService);
            SettingWindow.DataContext = settingViewModel;
            SettingWindow.ShowDialog();
        }
        #endregion



    }
}
