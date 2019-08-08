namespace WebChromiumCcsipro.Domain.Interfaces.IServices
{
    public interface ICefSharpJsService
    {

        void bozpSignatureJsAsync();
        void sendCameralink(string link);
        void sendApi(string apiLinks);
    }
}