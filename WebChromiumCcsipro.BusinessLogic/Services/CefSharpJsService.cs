using System.Threading.Tasks;
using WebChromiumCcsipro.Domain.Interfaces.IServices;

namespace WebChromiumCcsipro.BusinessLogic.Services
{
    public class CefSharpJsService : ICefSharpJsService
    {
        private ISignatureService SignatureService { get; set; }


        public CefSharpJsService(ISignatureService signatureService)
        {
            SignatureService = signatureService;
        }


        public void bozpSignatureJsAsync()
        {
            if (SignatureService.InProcces)
                return;

            Task.Run(() =>
            {
                SignatureService.StartSign();

            });
        }


    }
}