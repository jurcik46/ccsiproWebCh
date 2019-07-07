using System.Globalization;

namespace WebChromiumCcsipro.Domain.Interfaces.IServices
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

        #region Chromium setting
        string ObjectId { get; set; }
        string UserId { get; set; }
        string HomePage { get; set; }
        CultureInfo Culture { get; set; }
        string Language { get; set; }
        string PasswordSalt { get; set; }
        string PasswordSetting { get; set; }
        #endregion

        #region Server setting
        string ServerIp { get; set; }
        int ServerPort { get; set; }
        string KioskIp { get; set; }
        int KioskPort { get; set; }
        #endregion
        void CreatePassword(string password);
        void LoadAllSetting();
        void SaveSetting();
        void SignatureSettingLoad();
        void ServerSettingSave(string serverIp, int serverPort, string kioskIp, int kioskPort);
        void SignatureSettingSave(string apiLink, string apiKey, string programPath, string processName,
            int signatureTimeOut = 100);
        void ChromiumSettingSave(string objectId, string userId, string homePage, string language);
    }
}
