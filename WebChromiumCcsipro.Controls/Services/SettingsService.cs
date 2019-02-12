using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Destructurama.Attributed;
using Serilog;
using WebChromiumCcsipro.Resources.Settings;
using WebChromiumCcsipro.Controls.Interfaces.IServices;

namespace WebChromiumCcsipro.Controls.Services
{
    [LogAsScalar]
    public class SettingsService : ISettingsService
    {
        private ILogger Logger => Log.Logger.ForContext<SettingsService>();

        #region Signature setting
        public string ApiLink { get; set; }
        public string ApiKey { get; set; }
        public string ProgramPath { get; set; }
        public string ProcessName { get; set; }
        public int SignatureTimeOut { get; set; }
        #endregion

        public SettingsService()
        {
            LoadAllSetting();
        }

        public void LoadAllSetting()
        {
            ChromiumSettingLoad();
            SignatureSettingLoad();
        }

        public void SignatureSettingLoad()
        {
            ApiLink = SignatureSetting.Default.ApiLink;
            ApiKey = SignatureSetting.Default.ApiKey;
            ProgramPath = SignatureSetting.Default.ProgramPath;
            ProcessName = SignatureSetting.Default.ProcessName;
            SignatureTimeOut = SignatureSetting.Default.singTimeOut;
        }

        private void ChromiumSettingLoad()
        {

        }

        public void SaveSetting()
        {

        }

        public void SignatureSettingSave(string apiLink, string apiKey, string programPath, string processName, int signatureTimeOut = 100)
        {
            SignatureSetting.Default.ApiLink = apiLink;
            SignatureSetting.Default.ApiKey = apiKey;
            SignatureSetting.Default.ProgramPath = programPath;
            SignatureSetting.Default.ProcessName = processName;
            SignatureSetting.Default.singTimeOut = signatureTimeOut;
            SignatureSetting.Default.Save();
        }




    }
}
