using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using GalaSoft.MvvmLight.Messaging;
using Serilog;
using WebChromiumCcsipro.BusinessLogic.Models;
using WebChromiumCcsipro.Domain.Enums;
using WebChromiumCcsipro.Domain.Extensions;
using WebChromiumCcsipro.Domain.Interfaces.IServices;
using WebChromiumCcsipro.Domain.Messages;
using WebChromiumCcsipro.Resources.Language;
using LoggerExtensions = WebChromiumCcsipro.Domain.Extensions.LoggerExtensions;


namespace WebChromiumCcsipro.BusinessLogic.Services
{
    public class SocketService : ISocketService
    {
        public ILogger Logger => Log.Logger.ForContext<SocketService>();

        private TcpClient _tcpClient;
        private Stream _stream;
        private string _serverIp;
        private int _serverPort;
        private int _reconnectCount;
        private ISettingsService _settingsService;

        public SocketService(ISettingsService settingsService)
        {
            Logger.Information(SocketServiceEvents.Create, "Creating new instance of SocketService");
            _settingsService = settingsService;
            _serverIp = _settingsService.ServerIp;
            _serverPort = _settingsService.ServerPort;
            _tcpClient = new TcpClient();
            Task.Run(() =>
            {
                Reconnect();
            });
        }

        public void Connect()
        {
            Logger.Debug(SocketServiceEvents.Connect);
            if (_serverIp.Equals("") || _serverPort == 0 || _tcpClient.Connected)
            {
                return;
            }
            try
            {
                _tcpClient.Dispose();
                _tcpClient = new TcpClient();
                _tcpClient.Connect(_serverIp, _serverPort);
                _stream = _tcpClient.GetStream();
                Logger.Information(SocketServiceEvents.ConnectSuccessfully);
                Messenger.Default.Send(new TrayIconsStatusMessage()
                {
                    IconStatus = TrayIconsStatus.Online
                });

                Task.Run(() =>
                {
                    HandleDataFromSocket();
                });
            }
            catch (SocketException socketErrorException)
            {
                Logger.Error(SocketServiceEvents.ConnectSocketError, $" Message: {socketErrorException.Message} Code: {socketErrorException.SocketErrorCode} Error {socketErrorException.StackTrace} ");
                ConnectionExceptionNotify();
            }
            catch (Exception errorException)
            {
                Logger.Error(SocketServiceEvents.ConnectError, $"  Source: {errorException.Source} Message: {errorException.Message}   Error {errorException.StackTrace}  ");
                ConnectionExceptionNotify();
            }
        }

        private void ConnectionExceptionNotify()
        {
            if (_reconnectCount == 1)
            {
                Messenger.Default.Send(new NotifiMessage()
                {
                    Title = lang.SocketNotificationConnectionTitle,
                    Msg = lang.SocketNotificationConnectionError,
                    IconType = Notifications.Wpf.NotificationType.Warning
                });
                Messenger.Default.Send(new TrayIconsStatusMessage()
                {
                    IconStatus = TrayIconsStatus.Offline
                });
            }
        }

        private void SendData(string msg)
        {
            Logger.Information(SocketServiceEvents.SendData, $"Sending data: {msg}");
            ASCIIEncoding asen = new ASCIIEncoding();
            byte[] bytes = asen.GetBytes(msg);
            _stream.Write(bytes, 0, bytes.Length);
        }

        private string ReadData(out int receivedBytes)
        {
            byte[] bytes = new byte[1024];
            receivedBytes = _stream.Read(bytes, 0, bytes.Length);
            return System.Text.Encoding.UTF8.GetString(bytes);
        }

        public void HandleDataFromSocket()
        {
            while (_tcpClient.Connected)
            {
                string data = ReadData(out int bytesCount);
                //Condition handle server disconnect
                if (bytesCount == 0)
                {
                    try
                    {
                        SendData(" ");

                    }
                    catch (Exception errorException)
                    {
                        Logger.Error(SocketServiceEvents.ConnectError, $"  Source: {errorException.Source} Message: {errorException.Message}   Error {errorException.StackTrace}  ");
                        break;
                    }
                }
                Logger.Information(SocketServiceEvents.ReadData, $"Received data: {data}");
                var jsonData = new JavaScriptSerializer().Deserialize<MotionDetectSocketModel>(data);
                Console.WriteLine(jsonData.Time);
            }
            Task.Run(() => { Reconnect(); });
        }
        //public bool SendMotionDetectSocket(MotionSocketModel motionSocketModel)
        //{
        //    Logger.Debug(SocketServiceEvents.SendMotionDetectSocket);
        //    if (!_tcpClient.Connected)
        //        return false;
        //    var jsonData = new JavaScriptSerializer().Serialize(motionSocketModel);
        //    try
        //    {
        //        SendData(jsonData);
        //    }
        //    catch (Exception errorException)
        //    {
        //        Logger.Error(SocketServiceEvents.SendMotionDetectSocketError, $" Source: {errorException.Source} Message: {errorException.Message}   Error {errorException.StackTrace}  ");
        //        Task.Run(() =>
        //        {
        //            Reconnect();
        //        });
        //        return false;
        //    }
        //    return true;
        //}

        private void Reconnect()
        {
            _reconnectCount = 0;
            while (!_tcpClient.Connected)
            {
                _reconnectCount++;
                Thread.Sleep(_reconnectCount * 1000);
                Logger.Warning(SocketServiceEvents.TryingReconnect, $"TCP reconnect count: {_reconnectCount}");
                Connect();
            }
            _reconnectCount = 0;
        }

        public void Disconnect()
        {
            Logger.Debug(SocketServiceEvents.Disconnect);
            _tcpClient.Close();
            _tcpClient.Dispose();
        }
    }
}