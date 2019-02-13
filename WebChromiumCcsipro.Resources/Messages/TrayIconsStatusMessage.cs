using WebChromiumCcsipro.Resources.Enums;

namespace WebChromiumCcsipro.Resources.Messages
{
    public class TrayIconsStatusMessage
    {
        private TrayIconsStatus _iconStatus;

        public TrayIconsStatus IconStatus { get => _iconStatus; set => _iconStatus = value; }
    }
}