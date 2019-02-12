using System;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight.Threading;
using Serilog;
using WebChromiumCcsipro.Controls.Enums;
using WebChromiumCcsipro.Controls.Extensions;
using WebChromiumCcsipro.Controls.Interfaces.IServices;
using WebChromiumCcsipro.UI.ViewModels.SettingViewModel;
using WebChromiumCcsipro.UI.Views.SettingsWindow;

namespace WebChromiumCcsipro.UI
{
    public class DialogService : IDialogServiceWithOwner
    {
        public ILogger Logger => Log.Logger.ForContext<DialogService>();

        public bool HideAllErrors { get; set; }
        public Window Owner { get; set; }

        public DialogService()
        {
            HideAllErrors = false;
        }

        public string ChangePassword()
        {
            Logger.Debug(DialogServiceEvents.ChangePassword);
            var viewModel = new ChangePasswordViewModel();
            var window = new ChangePasswordWindowView();
            window.DataContext = viewModel;
            //            window.Owner = Owner;
            viewModel.CloseAction = () => window.Close();

            var result = window.ShowDialog();

            if (result == false)
            {
                Logger.Warning(DialogServiceEvents.ChangePasswordCancel, "Change password dialog was canceled.");
                return null;
            }
            if (result == true)
            {
                Logger.Information(DialogServiceEvents.ChangePasswordSuccess, "Password was changed.");
            }

            return viewModel.NewPassword;
        }

        public string EnterSetting()
        {
            Logger.Debug(DialogServiceEvents.EnterSetting);
            var viewModel = new EnterSettingViewModel();
            var window = new EnterSettingWindowView();
            window.DataContext = viewModel;
            viewModel.CloseAction = () => window.Close();

            window.ShowDialog();
            return viewModel.Password;
        }

        public Task ShowError(string message, string title, string buttonText, Action afterHideCallback)
        {
            return ShowError(message, title, buttonText, afterHideCallback, false);

        }

        public Task ShowError(Exception error, string title, string buttonText, Action afterHideCallback)
        {
            if (error == null)
            {
                error = new ArgumentNullException(nameof(error), @"Exception passed to ShowError was null.");
            }
            Logger.Error(error, DialogServiceEvents.ShowError);
            var task = Task.Run(() =>
            {
                if (Owner != null)
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(() => ShowError(error.Message, title, afterHideCallback, false));
                }
                else
                {
                    ShowError(error.Message, title, afterHideCallback, false);
                }
            });
            return task;
        }

        public Task ShowError(string message, string title, string buttonText, Action afterHideCallback, bool alwaysShow)
        {
            var task = Task.Run(() =>
            {
                if (Owner != null)
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(() => ShowError(message, title, afterHideCallback, alwaysShow));
                }
                else
                {
                    ShowError(message, title, afterHideCallback, alwaysShow);
                }
            });
            return task;
        }

        private void ShowError(string message, string title, Action afterHideCallback, bool alwaysShow)
        {
            if (HideAllErrors && !alwaysShow)
            {
                Logger.Warning(DialogServiceEvents.HiddenError, "Error dialog is hidden. {Title}: {ErrorMessage}", title, message);
            }
            else
            {
                Logger.Information(DialogServiceEvents.ShowErrorDialog, "{Title}: {ErrorMessage}", title, message);
                if (Owner != null)
                {
                    MessageBox.Show(Owner, message, title, MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            afterHideCallback?.Invoke();
        }

        public Task ShowMessage(string message, string title)
        {
            var task = Task.Run(() =>
            {
                if (Owner != null)
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(() => ShowInformation(message, title, null));
                }
                else
                {
                    ShowInformation(message, title, null);
                }
            });
            return task;
        }

        public Task ShowMessage(string message, string title, string buttonText, Action afterHideCallback)
        {
            var task = Task.Run(() =>
            {
                if (Owner != null)
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(() => ShowInformation(message, title, afterHideCallback));
                }
                else
                {
                    ShowInformation(message, title, afterHideCallback);
                }
            });
            return task;
        }

        public Task<bool> ShowMessage(string message, string title, string buttonConfirmText, string buttonCancelText,
            Action<bool> afterHideCallback)
        {
            var task = Task.Run(() =>
            {
                if (Owner != null)
                {
                    var result = false;
                    DispatcherHelper.RunAsync(() =>
                    {
                        result = ShowConfirmation(message, title, afterHideCallback, false);
                    }).Wait();
                    return result;
                }
                else
                {
                    var result = ShowConfirmation(message, title, afterHideCallback, false);
                    return result;
                }
            });
            return task;
        }

        public Task<bool> ShowMessage(string message, string title, string buttonConfirmText, string buttonCancelText,
            Action<bool> afterHideCallback, bool defaultCancel)
        {
            var task = Task.Run(() =>
            {
                if (Owner != null)
                {
                    var result = false;
                    DispatcherHelper.RunAsync(() =>
                    {
                        result = ShowConfirmation(message, title, afterHideCallback, defaultCancel);
                    }).Wait();
                    return result;
                }
                else
                {
                    var result = ShowConfirmation(message, title, afterHideCallback, defaultCancel);
                    return result;
                }
            });
            return task;
        }

        public Task ShowWarning(string message, string title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                title = Resources.Language.lang.WarningCaption;
            }
            Logger.Warning(DialogServiceEvents.ShowWarning, "{Title}: {ErrorMessage}", title, message);
            var task = Task.Run(() =>
            {
                if (Owner != null)
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(() => MessageBox.Show(Owner, message, title, MessageBoxButton.OK, MessageBoxImage.Warning));
                }
                else
                {
                    MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            });
            return task;
        }

        public Task ShowMessageBox(string message, string title)
        {
            var task = Task.Run(() =>
            {
                if (Owner != null)
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(() => ShowInformation(message, title, null));
                }
                else
                {
                    ShowInformation(message, title, null);
                }
            });
            return task;
        }

        private void ShowInformation(string message, string title, Action afterHideCallback)
        {
            Logger.Information(DialogServiceEvents.ShowInformation, "{Title}: {Message}", title, message);
            MessageBox.Show(Owner, message, title, MessageBoxButton.OK, MessageBoxImage.Information);
            afterHideCallback?.Invoke();
        }

        private bool ShowConfirmation(string message, string title, Action<bool> afterHideCallback, bool defaultCancel)
        {
            Logger.Debug(DialogServiceEvents.ShowConfirmation, "{Title}: {Message}", title, message);
            var response = MessageBox.Show(Owner, message, title, MessageBoxButton.OKCancel, MessageBoxImage.Warning, defaultCancel ? MessageBoxResult.Cancel : MessageBoxResult.OK);
            var confirm = response == MessageBoxResult.OK;
            afterHideCallback?.Invoke(confirm);
            Logger.Information(DialogServiceEvents.ShowConfirmation, "{Title}: {Message} Confirmed: {Confirmed}", title, message, confirm);
            return confirm;
        }


    }
}
