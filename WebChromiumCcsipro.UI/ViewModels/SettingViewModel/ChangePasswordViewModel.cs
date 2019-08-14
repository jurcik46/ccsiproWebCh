using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Serilog;
using WebChromiumCcsipro.Domain.Enums;
using WebChromiumCcsipro.Domain.Extensions;
using WebChromiumCcsipro.Resources.Language;

namespace WebChromiumCcsipro.UI.ViewModels.SettingViewModel
{
    public class ChangePasswordViewModel : ViewModelBase
    {
        public ILogger Logger => Log.Logger.ForContext<ChangePasswordViewModel>();

        private string _password1;
        private string _password2;
        private string _changePasswordButtonTooltip;

        public string ChangePasswordButtonTooltip
        {
            get => _changePasswordButtonTooltip;
            private set
            {
                _changePasswordButtonTooltip = value;
                RaisePropertyChanged();
            }
        }

        public string Password1
        {
            get { return _password1; }
            set
            {
                _password1 = value;
                ChangePasswordCommand.RaiseCanExecuteChanged();
            }
        }

        public string Password2
        {
            get { return _password2; }
            set
            {
                _password2 = value;
                ChangePasswordCommand.RaiseCanExecuteChanged();
            }

        }
        public string NewPassword { get; private set; }
        public bool Result { get; set; }
        public RelayCommand ChangePasswordCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }

        public ChangePasswordViewModel()
        {
            Logger.Information(ChangePasswordViewModelEvents.CreateInstance, "Creating new instance of ChangePasswordViewModel");
            Result = false;
            ChangePasswordCommand = new RelayCommand(Change, CanChange);
            CancelCommand = new RelayCommand(() => CloseAction?.Invoke());

        }

        private bool CanChange()
        {
            if ((!NotIdentical && !string.IsNullOrWhiteSpace(this.Password1) && Password1.Length >= 5))
            {
                ChangePasswordButtonTooltip = null;
                return true;
            }

            ChangePasswordButtonTooltip = lang.ChangePasswordWindowChangeButtonTooltip;
            return false;
        }

        private void Change()
        {
            Logger.Information(ChangePasswordViewModelEvents.ChangePasswordCommand);
            NewPassword = Password1;
            Result = true;
            CloseAction?.Invoke();
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
