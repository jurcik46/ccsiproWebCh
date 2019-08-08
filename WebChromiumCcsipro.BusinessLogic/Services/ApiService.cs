using System;
using Serilog;
using WebChromiumCcsipro.API;
using WebChromiumCcsipro.BusinessLogic.Models;
using WebChromiumCcsipro.Domain.Enums;
using WebChromiumCcsipro.Domain.Extensions;
using WebChromiumCcsipro.Domain.Interfaces;
using WebChromiumCcsipro.Domain.Interfaces.IServices;

namespace WebChromiumCcsipro.BusinessLogic.Services
{
    public class ApiService : IApiService
    {
        private Api _api;
        public ILogger Logger => Log.Logger.ForContext<ApiService>();
        private ISettingsService SettingsService { get; set; }
        private Api Api { get; set; }
        public ApiService(ISettingsService settingsService)
        {
            Logger.Information(ApiServiceEvents.CreateInstance, "Creating new instance of ApiService");
            SettingsService = settingsService;
            Api = new Api(SettingsService.ApiLink, SettingsService.ApiKey);
        }

        #region Get document to sign
        public ISignatureFileModel GetDocumentToSignature()
        {
            Logger.Debug(ApiServiceEvents.GetDocumentToSignature);
            using (Logger.BeginTimedOperation(ApiServiceEvents.GetDocumentToSignature))
            {
                try
                {
                    var document = Api.GetDocument(SettingsService.ObjectId, SettingsService.UserId);
                    if (document != null)
                    {
                        if (document.Status == 200)
                        {
                            ISignatureFileModel fileModel = new SignatureFileModel(document);
                            return fileModel;
                        }
                    }
                    Logger.Warning(ApiServiceEvents.GetDocumentToSignatureNotFound);
                    return null;
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, ApiServiceEvents.GetDocumentToSignatureError);
                    _api = null;
                    return null;
                }
            }

        }
        #endregion

        #region Upload signed document
        public bool UploadSignedDocument(string hash, string pdfFilePath, string file)
        {
            Logger.Debug(ApiServiceEvents.UploadSignedDocument);
            using (Logger.BeginTimedOperation(ApiServiceEvents.UploadSignedDocument))
            {
                try
                {
                    var status = Api.UploadDocument(SettingsService.ObjectId, SettingsService.UserId, hash, pdfFilePath, file);
                    if (status != null)
                    {
                        if (status.Status == 200)
                            return true;
                    }
                    Logger.Warning(ApiServiceEvents.UploadSignedDocumentFailed);
                    return false;
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, ApiServiceEvents.UploadSignedDocumentError);
                    _api = null;
                    return false;
                }
            }
        }
        #endregion
    }
}