namespace WebChromiumCcsipro.Domain.Interfaces.IServices
{
    public interface ISocketService
    {
        void Connect();
        void HandleDataFromSocket();
        void Disconnect();
    }
}