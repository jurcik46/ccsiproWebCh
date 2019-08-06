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


            SocketService.KioskSendData(links);
        }

        public void sendApi(string apiLinks)
        {
            //Logger.Information(CefSharpJsServiceEvents.sendCameralink, $"Camera Link {links}");
            //TODO make api request 
        }


    }
}