using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ZohoDocsSDK;

namespace ZohoDocsSDK
{
    public static class Basic
    {


        //''api doc
        //'https://www.zoho.com/docs/zoho-docs-api.html

        public static string APIbase = "https://apidocs.zoho.com/files/v1/";
        public static JsonSerializerSettings JSONhandler = new JsonSerializerSettings() { MissingMemberHandling = MissingMemberHandling.Ignore, NullValueHandling = NullValueHandling.Ignore };
        public static string authToken = null;
        public static System.TimeSpan m_TimeOut = System.Threading.Timeout.InfiniteTimeSpan; //' TimeSpan.FromMinutes(60)
        public static bool m_CloseConnection = true;
        public static ConnectionSettings ConnectionSetting = null;

        private static ProxyConfig _proxy;
        public static ProxyConfig m_proxy
        {
            get
            {
                return _proxy ?? new ProxyConfig();
            }
            set
            {
                _proxy = value;
            }
        }


        public class HCHandler : System.Net.Http.HttpClientHandler
        {
            public HCHandler() : base()
            {
                if (m_proxy.SetProxy)
                {
                    base.MaxRequestContentBufferSize = 1 * 1024 * 1024;
                    base.Proxy = new System.Net.WebProxy($"http://{m_proxy.ProxyIP}:{m_proxy.ProxyPort}", true, null, new System.Net.NetworkCredential(m_proxy.ProxyUsername, m_proxy.ProxyPassword));
                    base.UseProxy = m_proxy.SetProxy;
                }
            }
        }

        public class HtpClient : System.Net.Http.HttpClient
        {
            public HtpClient(HCHandler HCHandler) : base(HCHandler)
            {
                base.DefaultRequestHeaders.UserAgent.ParseAdd("ZohoDocsSDK");
                base.DefaultRequestHeaders.ConnectionClose = m_CloseConnection;
                base.Timeout = m_TimeOut;
            }
            public HtpClient(System.Net.Http.Handlers.ProgressMessageHandler progressHandler) : base(progressHandler)
            {
                base.DefaultRequestHeaders.UserAgent.ParseAdd("ZohoDocsSDK");
                base.DefaultRequestHeaders.ConnectionClose = m_CloseConnection;
                base.Timeout = m_TimeOut;
            }
        }

        public class AuthDictionary : Dictionary<string, string>
        {
            public AuthDictionary() : base()
            {
                base.Add("SCOPE", "ZohoPC/docsapi");
                //'MyBase.Add("scope", "docsapi")
                base.Add("authtoken", authToken);
            }
        }

        public class pUri : Uri
        {
            public pUri(string ApiAction, Dictionary<string, string> Parameters): base(APIbase + ApiAction + Utilitiez.AsQueryString(Parameters)) {}
            public pUri(string ApiAction) : base(APIbase + ApiAction ) { }
        }

        public static  Newtonsoft.Json.Linq.JObject Jobj(this string response)
        {
            return Newtonsoft.Json.Linq.JObject.Parse(response);
        }

        public static async Task<HttpResponseMessage> RequestAsync(HttpMethod method, string url, HttpContent content)
        {
            using (HtpClient localHtpClient = new HtpClient(new HCHandler()))
            {
                HttpRequestMessage requ = new HttpRequestMessage(method, new Uri(APIbase + url)) { Content = content };
                HttpResponseMessage response = await localHtpClient.SendAsync(requ, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false);
                return response;
            }
        }







    }
}
