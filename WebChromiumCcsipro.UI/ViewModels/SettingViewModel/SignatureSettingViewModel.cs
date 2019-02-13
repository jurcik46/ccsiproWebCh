using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Serilog;
using WebChromiumCcsipro.Controls.Services;
using WebChromiumCcsipro.Resources.Interfaces.IServices;

namespace WebChromiumCcsipro.UI.ViewModels.SettingViewModel
{
    public class SignatureSettingViewModel : ViewModelBase
    {
        public ILogger Logger => Log.Logger.ForContext<SignatureSettingViewModel>();
        public RelayCommand SaveCommand { get; set; }

        public string ApiLink { get; set; }
        public string ApiKey { get; set; }
        public string ProgramPath { get; set; }
        public string ProcessName { get; set; }
        public int SignatureTimeOut { get; set; }
        private ISettingsService SettingsService { get; set; }

        public Action CloseAction { get; set; }

        public SignatureSettingViewModel(ISettingsService settingsService)
        {
            SettingsService = settingsService;
            ApiLink = SettingsService.ApiLink;
            ApiKey = SettingsService.ApiKey;
            ProgramPath = SettingsService.ProgramPath;
            ProcessName = SettingsService.ProcessName;
            SignatureTimeOut = SettingsService.SignatureTimeOut;
            SaveCommand = new RelayCommand(Save, CanSave);
        }

        private bool CanSave()
        {
            return true;
        }

        private void Save()
        {
            SettingsService.SignatureSettingSave(ApiLink, ApiKey, ProgramPath, ProcessName, SignatureTimeOut);
            if (CloseAction != null)
            {
                CloseAction();
            }

        }







    }
}
