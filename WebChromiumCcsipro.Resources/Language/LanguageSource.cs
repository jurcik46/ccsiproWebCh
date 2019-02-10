using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WebChromiumCcsipro.Resources.Language
{
    public class LanguageSource
    {

        static public Dictionary<string, string> GetValues()
        {
            var languages = new Dictionary<string, string>() { { "en-US", "English" }, { "cs-CZ", "Čeština" }, { "sk-SK", "Slovenčina" } };
            return languages;
        }
    }
}
