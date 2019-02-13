﻿using System;
using System.Threading;
using Serilog;
using WebChromiumCcsipro.Resources.Interfaces.IServices;
using WebChromiumCcsipro.API;
using WebChromiumCcsipro.Controls.Models;
using WebChromiumCcsipro.Resources.Enums;
using WebChromiumCcsipro.Resources.Extensions;
using WebChromiumCcsipro.Resources.Interfaces;

namespace WebChromiumCcsipro.Controls.Services
{
    public class ApiService : IApiService
    {
        private Api _api;
        public ILogger Logger => Log.Logger.ForContext<ApiService>();
        private ISettingsService SettingsService { get; set; }
        internal Api Api => _api ?? (_api = new Api(SettingsService));

        public ApiService(ISettingsService settingsService)
        {
            SettingsService = settingsService;
        }

        #region Get document to sign
        public ISignatureFileModel GetDocumentToSignature()
        {
            Logger.Debug(ApiServiceEvents.GetDocumentToSignature);
            using (Logger.BeginTimedOperation(ApiServiceEvents.GetDocumentToSignature))
            {
                try
                {
                    var document = Api.GetDocument();
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
                    var status = Api.UploadDocument(hash, pdfFilePath, file);
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