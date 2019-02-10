using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Serilog;
using WebChromiumCcsipro.Controls.Interfaces;
using WebChromiumCcsipro.Controls.Interfaces.IServices;
using WebChromiumCcsipro.Resources;
using WebChromiumCcsipro.UI.Views.SettingsWindow;
using Path = System.IO.Path;

namespace WebChromiumCcsipro.UI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public ILogger Logger => Log.Logger.ForContext<MainViewModel>();

        private string _urlAddress;
        private string _toolTipText;

        public RelayCommand SignatureCommand { get; set; }
        public RelayCommand SettingsCommand { get; set; }
        public RelayCommand<IClosable> RestartCommand { get; set; }
        public RelayCommand<IClosable> ExitCommand { get; set; }

        private Window SettingWindow { get; set; }

        private ISettingsService SettingService;

        public string ToolTipText
        {
            get { return _toolTipText; }
            set
            {
                _toolTipText = value;
                RaisePropertyChanged();
            }
        }

        public string UrlAddress
        {
            get { return _urlAddress; }
            set { _urlAddress = value; }
        }

        public MainViewModel(ISettingsService settingService)
        {
            SettingService = settingService;
            CommandInit();
            UrlAddress =
                @"https://stackoverflow.com/questions/6925584/the-name-initializecomponent-does-not-exist-in-the-current-context";
            ToolTipText = Resources.Language.lang.TrayIconToolTipDefault;
        }


        #region Commands

        private void CommandInit()
        {

            //this.Options = new RelayCommand(this.ShowOptionsLogin, this.CanShowOptionsLogin);
            SettingsCommand = new RelayCommand(OpenSetting, CanOpenSetting);
            RestartCommand = new RelayCommand<IClosable>(RestartApplication, CanRestartRestartApplication);
            ExitCommand = new RelayCommand<IClosable>(ExitApplication, CanExitApplication);

        }

        private bool CanExitApplication(IClosable win)
        {
            return true;
        }


        private void ExitApplication(IClosable win)
        {
            if (win != null)
            {
                win.Close();

            }

            Application.Current.Shutdown();
        }

        private bool CanRestartRestartApplication(IClosable win)
        {
            return true;
        }

        private void RestartApplication(IClosable win)
        {
            if (win != null)
            {
                win.Close();
            }

            //Application.Current.Shutdown();
            //System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
        }

        private bool CanOpenSetting()
        {
            if (SettingWindow != null)
                return (SettingWindow.IsLoaded) ? false : true;
            else
                return true;
        }

        private void OpenSetting()
        {
            SettingWindow = new SettingWindowView();
            var settingViewModel = new SettingViewModel.SettingViewModel(SettingService);
            SettingWindow.DataContext = settingViewModel;
            SettingWindow.Show();

        }
        #endregion



    }
}
