using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Serilog;
using WebChromiumCcsipro.Resources.Language;

namespace WebChromiumCcsipro.UI.ViewModels.SettingViewModel
{
    public class ChangePasswordViewModel : ViewModelBase
    {
        public ILogger Logger => Log.Logger.ForContext<ChangePasswordViewModel>();

        private string _password1;
        private string _password2;
        private RelayCommand _changePassword;
        public string ChangePasswordButtonTooltip { get; private set; }

        public string Password1
        {
            get { return _password1; }
            set
            {
                _password1 = value;
                ChangePassword.RaiseCanExecuteChanged();
            }
        }

        public string Password2
        {
            get { return _password2; }
            set
            {
                _password2 = value;
                ChangePassword.RaiseCanExecuteChanged();
            }

        }
        public string NewPassword { get; private set; }

        public RelayCommand ChangePassword { get => _changePassword; set => _changePassword = value; }

        public ChangePasswordViewModel()
        {
            Logger.Information(ChangePasswordViewModelEvents.CreateInstance);
            this.ChangePassword = new RelayCommand(Change, CanChange, true);
        }

        private bool CanChange()
        {
            if ((!NotIdentical && !string.IsNullOrWhiteSpace(this.Password1) && Password1.Length >= 5))
            {
                ChangePasswordButtonTooltip = "";
                return true;
            }

            ChangePasswordButtonTooltip = lang.ChangePasswordWindowChangeButtonTooltip;
            return false;
        }

        private void Change()
        {
            Logger.Information(ChangePasswordViewModelEvents.ChangePasswordCommand);
            NewPassword = Password1;
            if (CloseAction != null)
            {
                CloseAction();
            }
        }

        public Action CloseAction { get; set; }

        public bool NotIdentical
        {
            get
            {
                var result = !String.Equals(Password1, Password2);
                return result;
            }
        }

    }
}
