using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CefSharp;
using CefSharp.Enums;
using CefSharp.Wpf;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using MahApps.Metro.Controls;
using Serilog;
using WebChromiumCcsipro.Domain.Enums;
using WebChromiumCcsipro.Domain.Interfaces;
using WebChromiumCcsipro.Domain.Messages;
using WebChromiumCcsipro.UI.ViewModels;
using WebChromiumCcsipro.UI.VirtualKeyboard;

namespace WebChromiumCcsipro.UI.Views.MainWindow
{
    /// <summary>
    /// Interaction logic for MainWindowView.xaml
    /// </summary>
    /// [STAThread]
    public partial class MainWindowView : MetroWindow, IClosable, IFullScreen
    {
        private NotifiWindowView _notifiWindow;
        public ILogger Logger => Log.Logger.ForContext<MainViewModel>();

        private TouchKeyboardEventManager _touchKeyboardEventManager;


        public MainWindowView()
        {
            SimpleIoc.Default.Register<MetroWindow>(() => this);
            ViewModelLocator.MetroDialogService.ResetMetroWindowOwner();
            InitializeComponent();
            RegistrationMessage();
            DataContext = ViewModelLocator.MainViewModel;
            _notifiWindow = new NotifiWindowView();
            _notifiWindow.Show();
            trayIconTaskbar.Icon = WebChromiumCcsipro.Resources.Properties.Resources.online;
            FullScreenMode = false;
            Browser.VirtualKeyboardRequested += BrowserVirtualKeyboardRequested;
            Browser.IsBrowserInitializedChanged += BrowserIsBrowserInitializedChanged;
        }


        private void RegistrationJsFunction()
        {
            Browser.JavascriptObjectRepository.Register("cefSharpServiceAsync", ViewModelLocator.CefSharpJsService, true);
            Browser.JavascriptObjectRepository.ObjectBoundInJavascript += (sender, e) =>
            {
                var name = e.ObjectName;
                Logger.Information($"Object {e.ObjectName} was bound successfully.");
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
            if (ViewModelLocator.SettingsService.FullScreen)
                Task.Run(() => DispatcherHelper.CheckBeginInvokeOnUI(FullScreenEnable));

        }

        private void MainWindowView_OnClosed(object sender, EventArgs e)
        {
            ViewModelLocator.SocketService.Disconnect();
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
                        trayIconTaskbar.Icon = WebChromiumCcsipro.Resources.Properties.Resources.online;
                        break;
                    case TrayIconsStatus.Offline:
                        trayIconTaskbar.Icon = WebChromiumCcsipro.Resources.Properties.Resources.offline;
                        break;
                    case TrayIconsStatus.Working:
                        trayIconTaskbar.Icon = WebChromiumCcsipro.Resources.Properties.Resources.working;
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

        private void BrowserIsBrowserInitializedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                var browserHost = Browser.GetBrowserHost();

                _touchKeyboardEventManager = new TouchKeyboardEventManager(browserHost.GetWindowHandle());
            }
            else
            {
                _touchKeyboardEventManager?.Dispose();
            }
        }

        private void BrowserVirtualKeyboardRequested(object sender, VirtualKeyboardRequestedEventArgs e)
        {
            var inputPane = _touchKeyboardEventManager.GetInputPane();

            if (e.TextInputMode == TextInputMode.None)
            {
                inputPane.TryHide();
            }
            else
            {
                inputPane.TryShow();
            }
        }


        public bool FullScreenMode { get; private set; }

        public void FullScreenEnable()
        {
            NoFullScreenMenu.Visibility = Visibility.Collapsed;
            FullScreenMenu.Visibility = Visibility.Visible;
            Grid.SetRow(BrowserBroder, 0);
            Grid.SetRowSpan(BrowserBroder, 3);
            Grid.SetRow(ProgressBar, 0);
            ProgressBar.Height = 10;
            IgnoreTaskbarOnMaximize = true;
            WindowState = WindowState.Maximized;
            UseNoneWindowStyle = true;
            FullScreenMode = true; // zistit ci sa neda nahradit s 

        }

        public void FullScreenDisable()
        {
            NoFullScreenMenu.Visibility = Visibility.Visible;
            FullScreenMenu.Visibility = Visibility.Collapsed;
            Grid.SetRow(BrowserBroder, 1);
            Grid.SetRow(ProgressBar, 1);
            ProgressBar.Height = 5;
            WindowState = WindowState.Normal;
            UseNoneWindowStyle = false;
            ShowTitleBar = true;
            IgnoreTaskbarOnMaximize = false;
            FullScreenMode = false;
        }
    }


}
