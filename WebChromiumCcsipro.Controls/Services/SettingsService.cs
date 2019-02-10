using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Destructurama.Attributed;
using Serilog;
using WebChromiumCcsipro.Controls.Interfaces.IServices;

namespace WebChromiumCcsipro.Controls.Services
{
    [LogAsScalar]
    public class SettingsService : ISettingsService
    {
        public ILogger Logger => Log.Logger.ForContext<SettingsService>();

        public SettingsService()
        {
            LoadSetting();
        }

        public void LoadSetting()
        {

        }

        public void SaveSetting()
        {

        }



    }
}
