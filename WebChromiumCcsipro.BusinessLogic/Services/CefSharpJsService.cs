using System.Threading.Tasks;
using Serilog;
using WebChromiumCcsipro.Domain.Enums;
using WebChromiumCcsipro.Domain.Extensions;
using WebChromiumCcsipro.Domain.Interfaces.IServices;

namespace WebChromiumCcsipro.BusinessLogic.Services
{
    public class CefSharpJsService : ICefSharpJsService
    {
        public ILogger Logger => Log.Logger.ForContext<CefSharpJsService>();

        private ISignatureService _signatureService { get; set; }
        private ISocketService _socketService { get; set; }


        public CefSharpJsService(ISignatureService signatureService, ISocketService socketService)
        {
            Logger.Information(CefSharpJsServiceEvents.Create, "Creating new instance of CefSharpJsService");
            _signatureService = signatureService;
            _socketService = socketService;
        }


        public void bozpSignatureJsAsync()
        {
            if (_signatureService.InProcces)
                return;
            Logger.Information(CefSharpJsServiceEvents.Create);

            Task.Run(() =>
            {
                _signatureService.StartSign();

            });
        }

        public void sendCameralink(string link)
        {
            Logger.Information(CefSharpJsServiceEvents.sendCameralink, $"Camera Link {link}");
            _socketService.KioskSendData(link);
        }


    }
}