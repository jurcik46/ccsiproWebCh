using System;
using System.Threading.Tasks;

namespace WebChromiumCcsipro.Domain.Interfaces.IServices
{
    public interface IDialogServiceWithOwner : GalaSoft.MvvmLight.Views.IDialogService
    {
        bool HideAllErrors { get; set; }
        Window Owner { get; set; }
        string ChangePassword();
        string EnterSetting();
        Task ShowError(string message, string title, string buttonText, Action afterHideCallback, bool alwaysShow);
        Task<bool> ShowMessage(string message, string title, string buttonConfirmText, string buttonCancelText, Action<bool> afterHideCallback, bool defaultCancel);
        Task ShowWarning(string message, string title);


    }
}
