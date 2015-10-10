using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace ImageRaider
{
    public class ImageResult
    {
        public Domain domainResults { get; set; }
    }

    public class Domain
    {
        [JsonConverter(typeof(SingleValueArrayConverter<Pages>))]
        public List<Pages> pages { get; set; }
        public Meta meta { get; set; }
    }

    public class Pages
    {

        [JsonExtensionData]
        public Dictionary<string, JToken> SearchResultData { get; set; }
        public List<ImageData> Images { get; set; }
    }

    public class ImageData
    {
        public string page { get; set; }
        public string base64_page { get; set; }
        public string source { get; set; }
        public int date { get; set; }

        [JsonProperty("usage-image")]
        public string usageimage { get; set; }

        [JsonProperty("usage-height")]
        public string usageheight { get; set; }

        [JsonProperty("usage-width")]
        public string usagewidth { get; set; }
        public string image { get; set; }
        public string iid { get; set; }
    }

    public class Meta
    {
        public string da { get; set; }
    }



}
