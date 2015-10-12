using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace ImageRaider
{
    public class ImageSearch
    {
        #region Constants

        const string ADD_ENDPOINT = "https://incandescent.xyz/api/add/";

        const string GET_ENDPOINT = "https://incandescent.xyz/api/get/";

        #endregion

        #region Attributes
        /// <summary>
        /// Time as long seconds before request expires
        /// </summary>
        public long Expires { get; set; }

        /// <summary>
        /// Image raider UID
        /// </summary>
        public int UID { get; set; }

        /// <summary>
        /// Image raider API key
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// Add images to be searched
        /// </summary>
        public List<string> Images { get; set; }

        private string Signature { get; set; }

        #endregion

        #region Public Method
        /// <summary>
        /// To be called only once all public properties are filled for this class
        /// </summary>
        /// <returns></returns>
        public async Task<Dictionary<string, Domain>> SearchImages()
        {
            var authResult = await SendSearchTask(UID, ApiKey, Expires, Images);
          
            if (authResult.IsSuccess)
            {
                bool resultReady = true; //expecting result to be immediately ready
                await Task.Delay(5000); //wait 5 seconds before checking status

                while (resultReady)
                {
                    resultReady = CheckIfResultIsReady(authResult.Project_Id);
                    if (resultReady)
                    {
                        await Task.Delay(20000); //wait 20 seconds before retrying
                    }
                }
                if (CheckResponse(authResult.Project_Id).StartsWith("{\"status\":755"))
                {
                    return new Dictionary<string, Domain>();
                }
                
                return await GetImages(authResult.Project_Id);
            }
         
            throw new ImageRaiderException(authResult.Error);
           
        }

        #endregion

        #region Private Methods
        private async Task<AuthenticateResult> SendSearchTask(int uid, string apiKey, long expires, List<string> images)
        {
            HttpContent content = PrepareRequest(uid, apiKey, expires, images);

            return await SendSearchCriteria(ADD_ENDPOINT, content);
        }

        private async Task<Dictionary<string, Domain>> GetImages(string projectId)
        {
            HttpContent content = new StringContent(JsonConvert.SerializeObject(new { uid = UID, expires = Expires, signature = Signature, project_id = projectId }));
            var response = await HttpHelper.PostData(GET_ENDPOINT, content);
            var responseContent = response.Content.ReadAsStringAsync().Result;

            var imageResults = JsonConvert.DeserializeObject<Dictionary<string, Domain>>(responseContent);

            foreach (var imageResult in imageResults)
            {
                foreach (var page in imageResult.Value.pages)
                {
                    page.Images = page.Images == null ? new List<ImageData>() : page.Images;
                    foreach (var searchPage in page.SearchResultData.Values)
                    {
                        var image = JsonConvert.DeserializeObject<ImageData>(searchPage.ToString());

                        page.Images.Add(image);
                    }
                }
            }

            return imageResults;
        }

        private async Task<AuthenticateResult> SendSearchCriteria(string url, HttpContent content)
        {
            var response = HttpHelper.PostData(url, content).Result;
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<AuthenticateResult>(responseContent);
        }

        private HttpContent PrepareRequest(int uid, string apiKey, long expires, List<string> images)
        {
            Expires = expires == default(long) ? UnixTimeNow() + 1000 : UnixTimeNow() + expires;
         
            Signature = CreateSignature(apiKey, uid);
            Signature = Regex.Replace(Signature, "(%[0-9a-f][0-9a-f])", c => c.Value.ToUpper());

            return new StringContent(JsonConvert.SerializeObject(new { uid = uid, expires = Expires, signature = Signature, images = images }));

        }

        private string CreateSignature(string apiKey, int uid)
        {
            Expires = UnixTimeNow() + 1000;
            var stringToSign = uid + "\n" + Expires;

            var sha1 = new HMACSHA1(Encoding.ASCII.GetBytes(apiKey));//

            byte[] byteArray = Encoding.ASCII.GetBytes(stringToSign);

            byte[] hashValue = sha1.ComputeHash(byteArray);

            return HttpUtility.UrlEncode(Convert.ToBase64String(hashValue));
        }

        private bool CheckIfResultIsReady(string projectId)
        {

            var responseContent = CheckResponse(projectId);

            return responseContent.StartsWith("{\"status\":710");
        }

        private string CheckResponse(string projectId)
        {
            HttpContent content = new StringContent(JsonConvert.SerializeObject(new { uid = UID, expires = Expires, signature = Signature, project_id = projectId }));
            var response = HttpHelper.PostData(GET_ENDPOINT, content).Result;
            return response.Content.ReadAsStringAsync().Result;
        }
        private long UnixTimeNow()
        {
            var timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0);

            return (long)timeSpan.TotalSeconds;
        }

        #endregion
    }
}
