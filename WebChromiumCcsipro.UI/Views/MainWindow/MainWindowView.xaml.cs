using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows;
using CefSharp;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using WebChromiumCcsipro.Domain.Enums;
using WebChromiumCcsipro.Domain.Interfaces;
using WebChromiumCcsipro.Domain.Messages;
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
            DataContext = ViewModelLocator.MainViewModel;

            notifiWindow = new NotifiWindowView();
            notifiWindow.Show();
            trayIconTaskbar.Icon = new Icon(@"Images/Icons/online.ico");
        }


        private void RegistrationJsFunction()
        {
            Browser.JavascriptObjectRepository.Register("cefSharpServiceAsync", ViewModelLocator.CefSharpJsService, true);
            Browser.JavascriptObjectRepository.ObjectBoundInJavascript += (sender, e) =>
            {
                var name = e.ObjectName;
                Debug.WriteLine($"Object {e.ObjectName} was bound successfully.");
            };
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
            RegistrationJsFunction();
        }

        private void MainWindowView_OnClosed(object sender, EventArgs e)
        {
            Cef.Shutdown();
            trayIconTaskbar.Visibility = Visibility.Hidden;
            trayIconTaskbar.Icon = null;
            Application.Current.Shutdown();
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
                }
            });
            Messenger.Default.Register<ExecuteJavaScriptMessage>(this, (message) =>
            {
                DispatcherHelper.RunAsync(() =>
                {
                    if (message.Parameters == null)
                    {
                        Browser.ExecuteScriptAsyncWhenPageLoaded($"window['{message.Function}']()");
                    }
                    else
                    {
                        var last = message.Parameters.Last();
                        string js = $"window['{message.Function}'](";
                        foreach (var param in message.Parameters)
                        {
                            js += "'" + param + "'";
                            if (!param.Equals(last))
                            {
                                js += ",";
                            }
                        }
                        js += ")";
                        Browser.ExecuteScriptAsyncWhenPageLoaded(js);
                    }
                });
            });
        }
        #endregion


    }


}
