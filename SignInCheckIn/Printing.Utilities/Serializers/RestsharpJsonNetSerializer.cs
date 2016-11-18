using Newtonsoft.Json;
using RestSharp.Serializers;

namespace Printing.Utilities.Serializers
{
    public class RestsharpJsonNetSerializer : ISerializer
    {
        public RestsharpJsonNetSerializer()
        {
            ContentType = "application/json";
        }

        public string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.None);
        }

        string ISerializer.RootElement { get; set; }
        string ISerializer.Namespace { get; set; }
        string ISerializer.DateFormat { get; set; }
        public string ContentType { get; set; }
    }
}
