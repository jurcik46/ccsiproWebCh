﻿using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CefSharp;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using Serilog;
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
    public partial class MainWindowView : Window, IClosable, IFullScreen
    {
        private NotifiWindowView notifiWindow;
        public ILogger Logger => Log.Logger.ForContext<MainViewModel>();



        public MainWindowView()
        {

            InitializeComponent();
            RegistrationMessage();
            DataContext = ViewModelLocator.MainViewModel;

            notifiWindow = new NotifiWindowView();
            notifiWindow.Show();
            trayIconTaskbar.Icon = WebChromiumCcsipro.Resources.Properties.Resources.online;
            FullScreenMode = false;
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


        public bool FullScreenMode { get; private set; }

        public void FullScreenEnable()
        {
            NoFullScreenMenu.Visibility = Visibility.Collapsed;
            FullScreenMenu.Visibility = Visibility.Visible;
            Grid.SetRow(BrowserBroder, 0);
            Grid.SetRowSpan(BrowserBroder, 3);
            Grid.SetRow(ProgressBar, 0);
            ProgressBar.Height = 10;
            WindowStyle = WindowStyle.None;
            WindowState = WindowState.Maximized;
            ResizeMode = ResizeMode.NoResize;


            FullScreenMode = true;
            //this.Width = System.Windows.SystemParameters.WorkArea.Width;
            //this.Height = System.Windows.SystemParameters.WorkArea.Height;
            //this.Left = 0;
            //this.Top = 0;
            //this.WindowState = WindowState.Normal;
        }

        public void FullScreenDisable()
        {
            NoFullScreenMenu.Visibility = Visibility.Visible;
            FullScreenMenu.Visibility = Visibility.Collapsed;
            Grid.SetRow(BrowserBroder, 1);
            Grid.SetRow(ProgressBar, 1);
            ProgressBar.Height = 5;
            WindowStyle = WindowStyle.SingleBorderWindow;
            this.WindowState = WindowState.Normal;
            //WindowState = WindowState.Maximized;
            ResizeMode = ResizeMode.CanResize;
            FullScreenMode = false;
        }
    }


}
