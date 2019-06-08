using System;
using WebChromiumCcsipro.Domain.Interfaces.IServices;

namespace WebChromiumCcsipro.UI.ViewModels.SettingViewModel
{
    public class ServerSettingViewModel
    {
        public Action CloseAction { get; set; }

        private ISettingsService _settingsService;
        public ServerSettingViewModel(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }


    }
}