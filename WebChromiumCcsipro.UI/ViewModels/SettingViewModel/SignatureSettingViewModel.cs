using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using Serilog;
using WebChromiumCcsipro.Controls.Interfaces.IServices;
using WebChromiumCcsipro.Controls.Services;

namespace WebChromiumCcsipro.UI.ViewModels.SettingViewModel
{
    public class SignatureSettingViewModel : ViewModelBase
    {
        public ILogger Logger => Log.Logger.ForContext<SignatureSettingViewModel>();

        public string ApiLink { get; set; }
        public string ApiKey { get; set; }

        public string ProgramPath { get; set; }

        public string ProcessName { get; set; }

        public string SignatureTimeOut { get; set; }
        private ISettingsService SettingsService { get; set; }

        public SignatureSettingViewModel(ISettingsService settingsService)
        {
            SettingsService = settingsService;

        }




    }
}
