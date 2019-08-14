using System;
using System.Threading.Tasks;
using CommonServiceLocator;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Serilog;
using WebChromiumCcsipro.Domain.Enums;
using WebChromiumCcsipro.Domain.Extensions;
using WebChromiumCcsipro.Domain.Interfaces.IServices;
using WebChromiumCcsipro.Resources.Language;
using WebChromiumCcsipro.UI.ViewModels.SettingViewModel;
using WebChromiumCcsipro.UI.Views.SettingsWindow;

namespace WebChromiumCcsipro.UI
{
    public class MetroDialogService : IMetroDialogServiceWithOwner
    {
        public ILogger Logger => Log.Logger.ForContext<MetroDialogService>();

        public void ResetMetroWindowOwner()
        {
            MetroWindowOwner = ServiceLocator.Current.GetInstance<MetroWindow>();
        }

        public async Task<string> ChangePasswordDialog()
        {

            Logger.Debug(DialogServiceEvents.ChangePassword);
            var customDialog = new CustomDialog() { Title = lang.ChangePasswodWindowTitle };

            var viewModel = new ChangePasswordViewModel();
            viewModel.CloseAction = async () => await MetroWindowOwner.HideMetroDialogAsync(customDialog);
            customDialog.Content = new ChangePasswordDialogControl() { DataContext = viewModel };

            await MetroWindowOwner.ShowMetroDialogAsync(customDialog);
            await customDialog.WaitUntilUnloadedAsync();
            var result = viewModel.Result;
            if (result == false)
            {
                Logger.Warning(DialogServiceEvents.ChangePasswordCancel, "Change password dialog was canceled.");
                return null;
            }
            Logger.Information(DialogServiceEvents.ChangePasswordSuccess, "Password was changed.");
            return viewModel.NewPassword;
        }

        public string ShowLoginDialogOnlyPassword()
        {
            var resultData = MetroWindowOwner.ShowModalLoginExternal(lang.EnterSettingWindowTitle, lang.EnterSettingWindowTextBlock, new LoginDialogSettings
            {
                ShouldHideUsername = true,
                AffirmativeButtonText = lang.EnterSettingWindowEnterButton,
                PasswordWatermark = lang.EnterSettingWindowTextBlock
            });

            return resultData?.Password;
        }

        public string ShowFileOpenDialog(string aDescription, string aExtension)
        {
            var theDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = String.Format("{0}|{1}", aDescription, aExtension),
                DefaultExt = aExtension,
                Multiselect = false
            };

            var theResult = theDialog.ShowDialog();
            if (theResult ?? false)
            {
                if (!String.IsNullOrWhiteSpace(theDialog.FileName))
                {
                    return theDialog.FileName;
                }
            }

            return null;
        }

        public async Task ShowProgressDialog(string title, string message = null, bool isCancelable = false)
        {
            var theSettings = new MetroDialogSettings();
            theSettings.AnimateShow = true;
            theSettings.AnimateHide = true;

            mProgressController = await MetroWindowOwner.ShowProgressAsync(title, message ?? String.Empty, isCancelable, theSettings);
        }

        public bool UpdateProgress(double progress, string message = null)
        {
            if (mProgressController == null)
            {
                return false;
            }

            if (mProgressController.IsCanceled)
            {
                return false;
            }

            if (progress >= 0 && progress <= 1)
            {
                mProgressController.SetProgress(progress);
            }
            else
            {
                mProgressController.SetIndeterminate();
            }

            if (message != null)
            {
                mProgressController.SetMessage(message);
            }

            return true;
        }

        public async Task CloseProgressDialog(bool closeWindowOwner = false)
        {
            if (mProgressController == null)
            {
                return;
            }
            await mProgressController.CloseAsync();

            if (closeWindowOwner)
            {
                MetroWindowOwner.Close();
            }
        }

        public async Task ShowMessageDialog(string title, string message)
        {
            var theSettings = new MetroDialogSettings();
            theSettings.AnimateShow = false;
            theSettings.AnimateHide = true;
            await MetroWindowOwner.ShowMessageAsync(title, message, MessageDialogStyle.Affirmative, theSettings);
        }

        public void ShowMessageDialogExternal(string title, string message)
        {
            var theSettings = new MetroDialogSettings();
            theSettings.AnimateShow = false;
            theSettings.AnimateHide = true;
            MetroWindowOwner.ShowMessageAsync(title, message, MessageDialogStyle.Affirmative, theSettings);
        }


        public bool ShowOkCancelDialogExternal(string title, string message, string affirmativeButtonText = null, string negativeButtonText = null)
        {
            var theSettings = new MetroDialogSettings();
            theSettings.AnimateShow = false;
            theSettings.AnimateHide = true;
            if (affirmativeButtonText != null)
            {
                theSettings.AffirmativeButtonText = affirmativeButtonText;
            }
            if (negativeButtonText != null)
            {
                theSettings.NegativeButtonText = negativeButtonText;
            }

            var result = MetroWindowOwner.ShowModalMessageExternal(title, message, MessageDialogStyle.AffirmativeAndNegative, theSettings);
            return result == MessageDialogResult.Affirmative;
        }
        public async Task<bool> ShowOkCancelDialog(string aTitle, string aMessage)
        {
            var theResult = await MetroWindowOwner.ShowMessageAsync(aTitle, aMessage, MessageDialogStyle.AffirmativeAndNegative);
            return theResult == MessageDialogResult.Affirmative;
        }


        private ProgressDialogController mProgressController;
        public MetroWindow MetroWindowOwner { get; set; }
    }
}