using System;
using System.Net;
using GalaSoft.MvvmLight.Messaging;
using RestSharp;
using Serilog;
using WebChromiumCcsipro.API.Enums;
using WebChromiumCcsipro.API.Models;
using WebChromiumCcsipro.Domain.Extensions;
using WebChromiumCcsipro.Domain.Interfaces.IServices;
using WebChromiumCcsipro.Domain.Messages;
using WebChromiumCcsipro.Resources.Language;
using LoggerExtensions = WebChromiumCcsipro.Domain.Extensions.LoggerExtensions;


namespace WebChromiumCcsipro.API
{
    public class Api
    {
        private ILogger Logger => Log.Logger.ForContext<Api>();

        public Uri ApiLink { get; set; }
        private ISettingsService SettingsService { get; set; }

        public Api(ISettingsService settingsService)
        {
            SettingsService = settingsService;
            Logger.Debug(ApiEvents.Create, "Creating new instance of API with {ApiLink} and {Apikey}", SettingsService.ApiLink, SettingsService.ApiKey);
            ApiLink = new Uri(SettingsService.ApiLink, UriKind.Absolute);
        }

        public T Execute<T>(RestRequest request) where T : class, new()
        {
            LoggerExtensions.Debug(Logger, ApiEvents.ExecuteType, "API.Execute<{T}>({@request})", typeof(T).FullName, request);
            var client = new RestClient { BaseUrl = ApiLink };
            request.AddParameter("api_key", SettingsService.ApiKey, ParameterType.QueryString);
            var response = client.Execute<T>(request);
            if (response.ErrorException != null)
            {
                if (response.StatusCode == 0)
                    Messenger.Default.Send(new NotifyMessage() { Title = lang.ApiNotificationExecuteConnectionTitle, Msg = lang.ApiNotificationExecuteConnectionInfo, IconType = Notifications.Wpf.NotificationType.Error, ExpTime = 10 });

                LoggerExtensions.Error(LoggerExtensions.With(LoggerExtensions.With(Logger.With("Request", request), "Response", response), "Type", typeof(T).FullName), response.ErrorException, ApiEvents.ExecuteTypeError);
                return null;
            }

            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created)
            {
                LoggerExtensions.Error(LoggerExtensions.With(LoggerExtensions.With(Logger.With("Request", request), "Response", response), "Type", typeof(T).FullName), ApiEvents.ExecuteTypeError, "Request on {Resource} returned {StatusCode}.\nResponse content: {Content}", request.Resource, response.StatusCode, response.Content);
                return null;

            }
            else
            {
                LoggerExtensions.Debug(LoggerExtensions.With(LoggerExtensions.With(Logger.With("Request", request), "Response", response), "Type", typeof(T).FullName), ApiEvents.ExecuteTypeSuccess, "Request on {Resource} returned {StatusCode}.\nResponse content: {Content}", request.Resource, response.StatusCode, response.Content);
            }
            return response.Data;
        }

        public bool Execute(RestRequest request, HttpStatusCode expectedCode)
        {
            Logger.Debug(ApiEvents.ExecuteBool, "EntranceAPI.Execute({@request}, {expectedCode})", request, expectedCode);
            var client = new RestClient { BaseUrl = ApiLink };
            var parameters = request.Parameters.Select(parameter => new
            {
                name = parameter.Name,
                value = parameter.Value,
                type = parameter.Type.ToString()
            });

            var response = client.Execute(request);
            if (response.ErrorException != null)
            {
                Logger.With("Request", request)
                    .With("Response", response)
                    .With("HttpStatusCodeExpected", expectedCode)
                    .Error(response.ErrorException, ApiEvents.ExecuteBoolError);
                return false;
            }
            var result = response.StatusCode == expectedCode;
            if (!result)
            {
                Logger.With("Request", request)
                    .With("Response", response)
                    .With("HttpStatusCodeExpected", expectedCode)
                    .Error(ApiEvents.ExecuteBoolError, "Request on {Resource} returned {StatusCode}.\nResponse content: {Content}", request.Resource, response.StatusCode, response.Content);
            }
            else
            {
                Logger.With("Request", request)
                    .With("Response", response)
                    .With("HttpStatusCodeExpected", expectedCode)
                    .Debug(ApiEvents.ExecuteBoolSuccess, "Request on {Resource} returned {StatusCode}.\nResponse content: {Content}", request.Resource, response.StatusCode, response.Content);
            }
            return result;
        }

        public void OneEmptyRequest()
        {
            var request = new RestRequest
            {
                Resource = @"",
                Method = Method.POST,
                RequestFormat = DataFormat.Json
            };

            Execute(request, HttpStatusCode.OK);
        }

        public SignatureFileModel GetDocument()
        {
            Logger.Debug(ApiEvents.GetDocument, "Object-id: {ObjectID}, User-id: {UserID}", SettingsService.ObjectId, SettingsService.UserId);

            var request = new RestRequest
            {
                Resource = @"/getinfo.json",
                Method = Method.POST,
                RequestFormat = DataFormat.Json
            };

            request.AddParameter("object_id", SettingsService.ObjectId, ParameterType.GetOrPost);
            request.AddParameter("user_id", SettingsService.UserId, ParameterType.GetOrPost);

            var result = Execute<SignatureFileModel>(request);
            return result;
        }

        public UploadDocumentModel UploadDocument(string hash, string pdfFilePath, string file)
        {
            Logger.Debug(ApiEvents.UploadDocument, "Object-id: {ObjectID}, User-id: {UserID}, Hash: {hash}, PdfFilePath: {pdfFilePath} ", SettingsService.ObjectId, SettingsService.UserId, hash, pdfFilePath);


            var request = new RestRequest
            {
                Resource = @"/uploadfile.json",
                Method = Method.POST,
                RequestFormat = DataFormat.Json
            };

            request.AddParameter("object_id", SettingsService.ObjectId, ParameterType.GetOrPost);
            request.AddParameter("user_id", SettingsService.UserId, ParameterType.GetOrPost);

            request.AddParameter("hash", hash, ParameterType.GetOrPost);
            request.AddParameter("pdf_file_path", "/" + pdfFilePath, ParameterType.GetOrPost);

            request.AddFile("file", file);

            var result = Execute<UploadDocumentModel>(request);
            return result;
        }
    }
}
