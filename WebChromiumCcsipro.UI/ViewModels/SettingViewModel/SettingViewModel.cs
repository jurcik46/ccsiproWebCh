using System;
using System.Collections.Generic;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Serilog;
using WebChromiumCcsipro.Resources.Language;
using WebChromiumCcsipro.Controls.Services;
using WebChromiumCcsipro.Resources.Interfaces.IServices;
using WebChromiumCcsipro.Resources.Messages;
using WebChromiumCcsipro.UI.Views.SettingsWindow;

namespace WebChromiumCcsipro.UI.ViewModels.SettingViewModel
{
    public class SettingViewModel : ViewModelBase
    {
        public ILogger Logger => Log.Logger.ForContext<SettingViewModel>();

        public RelayCommand ApplicationSettingCommand { get; set; }
        public RelayCommand SignatureSettingCommand { get; set; }
        public RelayCommand ChangePasswordCommand { get; set; }
        private ISettingsService SettingService { get; set; }
        private IDialogServiceWithOwner DialogService { get; set; }

        public SettingViewModel(ISettingsService settingService, IDialogServiceWithOwner dialogService)
        {
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
        }


        private bool CanApplicationSetting()
        {
            return true;
        }

        private void ApplicationSetting()
        {
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
            var newPassword = DialogService.ChangePassword();
            if (newPassword != null)
            {
                SettingService.CreatePassword(newPassword);
                Messenger.Default.Send(new NotifiMessage()
                {
                    Title = lang.ChangePasswodWindowNotificationTitle,
                    Msg = lang.ChangePasswodWindowNotificationInfoAbouChange,
                    IconType = Notifications.Wpf.NotificationType.Success,
                    ExpTime = 4
                });
            }
        }
        #endregion



    }
}
