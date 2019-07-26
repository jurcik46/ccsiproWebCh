namespace WebChromiumCcsipro.Domain.Interfaces.IServices
{
    public interface ISocketService
    {
        void ServerConnect();
        void HandleDataFromSocket();
        void KioskSendData(string msg);
        void Disconnect();
    }
}