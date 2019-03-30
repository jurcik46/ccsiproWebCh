using System.Globalization;

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