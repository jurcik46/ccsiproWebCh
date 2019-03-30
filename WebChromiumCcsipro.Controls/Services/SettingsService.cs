using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Destructurama.Attributed;
using GalaSoft.MvvmLight.Messaging;
using Serilog;
using WebChromiumCcsipro.Resources.Enums;
using WebChromiumCcsipro.Resources.Settings;
using WebChromiumCcsipro.Resources.Extensions;
using WebChromiumCcsipro.Resources.Interfaces.IServices;
using WebChromiumCcsipro.Resources.Messages;

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
        private CultureInfo _culture;
        private bool _langChange;
        public CultureInfo Culture
        {
            get { return _culture; }
            set
            {
                _culture = value;

                if (_langChange)
                {
                    Messenger.Default.Send(new ChangeLanguageMessage(this, Culture));
                    _langChange = false;
                }
            }
        }
        private string _language;
        public string Language
        {
            get { return _language; }
            set
            {
                _language = value;
                Culture = new CultureInfo(value);
            }
        }

        public string PasswordSalt { get; set; }
        public string PasswordSetting { get; set; }
        #endregion

        public SettingsService()
        {
            Logger.Information(SettingsServiceEvents.CreateInstance);
            LoadAllSetting();
            if (PasswordSetting == "")
            {
                Logger.Information(SettingsServiceEvents.CreateDefaultPass);
                CreatePassword("admin");
            }
        }

        public void CreatePassword(string password)
        {
            Logger.Information(SettingsServiceEvents.CreatePassword);
            PasswordSalt = CryptoExtension.GenerateSalt();
            PasswordSetting = CryptoExtension.HashPassword(password, PasswordSalt);
            CCSIproChromiumSetting.Default.PasswordSetting = PasswordSetting;
            CCSIproChromiumSetting.Default.PasswordSalt = PasswordSalt;
            CCSIproChromiumSetting.Default.Save();
        }

        public void LoadAllSetting()
        {
            Logger.Information(SettingsServiceEvents.LoadingAllSetting);
            ChromiumSettingLoad();
            SignatureSettingLoad();
        }

        public void SignatureSettingLoad()
        {
            Logger.Information(SettingsServiceEvents.SignatureSettingLoading);
            ApiLink = SignatureSetting.Default.ApiLink;
            ApiKey = SignatureSetting.Default.ApiKey;
            ProgramPath = SignatureSetting.Default.ProgramPath;
            ProcessName = SignatureSetting.Default.ProcessName;
            SignatureTimeOut = SignatureSetting.Default.singTimeOut;
        }

        private void ChromiumSettingLoad()
        {
            Logger.Information(SettingsServiceEvents.ChromiumSettingLoading);
            ObjectId = CCSIproChromiumSetting.Default.ObjecID;
            UserId = CCSIproChromiumSetting.Default.UserID;
            HomePage = CCSIproChromiumSetting.Default.HomePage;
            Language = CCSIproChromiumSetting.Default.Language;
            PasswordSetting = CCSIproChromiumSetting.Default.PasswordSetting;
            PasswordSalt = CCSIproChromiumSetting.Default.PasswordSalt;

        }

        public void SaveSetting()
        {

        }

        public void SignatureSettingSave(string apiLink, string apiKey, string programPath, string processName, int signatureTimeOut = 100)
        {
            Logger.Information(SettingsServiceEvents.SignatureSettingSave);
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
            Logger.Information(SettingsServiceEvents.ChromiumSettingSave);
            _langChange = CCSIproChromiumSetting.Default.Language != language;
            CCSIproChromiumSetting.Default.ObjecID = objectId;
            CCSIproChromiumSetting.Default.UserID = userId;
            CCSIproChromiumSetting.Default.HomePage = homePage;
            CCSIproChromiumSetting.Default.Language = language;
            CCSIproChromiumSetting.Default.PasswordSalt = PasswordSalt;
            CCSIproChromiumSetting.Default.PasswordSetting = PasswordSetting;
            CCSIproChromiumSetting.Default.Save();
            ChromiumSettingLoad();

        }




    }
}
