using Notifications.Wpf;
using WebChromiumCcsipro.Resources.Settings;

namespace WebChromiumCcsipro.Resources.Messages
{
    public class NotifiMessage
    {
        private string _appName = CCSIproChromiumSetting.Default.CompanyName + " - ";
        private string _title;
        private string _msg;
        private NotificationType _iconType;
        private int _expTime = 5;

        public string Title { get => _title; set => _title = AppName + value; }
        public string Msg { get => _msg; set => _msg = value; }
        public NotificationType IconType { get => _iconType; set => _iconType = value; }
        public int ExpTime { get => _expTime; set => _expTime = value; }
        public string AppName { get => _appName; set => _appName = value; }
    }
}
