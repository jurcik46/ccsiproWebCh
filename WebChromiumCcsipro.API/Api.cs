using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using RestSharp;
using Serilog;
using Serilog.Core;
using WebChromiumCcsipro.API.Enums;
using WebChromiumCcsipro.Resources.Extensions;
using WebChromiumCcsipro.Resources.Language;
using WebChromiumCcsipro.Resources.Messages;
using LoggerExtensions = WebChromiumCcsipro.Resources.Extensions.LoggerExtensions;


namespace WebChromiumCcsipro.API
{
    public class Api
    {
        private ILogger Logger => Log.Logger.ForContext<Api>();


        public T Execute<T>(RestRequest request) where T : class, new()
        {
            LoggerExtensions.Debug(Logger, ApiEvents.ExecuteType, "API.Execute<{T}>({@request})", typeof(T).FullName, request);
            var client = new RestClient { BaseUrl = this.ApiLink };
            request.AddParameter("api_key", this.Apikey, ParameterType.QueryString);
            var response = client.Execute<T>(request);
            if (response.ErrorException != null)
            {
                if (response.StatusCode == 0)
                    Messenger.Default.Send(new NotifiMessage() { Title = lang.ApiNotificationExecuteConnectionTitle, Msg = lang.ApiNotificationExecuteConnectionInfo, IconType = Notifications.Wpf.NotificationType.Error, ExpTime = 10 });

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
    }
}
