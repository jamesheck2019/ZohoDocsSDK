using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace ZohoDocsSDK
{
    public class Authentication
    {
        #region "Generate AuthToken - [API Mode]"
        /// <summary>
        /// API Mode
        /// To generate AuthToken, you need to send an request to Zoho Accounts
        /// https://accounts.zoho.com/apiauthtoken/nb/create
        /// </summary>
        /// <param name="Email">Mandatory. Specify your Zoho Docs User name or Email Id</param>
        /// <param name="Password">Mandatory. Specify your Zoho Docs password</param>
        /// <returns>Returns the generated AuthToken for the specified user</returns>
        public static async Task<string> GenerateAuthToken(string Email, string Password)
        {
            var parameters = new Dictionary<string, string>
            {
                { "SCOPE", "ZohoPC/docsapi" },
                { "EMAIL_ID", Email },
                { "PASSWORD", Password },
                { "DISPLAY_NAME", "ZohoDocsSDK" }
            };

            using (Basic.HtpClient localHttpClient = new Basic.HtpClient(new Basic.HCHandler()))
            {
                var HtpReqMessage = new HttpRequestMessage(HttpMethod.Post, new Uri("https://accounts.zoho.com/apiauthtoken/nb/create"));
                HtpReqMessage.Content = new FormUrlEncodedContent(parameters);
                using (HttpResponseMessage response = await localHttpClient.SendAsync(HtpReqMessage, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false))
                {
                    var result = await response.Content.ReadAsStringAsync();
                    string[] tokn = result.ToString().Split(new string[] { "=", "=" }, StringSplitOptions.None);
                    if (tokn[2].ToString().Trim() == "TRUE")
                    {
                        return tokn[1].Replace("RESULT", "");
                    }
                    else if (tokn[2].ToString().Trim() == "FALSE")
                    {
                        throw new ZohoDocsException(tokn[1].Replace("RESULT", ""), (int)response.StatusCode);
                    }
                    else { return null; }
                }
            }
        }
        #endregion

        #region "Generate AuthToken From Browser - [Browser Mode]"
        /// <summary>
        /// Browser Mode
        /// Generates the AuthToken
        /// https://accounts.zoho.com/apiauthtoken/create
        /// </summary>
        /// <returns>Returns the generated AuthToken for the specified user.</returns>
        public static string GetAuthTokenFromBrowser()
        {
            return ("https://accounts.zoho.com/apiauthtoken/create" + Utilitiez.AsQueryString(new Dictionary<string, string> { { "SCOPE", "ZohoPC/docsapi" }, { "DISPLAY_NAME", "ZohoDocsSDK" } }));
        }
        #endregion
















    }
}
