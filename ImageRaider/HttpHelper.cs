using System.Net.Http;
using System.Threading.Tasks;

namespace ImageRaider
{
    internal static class HttpHelper
    {

        internal static async Task<HttpResponseMessage> PostData(string url, HttpContent content)
        {
            using (var client = new HttpClient())
            {
                client.Timeout = System.TimeSpan.FromMinutes(5);
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                content.Headers.ContentType.MediaType = "application/json";
                return await client.PostAsync(url, content);
            }
        }


    }
}
