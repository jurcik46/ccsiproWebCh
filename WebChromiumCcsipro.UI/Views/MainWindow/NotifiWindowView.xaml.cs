using System;
using System.Collections.Generic;
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
using GalaSoft.MvvmLight.Messaging;
using Notifications.Wpf;
using WebChromiumCcsipro.Domain.Messages;

namespace WebChromiumCcsipro.UI.Views.MainWindow
{
    /// <summary>
    /// Interaction logic for NotifiWindowView.xaml
    /// </summary>
    public partial class NotifiWindowView : Window
    {
        private readonly NotificationManager _notificationManager = new NotificationManager();

        private NotificationContent content;

        public NotifiWindowView()
        {
            InitializeComponent();

            content = new NotificationContent();

            Messenger.Default.Register<NotifyMessage>(this, (message) =>
            {
                content.Title = message.Title;
                content.Message = message.Msg;
                content.Type = message.IconType;

                _notificationManager.Show(content, expirationTime: System.TimeSpan.FromSeconds(message.ExpTime), areaName: "WindowArea");
            });
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            Width = desktopWorkingArea.Width;
            Height = desktopWorkingArea.Height;
            Left = desktopWorkingArea.Right - Width;
            Top = desktopWorkingArea.Bottom - Height;
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            Window window = (Window)sender;
            window.Topmost = true;
        }
    }
}
