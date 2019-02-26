namespace WebChromiumCcsipro.Resources.Interfaces.IServices
{
    public interface ISignatureService
    {
        bool InProcces { get; set; }
        void StartSign();

    }
}