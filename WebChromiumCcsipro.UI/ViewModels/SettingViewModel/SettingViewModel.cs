using System;
using System.Collections.Generic;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Serilog;
using WebChromiumCcsipro.Domain.Enums;
using WebChromiumCcsipro.Domain.Extensions;
using WebChromiumCcsipro.Domain.Interfaces.IServices;
using WebChromiumCcsipro.Domain.Messages;
using WebChromiumCcsipro.Resources.Language;
using WebChromiumCcsipro.UI.Views.SettingsWindow;

namespace WebChromiumCcsipro.UI.ViewModels.SettingViewModel
{
    public class SettingViewModel : ViewModelBase
    {
        public ILogger Logger => Log.Logger.ForContext<SettingViewModel>();

        public RelayCommand ApplicationSettingCommand { get; set; }
        public RelayCommand SignatureSettingCommand { get; set; }
        public RelayCommand ChangePasswordCommand { get; set; }
        public RelayCommand ServerSettingCommand { get; set; }
        private ISettingsService SettingService { get; set; }
        private IDialogServiceWithOwner DialogService { get; set; }

        public SettingViewModel(ISettingsService settingService, IDialogServiceWithOwner dialogService)
        {
            Logger.Information(SettingViewModelEvents.CreateInstance, "Creating new instance of SettingViewModel");
            SettingService = settingService;
            DialogService = dialogService;
            CommandInit();
        }

        #region Commands init

        private void CommandInit()
        {
            ApplicationSettingCommand = new RelayCommand(ApplicationSetting, CanApplicationSetting);
            SignatureSettingCommand = new RelayCommand(SignatureSetting, CanSignatureSetting);
            ChangePasswordCommand = new RelayCommand(ChangePassword, CanChangePassword);
            ServerSettingCommand = new RelayCommand(ServerSetting, CanServerSetting);
        }

        private bool CanServerSetting()
        {
            return true;
        }

        private void ServerSetting()
        {
            Logger.Information(SettingViewModelEvents.ServerSettingCommand);
            var viewModel = new ServerSettingViewModel(SettingService);
            var window = new ServerSettingWindowView();
            viewModel.CloseAction = () => window.Close();
            window.DataContext = viewModel;
            window.ShowDialog();
        }

        private bool CanApplicationSetting()
        {
            return true;
        }

        private void ApplicationSetting()
        {
            Logger.Information(SettingViewModelEvents.ApplicationSettingCommand);
            var viewModel = new ApplicationSettingViewModel(SettingService);
            var window = new ApplicationSettingWindowView();
            viewModel.CloseAction = () => window.Close();
            window.DataContext = viewModel;
            window.ShowDialog();
        }

        private bool CanSignatureSetting()
        {
            return true;
        }

        private void SignatureSetting()
        {
            Logger.Information(SettingViewModelEvents.SignatureSettingCommand);
            var viewModel = new SignatureSettingViewModel(SettingService);
            var window = new SignatureSettingWindowView();
            viewModel.CloseAction = () => window.Close();
            window.DataContext = viewModel;
            window.ShowDialog();
        }


        private bool CanChangePassword()
        {
            return true;
        }

        private void ChangePassword()
        {
            Logger.Information(SettingViewModelEvents.ChangePasswordCommand);
            var newPassword = DialogService.ChangePassword();
            if (newPassword != null)
            {
                SettingService.CreatePassword(newPassword);
                Logger.Information(SettingViewModelEvents.ChangePasswordSuccessful);
                Messenger.Default.Send(new NotifyMessage()
                {
                    Title = lang.ChangePasswodWindowNotificationTitle,
                    Msg = lang.ChangePasswodWindowNotificationInfoAbouChange,
                    IconType = Notifications.Wpf.NotificationType.Success,
                });
            }
        }
        #endregion



    }
}
