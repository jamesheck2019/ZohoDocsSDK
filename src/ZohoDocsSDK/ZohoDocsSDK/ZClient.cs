using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ZohoDocsSDK.JSON;
using static ZohoDocsSDK.Basic;
using static ZohoDocsSDK.Utilitiez;

namespace ZohoDocsSDK
{
    public class ZClient : IClient
    {

        public ZClient(string accessToken, ConnectionSettings Settings = null)
        {
            authToken = accessToken;
            ConnectionSetting = Settings;

            if (Settings == null)
            {
                m_proxy = null;
            }
            else
            {
                m_proxy = Settings.Proxy;
                m_CloseConnection = Settings.CloseConnection ?? true;
                m_TimeOut = Settings.TimeOut ?? TimeSpan.FromMinutes(60);
            }
            ServicePointManager.Expect100Continue = true; ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
        }


        public IItems Items(List<string> IDs) => new ItemsClient(IDs);
        public IItem Item(string ID) => new ItemClient(ID);


        #region ListAllFiles
        public async Task<List<JSON_FileMetadata>> ListAllFiles(string DestinationFolderID, CategoryEnum Filter = CategoryEnum._ALL_, int? Limit = 200, int? Offset = 0)
        {
            var parameters = new AuthDictionary();
            if (DestinationFolderID != null) { parameters.Add("folderid", DestinationFolderID); }
            if (Filter != CategoryEnum._ALL_)
            {
                parameters.Add("category", Filter.ToString());
            }

            if (Offset.HasValue)
            {
                parameters.Add("start", Offset.Value.ToString());
            }

            if (Limit.HasValue)
            {
                parameters.Add("limit", Limit.Value.ToString());
            }

            using (HttpClient localHttpClient = new HttpClient(new HCHandler()))
            {
                using (HttpResponseMessage response = await localHttpClient.GetAsync(new pUri("files", parameters), HttpCompletionOption.ResponseContentRead).ConfigureAwait(false))
                {
                    var result = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        return JsonConvert.DeserializeObject<List<JSON_FileMetadata>>(result.Jobj().SelectToken("FILES").ToString(), JSONhandler);
                    }
                    else
                    {
                        throw ExceptionCls.CreateException(result.Jobj().SelectToken("response[1].message").ToString(), (int)response.StatusCode);
                    }
                }
            }
        }
        #endregion

