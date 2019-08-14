using System.Threading.Tasks;
using MahApps.Metro.Controls;

namespace WebChromiumCcsipro.Domain.Interfaces.IServices
{
    public interface IMetroDialogServiceWithOwner
    {
        void ResetMetroWindowOwner();
        MetroWindow MetroWindowOwner { get; set; }
        Task<string> ChangePasswordDialog();
        string ShowFileOpenDialog(string aDescription, string aExtension);

        Task ShowProgressDialog(string title, string message = null, bool isCancelable = false);

        /// <summary>
        /// Updates progress for current Progress dialog
        /// </summary>
        /// <param name="aProgress">Progress from 0 to 1, if outside of this range, progress dialog will show indeterminate progress</param>
        /// <param name="aMessage">Progress message</param>
        /// <returns>false if Cancel was pressed</returns>
        bool UpdateProgress(double progress, string message = null);
        Task CloseProgressDialog(bool closeWindowOwner = false);

        Task ShowMessageDialog(string title, string message);
        void ShowMessageDialogExternal(string title, string message);
        string ShowLoginDialogOnlyPassword();
        bool ShowOkCancelDialogExternal(string aTitle, string aMessage, string affirmativeButtonText = null,
            string negativeButtonText = null);
        Task<bool> ShowOkCancelDialog(string aTitle, string aMessage);
    }
}