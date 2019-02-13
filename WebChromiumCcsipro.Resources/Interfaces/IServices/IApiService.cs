namespace WebChromiumCcsipro.Resources.Interfaces.IServices
{
    public interface IApiService
    {
        ISignatureFileModel GetDocumentToSignature();
        bool UploadSignedDocument(string hash, string pdfFilePath, string file);

    }
}