using System;

namespace WebChromiumCcsipro.BusinessLogic.Models
{
    public class SignatureFileModel : ISignatureFileModel
    {

        public string PdfFilePath { get; set; }
        public string Hash { get; set; }
        public byte[] File { get; set; }

        public SignatureFileModel(API.Models.SignatureFileModel signatueFile)
        {
            PdfFilePath = signatueFile.PdfFilePath;
            Hash = signatueFile.Hash;
            File = Convert.FromBase64String(signatueFile.File);
        }
    }
}
