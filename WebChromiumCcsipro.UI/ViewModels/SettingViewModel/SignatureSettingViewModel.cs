using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Serilog;
using WebChromiumCcsipro.Domain.Enums;
using WebChromiumCcsipro.Domain.Extensions;
using WebChromiumCcsipro.Domain.Interfaces.IServices;

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
            Logger.Information(SignatureSettingViewModelEvents.CreateInstance, "Creating new instance of SignatureSettingViewModel");
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
            Logger.Information(SignatureSettingViewModelEvents.SaveSettingCommand, $"ApiLink: {ApiLink} ApiKey: {ApiKey} " +
                                                                                   $"ProgramPath: {ProgramPath} ProcessName: {ProcessName} SignatureTimeOut: {SignatureTimeOut}");
            SettingsService.SignatureSettingSave(ApiLink, ApiKey, ProgramPath, ProcessName, SignatureTimeOut);
            if (CloseAction != null)
            {
                CloseAction();
            }

        }
    }
}
