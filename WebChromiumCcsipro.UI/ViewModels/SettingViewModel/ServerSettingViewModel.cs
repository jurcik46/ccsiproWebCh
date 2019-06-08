using System;
using System.Net;
using GalaSoft.MvvmLight.Command;
using Serilog;
using WebChromiumCcsipro.Domain.Enums;
using WebChromiumCcsipro.Domain.Extensions;
using WebChromiumCcsipro.Domain.Interfaces.IServices;

namespace WebChromiumCcsipro.UI.ViewModels.SettingViewModel
{
    public class ServerSettingViewModel
    {
        public ILogger Logger => Log.Logger.ForContext<ServerSettingViewModel>();
        public Action CloseAction { get; set; }
        public RelayCommand SaveCommand { get; set; }
        private string _serverIp;
        public string ServerIp
        {
            get { return _serverIp; }
            set
            {
                _serverIp = value;
                SaveCommand.RaiseCanExecuteChanged();
            }
        }
        public int ServerPort { get; set; }

        private ISettingsService _settingsService;

        public ServerSettingViewModel(ISettingsService settingsService)
        {
            Logger.Information(ServerSettingViewModelEvents.CreateInstance);
            SaveCommand = new RelayCommand(Save, CanSave);
            _settingsService = settingsService;
            ServerIp = _settingsService.ServerIp;
            ServerPort = _settingsService.ServerPort;
        }

        private bool CanSave()
        {
            IPAddress address;
            return IPAddress.TryParse(ServerIp, out address);
        }

        private void Save()
        {
            Logger.Information(ServerSettingViewModelEvents.SaveSettingCommand, $"Server IP: {ServerIp} " +
                                                                                     $"Server Port: {ServerPort}");
            _settingsService.ServerSettingSave(ServerIp, ServerPort);
            CloseAction?.Invoke();
        }


    }
}