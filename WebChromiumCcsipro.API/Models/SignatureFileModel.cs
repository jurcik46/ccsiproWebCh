using RestSharp.Deserializers;

namespace WebChromiumCcsipro.API.Models
{
    public class SignatureFileModel
    {
        [DeserializeAs(Name = "pdf_file_path")]
        public string PdfFilePath { get; set; }

        [DeserializeAs(Name = "hash")]
        public string Hash { get; set; }

        [DeserializeAs(Name = "file")]
        public string File { get; set; }

        [DeserializeAs(Name = "status")]
        public int Status { get; set; }
    }
}