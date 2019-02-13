using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Destructurama.Attributed;
using Serilog;
using WebChromiumCcsipro.Resources.Settings;
using WebChromiumCcsipro.Resources.Extensions;
using WebChromiumCcsipro.Resources.Interfaces.IServices;

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

        #region Chromium setting
        public string ObjectId { get; set; }
        public string UserId { get; set; }
        public string HomePage { get; set; }
        public string Language { get; set; }
        public StringCollection AllowedUrl { get; set; }
        public string PasswordSalt { get; set; }
        public string PasswordSetting { get; set; }
        #endregion

        public SettingsService()
        {

            LoadAllSetting();
            if (PasswordSetting == "")
            {
                //                Logger.Information(PasswordServiceEvents.CreateDefaultPass);
                CreatePassword("admin");
            }
        }

        public void CreatePassword(string password)
        {
            PasswordSalt = CryptoExtension.GenerateSalt();
            PasswordSetting = CryptoExtension.HashPassword(password, PasswordSalt);
            CCSIproChromiumSetting.Default.PasswordSetting = PasswordSetting;
            CCSIproChromiumSetting.Default.PasswordSalt = PasswordSalt;
            CCSIproChromiumSetting.Default.Save();
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
            ObjectId = CCSIproChromiumSetting.Default.ObjecID;
            UserId = CCSIproChromiumSetting.Default.UserID;
            HomePage = CCSIproChromiumSetting.Default.HomePage;
            Language = CCSIproChromiumSetting.Default.Language;
            AllowedUrl = CCSIproChromiumSetting.Default.AllowedUrl;
            PasswordSetting = CCSIproChromiumSetting.Default.PasswordSetting;
            PasswordSalt = CCSIproChromiumSetting.Default.PasswordSalt;
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
            SignatureSettingLoad();
        }

        public void ChromiumSettingSave(string objectId, string userId, string homePage, string language)
        {
            CCSIproChromiumSetting.Default.ObjecID = objectId;
            CCSIproChromiumSetting.Default.UserID = userId;
            CCSIproChromiumSetting.Default.HomePage = homePage;
            CCSIproChromiumSetting.Default.Language = language;
            CCSIproChromiumSetting.Default.AllowedUrl = AllowedUrl;
            CCSIproChromiumSetting.Default.PasswordSalt = PasswordSalt;
            CCSIproChromiumSetting.Default.PasswordSetting = PasswordSetting;
            CCSIproChromiumSetting.Default.Save();
            ChromiumSettingLoad();
        }




    }
}
