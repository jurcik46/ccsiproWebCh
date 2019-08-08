﻿using System;
using System.Globalization;
using System.Reflection;
using GalaSoft.MvvmLight.Messaging;
using Serilog;
using WebChromiumCcsipro.Domain.Enums;
using WebChromiumCcsipro.Domain.Extensions;
using WebChromiumCcsipro.Domain.Interfaces.IServices;
using WebChromiumCcsipro.Domain.Messages;
using WebChromiumCcsipro.Resources.Settings;

namespace WebChromiumCcsipro.BusinessLogic.Services
{

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

        public int ObjectId { get; set; }
        public int UserId { get; set; }
        public string HomePage { get; set; }
        public string ReloadTime { get; set; }
        public bool FullScreen { get; set; }
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

        #region Server setting
        public string ServerIp { get; set; }
        public int ServerPort { get; set; }
        public string KioskIp { get; set; }
        public int KioskPort { get; set; }
        #endregion


        public SettingsService()
        {
            Logger.Information(SettingsServiceEvents.CreateInstance, "Creating new instance of SettingsService");
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
            ServerSettingLoad();
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
            ReloadTime = CCSIproChromiumSetting.Default.ReloadTime;
            Language = CCSIproChromiumSetting.Default.Language;
            PasswordSetting = CCSIproChromiumSetting.Default.PasswordSetting;
            PasswordSalt = CCSIproChromiumSetting.Default.PasswordSalt;
        }

        private void ServerSettingLoad()
        {
            Logger.Information(SettingsServiceEvents.ServerSettingLoad);
            ServerIp = CCSIproChromiumSetting.Default.ServerIp;
            ServerPort = CCSIproChromiumSetting.Default.ServerPort;
            KioskIp = CCSIproChromiumSetting.Default.KioskIp;
            KioskPort = CCSIproChromiumSetting.Default.KioskPort;
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

        public void ChromiumSettingSave(int objectId, int userId, string homePage, string reloadTime, string language)
        {
            Logger.Information(SettingsServiceEvents.ChromiumSettingSave);
            _langChange = CCSIproChromiumSetting.Default.Language != language;
            CCSIproChromiumSetting.Default.ObjecID = objectId;
            CCSIproChromiumSetting.Default.UserID = userId;
            CCSIproChromiumSetting.Default.HomePage = homePage;
            CCSIproChromiumSetting.Default.ReloadTime = reloadTime;
            CCSIproChromiumSetting.Default.Language = language;
            CCSIproChromiumSetting.Default.PasswordSalt = PasswordSalt;
            CCSIproChromiumSetting.Default.PasswordSetting = PasswordSetting;
            CCSIproChromiumSetting.Default.Save();
            ChromiumSettingLoad();
        }

        public void ServerSettingSave(string serverIp, int serverPort, string kioskIp, int kioskPort)
        {
            Logger.Information(SettingsServiceEvents.ServerSettingSave);
            CCSIproChromiumSetting.Default.ServerIp = serverIp;
            CCSIproChromiumSetting.Default.ServerPort = serverPort;
            CCSIproChromiumSetting.Default.KioskIp = kioskIp;
            CCSIproChromiumSetting.Default.KioskPort = kioskPort;
            CCSIproChromiumSetting.Default.Save();
            ServerSettingLoad();

        }
    }
}
