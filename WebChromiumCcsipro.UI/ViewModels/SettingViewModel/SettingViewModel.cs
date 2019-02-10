using System;
using System.Collections.Generic;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Serilog;
using WebChromiumCcsipro.Controls.Interfaces.IServices;
using WebChromiumCcsipro.Controls.Services;

namespace WebChromiumCcsipro.UI.ViewModels.SettingViewModel
{
    public class SettingViewModel : ViewModelBase
    {
        public ILogger Logger => Log.Logger.ForContext<SettingViewModel>();

        public RelayCommand ApplicationSettingCommand { get; set; }
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
            ChangePasswordCommand = new RelayCommand(ChangePassword, CanChangePassword);
        }


        public bool CanApplicationSetting()
        {
            return true;
        }

        public void ApplicationSetting()
        {

        }

        public bool CanChangePassword()
        {
            return true;
        }

        public void ChangePassword()
        {
            var newPassword = DialogService.ChangePassword();
            //            Messenger.Default.Send<NotifiMessage>(new NotifiMessage() { Title = ViewModelLocator.rm.GetString("settingTitle"), Msg = ViewModelLocator.rm.GetString("savedSetting"), IconType = Notifications.Wpf.NotificationType.Success, ExpTime = 4 });
            // Send notification for succel change password
            Console.WriteLine(newPassword);
        }
        #endregion



    }
}
