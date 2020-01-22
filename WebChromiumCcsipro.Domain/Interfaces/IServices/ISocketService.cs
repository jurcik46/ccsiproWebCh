namespace WebChromiumCcsipro.Domain.Interfaces.IServices
{
    public interface ISocketService
    {
        void ServerConnect();
        void HandleDataFromSocket();
        void SendOneTimeSocketMessage(string ipAddress, int port, string message);
        void KioskSendData(string msg);
        void Disconnect();

    }
}