using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using Serilog;
using WebChromiumCcsipro.Domain.Enums;
using WebChromiumCcsipro.Domain.Extensions;
using WebChromiumCcsipro.Domain.Interfaces.IServices;
using WebChromiumCcsipro.Domain.Messages;
using WebChromiumCcsipro.Resources.Language;


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
        private int _serverReconnectCount;
        private int _kioskReconnectCount;
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
            Task.Run(ServerReconnect);
            Task.Run(KioskReconnect);
        }

        private void KioskConnect()
        {
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
                Logger.Information(SocketServiceEvents.KioskConnectSuccessfully);
            }
            catch (SocketException socketErrorException)
            {
                Logger.Error(SocketServiceEvents.KioskConnectSocketError, $" Message: {socketErrorException.Message} Code: {socketErrorException.SocketErrorCode} Error {socketErrorException.StackTrace} ");
                //ConnectionExceptionNotify();
            }
            catch (Exception errorException)
            {
                Logger.Error(SocketServiceEvents.KioskConnectError, $"  Source: {errorException.Source} Message: {errorException.Message}   Error {errorException.StackTrace}  ");
                //ConnectionExceptionNotify();
            }
        }

        public void ServerConnect()
        {
            Logger.Debug(SocketServiceEvents.ServerConnect);
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
                Logger.Information(SocketServiceEvents.ServerConnectSuccessfully);
                Messenger.Default.Send(new TrayIconsStatusMessage()
                {
                    IconStatus = TrayIconsStatus.Online
                });

                Task.Run(HandleDataFromSocket);
            }
            catch (SocketException socketErrorException)
            {
                Logger.Error(SocketServiceEvents.ServerConnectSocketError, $" Message: {socketErrorException.Message} Code: {socketErrorException.SocketErrorCode} Error {socketErrorException.StackTrace} ");
                ConnectionExceptionNotify();
            }
            catch (Exception errorException)
            {
                Logger.Error(SocketServiceEvents.ServerConnectError, $"  Source: {errorException.Source} Message: {errorException.Message}   Error {errorException.StackTrace}  ");
                ConnectionExceptionNotify();
            }
        }

        private void ConnectionExceptionNotify()
        {
            if (_serverReconnectCount == 1)
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
            try
            {
                Logger.Information($"Kiosk Sending data: {msg}");
                ASCIIEncoding asen = new ASCIIEncoding();
                byte[] bytes = asen.GetBytes(msg);
                _kioskStream.Write(bytes, 0, bytes.Length);
            }
            catch (Exception errorException)
            {
                Logger.Error(SocketServiceEvents.KioskSendDataError,
               $"  Source: {errorException.Source} Message: {errorException.Message}   Error {errorException.StackTrace}  ");
                Task.Run(KioskReconnect);
            }
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
            try
            {
                byte[] bytes = new byte[1024];
                receivedBytes = _serverStream.Read(bytes, 0, bytes.Length);
                return Encoding.UTF8.GetString(bytes);
            }
            catch (Exception errorException)
            {
                Logger.Error(SocketServiceEvents.ServerReadDataError,
                    $"  Source: {errorException.Source} Message: {errorException.Message}   Error {errorException.StackTrace}  ");
                receivedBytes = -1;
                return " ";
            }

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
                        Logger.Error(SocketServiceEvents.ServerConnectError,
                            $"  Source: {errorException.Source} Message: {errorException.Message}   Error {errorException.StackTrace}  ");
                    }
                    continue;
                }
                Logger.Information(SocketServiceEvents.ServerReadData, $"Received data: {data}");

                if (bytesCount == -1)
                    continue;
                //string[] splitData = data.Replace("\0", string.Empty).Split(';');
                //Array.Resize(ref splitData, splitData.Length - 1);

                //for (int i = 0; i < splitData.Length; i++)
                //{
                //    jsExeMessage.Parameters[i] = splitData[i];
                //}
                ExecuteJavaScriptMessage jsExeMessage = new ExecuteJavaScriptMessage() { Function = "refresh", Parameters = null };
                Messenger.Default.Send(jsExeMessage);
                //var jsonData = new JavaScriptSerializer().Deserialize<MotionDetectSocketModel>(data);
                //Console.WriteLine(jsonData.Time);
            }
            Task.Run(ServerReconnect);
        }


        private void ServerReconnect()
        {
            _serverReconnectCount = 0;
            while (_serverTcpClient != null && !_serverTcpClient.Connected)
            {
                Messenger.Default.Send(new TrayIconsStatusMessage()
                {
                    IconStatus = TrayIconsStatus.Offline
                });
                _serverReconnectCount++;
                Thread.Sleep(_serverReconnectCount * 1000);
                Logger.Warning(SocketServiceEvents.ServerTryingReconnect, $"Server TCP reconnect count: {_serverReconnectCount}");
                ServerConnect();
            }
            _serverReconnectCount = 0;

        }

        private void KioskReconnect()
        {
            _kioskReconnectCount = 0;
            while (_kioskTcpClient != null && !_kioskTcpClient.Connected)
            {
                _kioskReconnectCount++;
                Thread.Sleep(_kioskReconnectCount * 1000);
                Logger.Warning(SocketServiceEvents.KioskTryingReconnect, $"Kiosk TCP reconnect count: {_kioskReconnectCount}");
                KioskConnect();
            }
            _kioskReconnectCount = 0;

        }

        public void Disconnect()
        {
            Logger.Debug(SocketServiceEvents.Disconnect);
            _serverTcpClient.Close();
            _serverTcpClient.Dispose();
            _serverTcpClient = null;
            _kioskTcpClient.Close();
            _kioskTcpClient.Dispose();
            _kioskTcpClient = null;
        }
    }
}