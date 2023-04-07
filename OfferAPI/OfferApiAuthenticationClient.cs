using System.Configuration;
using System.IO;
using System.Net;
using System.Web.Script.Serialization;

namespace Tourpaq.Helpers.Auth.OfferAPI
{
    public class OfferApiAuthenticationClient
    {
        private string _endpoint
        {
            get
            {
                string offerApiUrl = ConfigurationManager.AppSettings["NewOfferApiUrl"];
                if (!string.IsNullOrEmpty(offerApiUrl))
                    return string.Format("{0}{1}", offerApiUrl, "login");
                return "";
            }
        }
        private readonly HttpWebRequest _httpRequest;
        public OfferApiAuthenticationClient(string serviceCode)
        {
            _httpRequest = (HttpWebRequest)WebRequest.Create(_endpoint);

            _httpRequest.KeepAlive = false;
            _httpRequest.MaximumAutomaticRedirections = 30;
            _httpRequest.Timeout = 30000;
            _httpRequest.Method = "POST";
            _httpRequest.Accept = "application/json, text/javascript, */*; q=0.01";
            _httpRequest.ContentType = "application/json";
            _httpRequest.UserAgent = "Tourpaq Service: " + serviceCode;
            _httpRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            _httpRequest.Headers.Remove("Accept-Encoding");
            _httpRequest.Headers.Add("Accept-Encoding", "gzip, deflate");
        }

        public Tourpaq.Common.TokenResponse RequestToken(string username, string password)
        {
            using (var streamWriter = new StreamWriter(_httpRequest.GetRequestStream()))
            {
                string json = new JavaScriptSerializer().Serialize(new
                {
                    userName = username,
                    password = password
                });

                streamWriter.Write(json);
            }

            var _httpResponse = (HttpWebResponse)_httpRequest.GetResponse();
            var _responseStream = new StreamReader(_httpResponse.GetResponseStream());
            var result = _responseStream.ReadToEnd();

            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            var obj = serializer.Deserialize<Tourpaq.Common.TokenResponse>(result);
            return obj;
        }
    }
}
