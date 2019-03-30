using System.Globalization;
using GalaSoft.MvvmLight.Messaging;

namespace WebChromiumCcsipro.Domain.Messages
{
    public class ChangeLanguageMessage : GenericMessage<CultureInfo>
    {
        public ChangeLanguageMessage(object sender, CultureInfo content)
            : base(sender, content)
        {
        }
    }
}