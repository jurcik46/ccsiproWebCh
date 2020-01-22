﻿using System;
using System.Threading.Tasks;
using Serilog;
using WebChromiumCcsipro.API;
using WebChromiumCcsipro.Domain.Enums;
using WebChromiumCcsipro.Domain.Extensions;
using WebChromiumCcsipro.Domain.Interfaces.IServices;

namespace WebChromiumCcsipro.BusinessLogic.Services
{
    public class CefSharpJsService : ICefSharpJsService
    {
        public ILogger Logger => Log.Logger.ForContext<CefSharpJsService>();

        private ISignatureService SignatureService { get; set; }
        private ISocketService SocketService { get; set; }


        public CefSharpJsService(ISignatureService signatureService, ISocketService socketService)
        {
            Logger.Information(CefSharpJsServiceEvents.Create, "Creating new instance of CefSharpJsService");
            SignatureService = signatureService;
            SocketService = socketService;
        }


        public void bozpSignatureJsAsync()
        {
            if (SignatureService.InProcces)
                return;
            Logger.Information(CefSharpJsServiceEvents.Create);

            Task.Run(() =>
            {
                SignatureService.StartSign();

            });
        }

        public void sendCameralink(string links)
        {
            Logger.Information(CefSharpJsServiceEvents.sendCameralink, $"Camera Link {links}");

            Console.WriteLine(links);
            SocketService.KioskSendData(links);
        }

        public void sendApi(string apiLinks)
        {
            Logger.Information(CefSharpJsServiceEvents.sendApi, $"API Link {apiLinks}");

            if (apiLinks != "")
            {
                try
                {
                    var api = new Api(apiLinks);
                    api.OneEmptyRequest();
                }
                catch (Exception ex)
                {
                    Logger.Error(CefSharpJsServiceEvents.sendApiError, $"Source {ex.Source} Error: {ex.Message} InnerException: {ex.InnerException}");

                }

            }
        }


        public void sendSocketMessage(string ipAddress, int port, string message)
        {
            Logger.Information(CefSharpJsServiceEvents.sendSocketMessage, $"IP address {ipAddress}:{port}  Message: {message}");

            if (ipAddress != "")
            {
                try
                {
                    SocketService.SendOneTimeSocketMessage(ipAddress: ipAddress, port: port, message: message);
                }
                catch (Exception ex)
                {
                    Logger.Error(CefSharpJsServiceEvents.sendSocketMessageError, $"Source {ex.Source} Error: {ex.Message} InnerException: {ex.InnerException}");

                }

            }
        }
    }
}