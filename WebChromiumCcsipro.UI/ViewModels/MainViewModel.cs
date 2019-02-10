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
using WebChromiumCcsipro.Controls.Interfaces;
using WebChromiumCcsipro.Resources;
using Path = System.IO.Path;

namespace WebChromiumCcsipro.UI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private string _urlAddress;

        public RelayCommand SignatureCommand { get; set; }
        public RelayCommand SettingsCommand { get; set; }
        public RelayCommand<IClosable> RestartCommand { get; set; }
        public RelayCommand<IClosable> ExitCommand { get; set; }

        private string _toolTipText;

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

        public MainViewModel()
        {
            CommandInit();
            UrlAddress =
                @"https://stackoverflow.com/questions/6925584/the-name-initializecomponent-does-not-exist-in-the-current-context";
            ToolTipText = Resources.Language.lang.TrayIconToolTipDefault;
        }


        #region Commands
        private void CommandInit()
        {

            //this.Options = new RelayCommand(this.ShowOptionsLogin, this.CanShowOptionsLogin);
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
        #endregion



    }
}
