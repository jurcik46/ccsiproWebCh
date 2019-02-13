using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using CefSharp;
using GalaSoft.MvvmLight.Messaging;
using WebChromiumCcsipro.Resources.Enums;
using WebChromiumCcsipro.Resources.Interfaces;
using WebChromiumCcsipro.Resources.Messages;
using WebChromiumCcsipro.UI.ViewModels;

namespace WebChromiumCcsipro.UI.Views.MainWindow
{
    /// <summary>
    /// Interaction logic for MainWindowView.xaml
    /// </summary>
    /// [STAThread]
    public partial class MainWindowView : Window, IClosable
    {
        private NotifiWindowView notifiWindow;

        public MainWindowView()
        {
            InitializeComponent();
            RegistrationMessage();
            this.DataContext = ViewModelLocator.MainViewModel;

            notifiWindow = new NotifiWindowView();
            notifiWindow.Show();
            this.trayIconTaskbar.Icon = new Icon(@"Images/Icons/online.ico");
            var viewModel = DataContext as MainViewModel;
            if (viewModel != null)
            {
                //                viewModel.ToolTipText = "asdasda";
                return;
            }

        }

        private void MainWindowView_OnLoaded(object sender, RoutedEventArgs e)
        {
            var splashScreen = ViewModelLocator.SplashScreen;
            if (splashScreen == null)
            {
                return;
            }
            splashScreen.Close(TimeSpan.FromMilliseconds(200));
            ViewModelLocator.SplashScreen = null;
        }

        private void MainWindowView_OnClosed(object sender, EventArgs e)
        {
            Cef.Shutdown();
            trayIconTaskbar.Visibility = Visibility.Hidden;
            trayIconTaskbar.Icon = null;

        }

        #region Message Registration
        private void RegistrationMessage()
        {
            Messenger.Default.Register<TrayIconsStatusMessage>(this, (message) =>
            {
                switch (message.IconStatus)
                {
                    case TrayIconsStatus.Online:
                        trayIconTaskbar.Icon = new Icon(@"Images/Icons/online.ico");
                        break;
                    case TrayIconsStatus.Offline:
                        trayIconTaskbar.Icon = new Icon(@"Images/Icons/offline.ico");
                        break;
                    case TrayIconsStatus.Working:
                        trayIconTaskbar.Icon = new Icon(@"Images/Icons/working.ico");
                        break;
                    default:
                        break;
                }
            });
        }
        #endregion


    }
}
