using System;
using System.Net;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Serilog;
using WebChromiumCcsipro.Domain.Enums;
using WebChromiumCcsipro.Domain.Extensions;
using WebChromiumCcsipro.Domain.Interfaces.IServices;

namespace WebChromiumCcsipro.UI.ViewModels.SettingViewModel
{
    public class ServerSettingViewModel : ViewModelBase
    {
        public ILogger Logger => Log.Logger.ForContext<ServerSettingViewModel>();
        public Action CloseAction { get; set; }
        public RelayCommand SaveCommand { get; set; }
        private string _serverIp;
        private string _kioskIp;
        public string ServerIp
        {
            get { return _serverIp; }
            set
            {
                _serverIp = value;
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        public string KioskIp
        {
            get { return _kioskIp; }
            set
            {
                _kioskIp = value;
                SaveCommand.RaiseCanExecuteChanged();
            }
        }
        public int ServerPort { get; set; }
        public int KioskPort { get; set; }

        private ISettingsService _settingsService;

        public ServerSettingViewModel(ISettingsService settingsService)
        {
            Logger.Information(ServerSettingViewModelEvents.CreateInstance, "Creating new instance of ServerSettingViewModel");
            SaveCommand = new RelayCommand(Save, CanSave);
            _settingsService = settingsService;
            ServerIp = _settingsService.ServerIp;
            ServerPort = _settingsService.ServerPort;
            KioskIp = _settingsService.KioskIp;
            KioskPort = _settingsService.KioskPort;
        }

        private bool CanSave()
        {
            IPAddress address;
            return IPAddress.TryParse(ServerIp, out address) && IPAddress.TryParse(KioskIp, out address);
        }

        private void Save()
        {
            Logger.Information(ServerSettingViewModelEvents.SaveSettingCommand, $"Server IP: {ServerIp} " +
                                                                                     $"Server Port: {ServerPort}");
            _settingsService.ServerSettingSave(ServerIp, ServerPort, KioskIp, KioskPort);
            CloseAction?.Invoke();
        }


    }
}