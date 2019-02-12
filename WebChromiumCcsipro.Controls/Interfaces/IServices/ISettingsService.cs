using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebChromiumCcsipro.Controls.Interfaces.IServices
{
    public interface ISettingsService
    {
        #region Signature setting
        string ApiLink { get; set; }
        string ApiKey { get; set; }
        string ProgramPath { get; set; }
        string ProcessName { get; set; }
        int SignatureTimeOut { get; set; }
        #endregion

        void LoadAllSetting();
        void SaveSetting();
        void SignatureSettingLoad();

        void SignatureSettingSave(string apiLink, string apiKey, string programPath, string processName,
            int signatureTimeOut = 100);
    }
}