        #region ListAllFolders
        public async Task<List<JSON_FolderMetadata>> ListAllFolders(string DestinationFolderID, int? Limit = 200, int? Offset = 0)
        {
            var parameters = new AuthDictionary();
            if (DestinationFolderID != null)
            {
                parameters.Add("folderid", DestinationFolderID);
            }

            if (Offset.HasValue)
            {
                parameters.Add("start", Offset.Value.ToString());
            }

            if (Limit.HasValue)
            {
                parameters.Add("limit", Limit.Value.ToString());
            }

            using (HttpClient localHttpClient = new HttpClient(new HCHandler()))
            {
                using (HttpResponseMessage response = await localHttpClient.GetAsync(new pUri("folders", parameters), HttpCompletionOption.ResponseContentRead).ConfigureAwait(false))
                {
                    var result = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        return JsonConvert.DeserializeObject<List<JSON_FolderMetadata>>(result.Jobj().SelectToken("FOLDER").ToString(), JSONhandler);
                    }

                    else
                    {
                        throw ExceptionCls.CreateException(result.Jobj().SelectToken("response[1].message").ToString(), (int)response.StatusCode);
                    }
                }
            }
        }
        #endregion

        #region ListFilesAndFolders
        public async Task<JSON_ListFolder> ListFilesAndFolders(string DestinationFolderID, int? Limit = 200, int? Offset = 0)
        {
            var parameters = new AuthDictionary();
            if (DestinationFolderID != null)
            {
                parameters.Add("folderid", DestinationFolderID);
            }

            if (Offset.HasValue)
            {
                parameters.Add("start", Offset.Value.ToString());
            }

            if (Limit.HasValue)
            {
                parameters.Add("limit", Limit.Value.ToString());
            }

            using (HttpClient localHttpClient = new HttpClient(new HCHandler()))
            {
                using (HttpResponseMessage response = await localHttpClient.GetAsync(new pUri("folders/files", parameters)).ConfigureAwait(false))
                {
                    var result = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        return JsonConvert.DeserializeObject<JSON_ListFolder>(result, JSONhandler);
                    }
                    else
                    {
                        throw ExceptionCls.CreateException(result.Jobj().SelectToken("response[1].message").ToString(), (int)response.StatusCode);
                    }
                }
            }
        }
        #endregion

        #region ListRootFilesFolders
        public async Task<JSON_ListFolder> ListRoot()
        {
            using (HttpClient localHttpClient = new HttpClient(new HCHandler()))
            {
                var RequestUri = new pUri("folders/files", new AuthDictionary());
                using (HttpResponseMessage response = await localHttpClient.GetAsync(RequestUri).ConfigureAwait(false))
                {
                    var result = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        return JsonConvert.DeserializeObject<JSON_ListFolder>(result, JSONhandler);
                    }
                    else
                    {
                        throw ExceptionCls.CreateException(result.Jobj().SelectToken("response[1].message").ToString(), (int)response.StatusCode);
                    }
                }
            }
        }
        #endregion

        #region RootID
        public string RootID()
        {
            return "1";
        }
        #endregion

        #region ListPublicLink
        public async Task<JSON_ListPublicLink> ListPublicFolder(Uri PublicFolderUrl)
        {
            if (!PublicFolderUrl.AbsoluteUri.ToLower().Contains("/folder/")) { throw ExceptionCls.CreateException("Not a folder url", 404); }

            try
            {
                var tsk = await Task.Factory.StartNew(() =>
                {
                    DeQmaTek.TcpClientHttpRequest hr = new DeQmaTek.TcpClientHttpRequest();
                    hr.Action = PublicFolderUrl.ToString(); // "https://apidocs.zoho.com/folder/buoe2b5ddc16d217447eb9077d60c01b595a2"
                    hr.Method = "GET";
                    // hr.ContentType = "application/json"
                    // hr.Accept = "application/json"
                    hr.UserAgent = "DeQma.TcpClientHttp";
                    hr.Send();
                    var result = System.Text.Encoding.UTF8.GetString(hr.Response.GetStream());
                    string partONE = "var folderObj = ";
                    string partTWO = ";";
                    // Dim FullUrl As String() = result.Split(New String() {partONE, partTWO}, StringSplitOptions.None)
                    var ExtractTheJSON = Between(result, partONE, partTWO);
                    var TheRsult = JsonConvert.DeserializeObject<JSON_ListPublicLink>(ExtractTheJSON, JSONhandler);
                    return TheRsult;
                });
                return tsk;
            }
            catch (Exception ex)
            {
                throw ExceptionCls.CreateException(ex.Message, 1001);
            }
        }
        #endregion

        #region DownloadPublicFile
        public async Task DownloadPublicFile(string FilePublicUrl, string FileSaveDir, string FileName, IProgress<ReportStatus> ReportCls = null, CancellationToken token = default)
        {
            if (ReportCls == null)
            {
                ReportCls = new Progress<ReportStatus>();
            }

            ReportCls.Report(new ReportStatus() { Finished = false, TextStatus = "Initializing..." });
            try
            {
                System.Net.Http.Handlers.ProgressMessageHandler progressHandler = new System.Net.Http.Handlers.ProgressMessageHandler(new HCHandler());
                progressHandler.HttpReceiveProgress += (sender, e) => { ReportCls.Report(new ReportStatus() { ProgressPercentage = e.ProgressPercentage, BytesTransferred = e.BytesTransferred, TotalBytes = e.TotalBytes ?? 0, TextStatus = "Downloading..." }); };
                using (HttpClient localHttpClient = new HttpClient(progressHandler))
                {
                    using (HttpResponseMessage ResPonse = await localHttpClient.GetAsync(new Uri(FilePublicUrl), HttpCompletionOption.ResponseContentRead, token).ConfigureAwait(false))
                    {
                        if (ResPonse.IsSuccessStatusCode)
                        {
                            ReportCls.Report(new ReportStatus() { Finished = true, TextStatus = $"[{FileName}] Downloaded successfully." });
                        }
                        else
                        {
                            ReportCls.Report(new ReportStatus() { Finished = true, TextStatus = $"Error code: {ResPonse.StatusCode}" });
                        }

                        ResPonse.EnsureSuccessStatusCode();
                        var stream_ = await ResPonse.Content.ReadAsStreamAsync();
                        string FPathname = Path.Combine(FileSaveDir, FileName);
                        using (var fileStream = new FileStream(FPathname, FileMode.Append, FileAccess.Write))
                        {
                            stream_.CopyTo(fileStream);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ReportCls.Report(new ReportStatus() { Finished = true });
                if (ex.Message.ToString().ToLower().Contains("a task was canceled"))
                {
                    ReportCls.Report(new ReportStatus() { TextStatus = ex.Message });
                }
                else
                {
                    throw ExceptionCls.CreateException(ex.Message, 1001);
                }
            }
        }
        #endregion

        #region ListTags
        public async Task<List<JSON_TagMetadata>> ListTags()
        {
            using (HttpClient localHttpClient = new HttpClient(new HCHandler()))
            {
                var RequestUri = new pUri("tags", new AuthDictionary());
                using (HttpResponseMessage response = await localHttpClient.GetAsync(RequestUri).ConfigureAwait(false))
                {
                    var result = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        return JsonConvert.DeserializeObject<List<JSON_TagMetadata>>(result.Jobj().SelectToken("UserTagDetails").ToString(), JSONhandler);
                    }
                    else
                    {
                        throw ExceptionCls.CreateException(result.Jobj().SelectToken("response[1].message").ToString(), (int)response.StatusCode);
                    }
                }
            }
        }

        #endregion


    }
}
