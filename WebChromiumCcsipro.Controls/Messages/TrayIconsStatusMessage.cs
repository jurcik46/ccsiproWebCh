using WebChromiumCcsipro.Controls.Enums;

namespace WebChromiumCcsipro.Controls.Messages
{
    public class TrayIconsStatusMessage
    {
        private TrayIconsStatus _iconStatus;

        public TrayIconsStatus IconStatus { get => _iconStatus; set => _iconStatus = value; }
    }
}