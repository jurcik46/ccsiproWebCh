using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Serilog;

namespace WebChromiumCcsipro.UI.ViewModels.SettingViewModel
{
    public class EnterSettingViewModel : ViewModelBase
    {
        public ILogger Logger => Log.Logger.ForContext<EnterSettingViewModel>();

        private RelayCommand _enterCommand;

        public RelayCommand EnterCommand
        {
            get { return _enterCommand; }
            set { _enterCommand = value; }
        }

        public Action CloseAction { get; set; }

        public string Password { get; set; }

        public EnterSettingViewModel()
        {
            CommandInit();
        }

        #region CommandInit
        private void CommandInit()
        {
            EnterCommand = new RelayCommand(Enter, CanEnter);
        }

        private bool CanEnter()
        {
            return true;
        }

        private void Enter()
        {
            CloseAction?.Invoke();
        }
        #endregion

    }
}
