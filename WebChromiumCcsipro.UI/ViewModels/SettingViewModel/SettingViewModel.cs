using System.Collections.Generic;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using WebChromiumCcsipro.Controls.Interfaces.IServices;

namespace WebChromiumCcsipro.UI.ViewModels.SettingViewModel
{
    public class SettingViewModel : ViewModelBase
    {
        public RelayCommand ApplicationSettingCommand { get; set; }
        public RelayCommand ChangePasswordCommand { get; set; }
        private ISettingsService SettingService { get; set; }


        public SettingViewModel(ISettingsService settingService)
        {
            SettingService = settingService;
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


        }

        #endregion



    }
}
