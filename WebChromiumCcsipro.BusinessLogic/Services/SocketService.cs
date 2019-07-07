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

        private TcpClient _serverTcpClient;
        private Stream _serverStream;
        private string _serverIp;
        private int _serverPort;
        private TcpClient _kioskTcpClient;
        private Stream _kioskStream;
        private string _kioskIp;
        private int _kioskPort;
        private int _reconnectCount;
        private ISettingsService _settingsService;

        public SocketService(ISettingsService settingsService)
        {
            Logger.Information(SocketServiceEvents.Create, "Creating new instance of SocketService");
            _settingsService = settingsService;
            _serverIp = _settingsService.ServerIp;
            _serverPort = _settingsService.ServerPort;
            _serverTcpClient = new TcpClient();
            _kioskTcpClient = new TcpClient();
            _kioskIp = _settingsService.KioskIp;
            _kioskPort = _settingsService.KioskPort;
            Task.Run(() =>
            {
                Reconnect();
            });
            KioskConnect();
        }

        private void KioskConnect()
        {
            //TODO refactor kiosk Connect and make logs 
            Logger.Debug(SocketServiceEvents.KioskConnect);
            if (_kioskIp.Equals("") || _kioskPort == 0 || _kioskTcpClient.Connected)
            {
                return;
            }

            try
            {
                _kioskTcpClient.Dispose();
                _kioskTcpClient = new TcpClient();
                _kioskTcpClient.Connect(_kioskIp, _kioskPort);
                _kioskStream = _kioskTcpClient.GetStream();
                //Logger.Information(SocketServiceEvents.ConnectSuccessfully);
                //Messenger.Default.Send(new TrayIconsStatusMessage()
                //{
                //    IconStatus = TrayIconsStatus.Online
                //});

                //Task.Run(() =>
                //{
                //    HandleDataFromSocket();
                //});
            }
            catch (SocketException socketErrorException)
            {
                Logger.Error(SocketServiceEvents.ConnectSocketError, $" Message: {socketErrorException.Message} Code: {socketErrorException.SocketErrorCode} Error {socketErrorException.StackTrace} ");
                //ConnectionExceptionNotify();
            }
            catch (Exception errorException)
            {
                Logger.Error(SocketServiceEvents.ConnectError, $"  Source: {errorException.Source} Message: {errorException.Message}   Error {errorException.StackTrace}  ");
                //ConnectionExceptionNotify();
            }
        }

        public void Connect()
        {
            Logger.Debug(SocketServiceEvents.Connect);
            if (_serverIp.Equals("") || _serverPort == 0 || _serverTcpClient.Connected)
            {
                return;
            }
            try
            {
                _serverTcpClient.Dispose();
                _serverTcpClient = new TcpClient();
                _serverTcpClient.Connect(_serverIp, _serverPort);
                _serverStream = _serverTcpClient.GetStream();
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
                Messenger.Default.Send(new NotifyMessage()
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

        public void KioskSendData(string msg)
        {
            Logger.Information($"Kiosk Sending data: {msg}");
            ASCIIEncoding asen = new ASCIIEncoding();
            byte[] bytes = asen.GetBytes(msg);
            _kioskStream.Write(bytes, 0, bytes.Length);
        }

        private void ServerSendData(string msg)
        {
            Logger.Information(SocketServiceEvents.SendData, $"Sending data: {msg}");
            ASCIIEncoding asen = new ASCIIEncoding();
            byte[] bytes = asen.GetBytes(msg);
            _serverStream.Write(bytes, 0, bytes.Length);
        }

        private string ServerReadData(out int receivedBytes)
        {
            byte[] bytes = new byte[1024];
            receivedBytes = _serverStream.Read(bytes, 0, bytes.Length);
            return Encoding.UTF8.GetString(bytes);
        }

        public void HandleDataFromSocket()
        {
            while (_serverTcpClient.Connected)
            {
                string data = ServerReadData(out int bytesCount);
                //Condition handle server disconnect
                if (bytesCount == 0)
                {
                    try
                    {
                        ServerSendData(" ");
                        _serverTcpClient.Close();
                    }
                    catch (Exception errorException)
                    {
                        Logger.Error(SocketServiceEvents.ConnectError,
                            $"  Source: {errorException.Source} Message: {errorException.Message}   Error {errorException.StackTrace}  ");
                    }
                    continue;
                }
                Logger.Information(SocketServiceEvents.ReadData, $"Received data: {data}");


                //string[] splitData = data.Replace("\0", string.Empty).Split(';');
                //Array.Resize(ref splitData, splitData.Length - 1);
                ExecuteJavaScriptMessage jsExeMessage = new ExecuteJavaScriptMessage() { Function = "clickCallback", Parameters = null };
                //for (int i = 0; i < splitData.Length; i++)
                //{
                //    jsExeMessage.Parameters[i] = splitData[i];
                //}
                Messenger.Default.Send(jsExeMessage);
                //var jsonData = new JavaScriptSerializer().Deserialize<MotionDetectSocketModel>(data);
                //Console.WriteLine(jsonData.Time);
            }
            Task.Run(() =>
            {
                Reconnect();
            });
        }


        private void Reconnect()
        {
            _reconnectCount = 0;
            while (_serverTcpClient != null && !_serverTcpClient.Connected)
            {
                Messenger.Default.Send(new TrayIconsStatusMessage()
                {
                    IconStatus = TrayIconsStatus.Offline
                });
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
            _serverTcpClient.Close();
            _serverTcpClient.Dispose();
            _kioskTcpClient.Close();
            _kioskTcpClient.Dispose();
        }
    }
}