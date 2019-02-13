using RestSharp.Deserializers;

namespace WebChromiumCcsipro.API.Models
{
    public class UploadDocumentModel
    {
        [DeserializeAs(Name = "status")]
        public int Status { get; set; }
    }
}