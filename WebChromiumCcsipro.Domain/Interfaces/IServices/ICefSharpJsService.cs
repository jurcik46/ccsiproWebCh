namespace WebChromiumCcsipro.Domain.Interfaces.IServices
{
    public interface ICefSharpJsService
    {

        void bozpSignatureJsAsync();
        void sendCameralink(string link);
        void sendApi(string apiLinks);
        void sendSocketMessage(string ipAddress, int port, string message);
    }
}