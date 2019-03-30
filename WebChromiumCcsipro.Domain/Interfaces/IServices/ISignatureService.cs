namespace WebChromiumCcsipro.Domain.Interfaces.IServices
{
    public interface ISignatureService
    {
        bool InProcces { get; set; }
        void StartSign();

    }
}