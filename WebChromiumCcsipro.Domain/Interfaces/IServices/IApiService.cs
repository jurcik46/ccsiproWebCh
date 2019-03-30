namespace WebChromiumCcsipro.Domain.Interfaces.IServices
{
    public interface IApiService
    {
        ISignatureFileModel GetDocumentToSignature();
        bool UploadSignedDocument(string hash, string pdfFilePath, string file);

    }
}