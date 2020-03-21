using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ZohoDocsSDK.JSON;
using static ZohoDocsSDK.Utilitiez;
using static ZohoDocsSDK.Basic;

namespace ZohoDocsSDK
{
    public class ItemClient : IItem
    {
        private string ID { get; set; }
        public ItemClient(string ID)
        {
            this.ID = ID;
        }

        #region RenameFileFolder
        public async Task<bool> FD_Rename(string NewName)
        {
            var parameters = new AuthDictionary
            {
                { "docid", ID },
                { "docname", NewName }
            };

            using (HtpClient localHttpClient = new HtpClient(new HCHandler()))
            {
                HttpRequestMessage HtpReqMessage = new HttpRequestMessage(HttpMethod.Post, new pUri("rename"));
                HtpReqMessage.Content = new FormUrlEncodedContent(parameters);
                using (HttpResponseMessage response = await localHttpClient.SendAsync(HtpReqMessage, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false))
                {
                    var result = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        return JObject.Parse(result).SelectToken("response.result.DocumentDetails.DocumentDetail.message").ToString().Contains("SUCCESSFULLY"); // ("response")("result")("DocumentDetails")("DocumentDetail")("message").ToString().Contains("SUCCESSFULLY")
                    }
                    else
                    {
                        throw ExceptionCls.CreateException(result.Jobj().SelectToken("response.result.DocumentDetails.DocumentDetail.message").ToString(), (int)response.StatusCode);
                    }
                }
            }
        }
        #endregion

        #region MoveFileFolder
        public async Task<bool> FD_Move(string DestinationFolderID)
        {
            var parameters = new AuthDictionary() { { "docid", ID }, { "folderid", DestinationFolderID } };

            using (HtpClient localHttpClient = new HtpClient(new HCHandler()))
            {
                HttpRequestMessage HtpReqMessage = new HttpRequestMessage(HttpMethod.Post, new pUri("move"));
                HtpReqMessage.Content = new FormUrlEncodedContent(parameters);
                using (HttpResponseMessage response = await localHttpClient.SendAsync(HtpReqMessage, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false))
                {
                    var result = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        return result.Jobj().SelectToken("response.result.message").ToString().Contains("SUCCESSFULLY");
                    }
                    else
                    {
                        throw ExceptionCls.CreateException(result.Jobj().SelectToken("response.result.message").ToString(), (int)response.StatusCode);
                    }
                }
            }
        }
        #endregion

        #region TrashFileFolder
        public async Task<bool> FD_Trash()
        {
            var parameters = new AuthDictionary();
            parameters.Add("docid", ID);

            using (HtpClient localHttpClient = new HtpClient(new HCHandler()))
            {
                HttpRequestMessage HtpReqMessage = new HttpRequestMessage(HttpMethod.Post, new pUri("trash"));
                HtpReqMessage.Content = new FormUrlEncodedContent(parameters);
                using (HttpResponseMessage response = await localHttpClient.SendAsync(HtpReqMessage, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false))
                {
                    var result = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        return result.Jobj().SelectToken("response.result.message").ToString().Contains("SUCCESSFULLY");
                    }
                    else
                    {
                        throw ExceptionCls.CreateException(result.Jobj().SelectToken("response.result.message").ToString(), (int)response.StatusCode);
                    }
                }
            }
        }
        #endregion

        #region DeleteFileFolderPermanently
        public async Task<bool> FD_Delete()
        {
            var parameters = new AuthDictionary() { { "docid", ID } };

            using (HtpClient localHttpClient = new HtpClient(new HCHandler()))
            {
                System.Net.Http.HttpRequestMessage HtpReqMessage = new System.Net.Http.HttpRequestMessage(HttpMethod. Post, new pUri("delete"));
                HtpReqMessage.Content = new FormUrlEncodedContent(parameters);
                using (System.Net.Http.HttpResponseMessage response = await localHttpClient.SendAsync(HtpReqMessage, System.Net.Http.HttpCompletionOption.ResponseContentRead).ConfigureAwait(false))
                {
                    var result = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                        return result.Jobj().SelectToken("response.result.message").ToString().Contains("SUCCESSFULLY");
                    else
                        throw ExceptionCls.CreateException(result.Jobj().SelectToken("response.result.message").ToString(), (int)response.StatusCode);
                }
            }
        }
        #endregion

        #region ShareFileFolder
        public async Task<string> FD_Share(PermissionEnum Permission, string Password = null, DateTime ExpireDate = default)
        {
            var parameters = new AuthDictionary
            {
                { "docid", ID },
                { "visibility", "linkshare" },
                { "permission", Permission.ToString() },
                { "password", Password },
                { "expireson", ExpireDate.ToString("mm/dd/yyyy") }
            };

            using (HtpClient localHttpClient = new HtpClient(new HCHandler()))
            {
                HttpRequestMessage HtpReqMessage = new HttpRequestMessage(HttpMethod.Post, new pUri("share/visibility"));
                HtpReqMessage.Content = new FormUrlEncodedContent(parameters);
                using (HttpResponseMessage response = await localHttpClient.SendAsync(HtpReqMessage, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false))
                {
                    var result = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                        return result.Jobj().SelectToken("response[2].result[0].permaLink").ToString();
                    else
                        throw ExceptionCls.CreateException(result.Jobj().SelectToken("response[1].message").ToString(), (int)response.StatusCode);
                }
            }
        }
        #endregion

        #region UnShareFileFolder
        public async Task<bool> FD_UnShare()
        {
            using (HtpClient localHttpClient = new HtpClient(new HCHandler()))
            {
                HttpRequestMessage HtpReqMessage = new HttpRequestMessage(HttpMethod.Post, new pUri("share/visibility"));
                HtpReqMessage.Content = new FormUrlEncodedContent(new AuthDictionary() { { "docid", ID }, { "visibility", "private" } });
                using (HttpResponseMessage response = await localHttpClient.SendAsync(HtpReqMessage, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false))
                {
                    var result = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                        return result.Jobj().SelectToken("response[1].message").ToString().Contains("SUCCESS");
                    else
                        throw ExceptionCls.CreateException(result.Jobj().SelectToken("response[1].message").ToString(), (int)response.StatusCode);
                }
            }
        }
        #endregion

        #region SharedDetails
        public async Task<JSON_SharesMetadata> FD_SharesMetadata()
        {
            using (HtpClient localHttpClient = new HtpClient(new HCHandler()))
            {
                var RequestUri = new pUri("share/details", new AuthDictionary() { { "docid", ID } });
                using (HttpResponseMessage response = await localHttpClient.GetAsync(RequestUri).ConfigureAwait(false))
                {
                    var result = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                        return JsonConvert.DeserializeObject<JSON_SharesMetadata>(result, JSONhandler);
                    else
                        throw ExceptionCls.CreateException(result.Jobj().SelectToken("response[1].message").ToString(), (int)response.StatusCode);
                }
            }
        }
        #endregion

        #region CopyFolder
        public async Task<bool> D_Copy(string DestinationFolderID)
        {
            var parameters = new AuthDictionary() { { "folderid", ID }, { "destfolderid", DestinationFolderID } };

            using (HtpClient localHttpClient = new HtpClient(new HCHandler()))
            {
                HttpRequestMessage HtpReqMessage = new HttpRequestMessage(HttpMethod.Post, new pUri("folders/copy"));
                HtpReqMessage.Content = new FormUrlEncodedContent(parameters);
                using (HttpResponseMessage response = await localHttpClient.SendAsync(HtpReqMessage, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false))
                {
                    var result = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                        return result.Jobj().SelectToken("response.result.message").ToString().Contains("SUCCESSFULLY");
                    else
                        throw ExceptionCls.CreateException(result.Jobj().SelectToken("response.result.message").ToString(), (int)response.StatusCode);
                }
            }
        }
        #endregion

        #region CreateNewFolder
        public async Task<JSON_NewFolder> D_Create(string FolderName)
        {
            var parameters = new AuthDictionary() { { "parentfolderid", ID }, { "foldername", FolderName } };

            using (HtpClient localHttpClient = new HtpClient(new HCHandler()))
            {
                HttpRequestMessage HtpReqMessage = new HttpRequestMessage(HttpMethod.Post, new pUri("folders/create"));
                HtpReqMessage.Content = new FormUrlEncodedContent(parameters);
                using (HttpResponseMessage response = await localHttpClient.SendAsync(HtpReqMessage, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false))
                {
                    var result = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                        return JsonConvert.DeserializeObject<JSON_NewFolder>(result.Jobj().SelectToken("response.result.FolderDetails.FolderDetail").ToString(), JSONhandler);
                    else
                        throw ExceptionCls.CreateException(result.Jobj().SelectToken("response.result.FolderDetails.FolderDetail.message").ToString(), (int)response.StatusCode);
                }
            }
        }
        #endregion

        #region UploadLocal
        public async Task<JSON_Upload> D_Upload(object FileToUpload, UploadTypes UploadType, string FileName, string DestinationWorkspaceID = null, IProgress<ReportStatus> ReportCls = null, CancellationToken token = default)
        {
            if (ReportCls == null)
                ReportCls = new Progress<ReportStatus>();
            ReportCls.Report(new ReportStatus() { Finished = false, TextStatus = "Initializing..." });
            try
            {
                System.Net.Http.Handlers.ProgressMessageHandler progressHandler = new System.Net.Http.Handlers.ProgressMessageHandler(new HCHandler());
                progressHandler.HttpSendProgress += ((sender, e) =>
                {
                    ReportCls.Report(new ReportStatus() { ProgressPercentage = e.ProgressPercentage, BytesTransferred = e.BytesTransferred, TotalBytes = e.TotalBytes ?? 0 , TextStatus = "Uploading..." });
                });
                // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                using (HtpClient localHttpClient = new HtpClient(progressHandler))
                {
                    HttpRequestMessage HtpReqMessage = new HttpRequestMessage();
                    HtpReqMessage.Method = HttpMethod.Post;
                    // '''''''''''''''''''''''''''''''''
                    var MultipartsformData = new MultipartFormDataContent();
                    HttpContent streamContent = null;
                    switch (UploadType)
                    {
                        case UploadTypes.FilePath:
                            streamContent = new StreamContent(new System.IO.FileStream(FileToUpload.ToString(), System.IO.FileMode.Open, System.IO.FileAccess.Read));
                            break;
                        case UploadTypes.Stream:
                            streamContent = new StreamContent((System.IO.Stream)FileToUpload);
                            break;
                        case UploadTypes.BytesArry:
                            streamContent = new StreamContent(new System.IO.MemoryStream((byte[])FileToUpload));
                            break;
                        case UploadTypes.String:
                            streamContent = new StringContent(System.IO.File.ReadAllText(FileToUpload.ToString()));
                            break;
                    }
                    MultipartsformData.Add(streamContent, "content", FileName);
                    MultipartsformData.Add(new StringContent(authToken), "authtoken");
                    MultipartsformData.Add(new StringContent("docsapi"), "scope");
                    MultipartsformData.Add(new StringContent(FileName), "filename");
                    if (ID != null)
                        MultipartsformData.Add(new StringContent(ID), "fid");
                    if (DestinationWorkspaceID != null)
                        MultipartsformData.Add(new StringContent(DestinationWorkspaceID), "wsid");
                    HtpReqMessage.Content = MultipartsformData;

                    HtpReqMessage.RequestUri = new pUri("upload", new AuthDictionary());
                    // ''''''''''''''''will write the whole content to H.D WHEN download completed'''''''''''''''''''''''''''''
                    using (HttpResponseMessage ResPonse = await localHttpClient.SendAsync(HtpReqMessage, HttpCompletionOption.ResponseHeadersRead, token).ConfigureAwait(false))
                    {
                        string result = await ResPonse.Content.ReadAsStringAsync();

                        token.ThrowIfCancellationRequested();
                        if (ResPonse.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            ReportCls.Report(new ReportStatus() { Finished = true, TextStatus = "Upload completed successfully" });
                            return JsonConvert.DeserializeObject<JSON_Upload>(result.Jobj().SelectToken("response[2].result[0]").ToString(), JSONhandler);
                        }
                        else
                        {
                            ReportCls.Report(new ReportStatus() { Finished = true, TextStatus = string.Format("The request returned with HTTP status code {0}", result.Jobj().SelectToken("response[1].message").ToString()) });
                            throw ExceptionCls.CreateException(result.Jobj().SelectToken("response[1].message").ToString(), (int)ResPonse.StatusCode);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ReportCls.Report(new ReportStatus() { Finished = true });
                if (ex.Message.ToString().ToLower().Contains("a task was canceled"))
                    ReportCls.Report(new ReportStatus() { TextStatus = ex.Message });
                else
                {
                    throw ExceptionCls.CreateException(ex.Message, 1001);
                }
                return null;
            }
        }
        #endregion

        #region CopyFile
        public async Task<bool> F_Copy(string DestinationFolderID)
        {
            var parameters = new AuthDictionary() { { "docid", ID }, { "folderid", DestinationFolderID } };

            using (HtpClient localHttpClient = new HtpClient(new HCHandler()))
            {
                HttpRequestMessage HtpReqMessage = new HttpRequestMessage(HttpMethod.Post, new pUri("copy"));
                HtpReqMessage.Content = new FormUrlEncodedContent(parameters);
                using (HttpResponseMessage response = await localHttpClient.SendAsync(HtpReqMessage, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false))
                {
                    var result = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                        return result.Jobj().SelectToken("response.result.message").ToString().Contains("SUCCESSFULLY");
                    else
                        throw ExceptionCls.CreateException(result.Jobj().SelectToken("response.result.message").ToString(), (int)response.StatusCode);
                }
            }
        }
        #endregion

        #region download File
        public async Task F_Download(string FileSaveDir, string FileName, string DestinationFileVersion = null, IProgress<ReportStatus> ReportCls = null, CancellationToken token = default)
        {
            if (ReportCls == null) ReportCls = new Progress<ReportStatus>();
            ReportCls.Report(new ReportStatus() { Finished = false, TextStatus = "Initializing..." });
            try
            {
                System.Net.Http.Handlers.ProgressMessageHandler progressHandler = new System.Net.Http.Handlers.ProgressMessageHandler(new HCHandler());
                progressHandler.HttpReceiveProgress += (sender, e) => {ReportCls.Report(new ReportStatus() { ProgressPercentage = e.ProgressPercentage, BytesTransferred = e.BytesTransferred, TotalBytes = e.TotalBytes ?? 0, TextStatus = "Downloading..." });};
                HtpClient localHttpClient = new HtpClient(progressHandler);
                var RequestUri = new pUri(string.Format("content/{0}", ID), new AuthDictionary() { { "docversion", DestinationFileVersion } });
                // ''''''''''''''''will write the whole content to H.D WHEN download completed'''''''''''''''''''''''''''''
                using (HttpResponseMessage ResPonse = await localHttpClient.GetAsync(RequestUri, HttpCompletionOption.ResponseContentRead, token).ConfigureAwait(false))
                {
                    if (ResPonse.IsSuccessStatusCode)
                        ReportCls.Report(new ReportStatus() { Finished = true, TextStatus = (string.Format("[{0}] Downloaded successfully.", FileName)) });
                    else
                        ReportCls.Report(new ReportStatus() { Finished = true, TextStatus = ((string.Format("Error code: {0}", ResPonse.StatusCode))) });

                    ResPonse.EnsureSuccessStatusCode();
                    var stream_ = await ResPonse.Content.ReadAsStreamAsync();
                    string FPathname = Path.Combine (FileSaveDir, FileName);
                    using (var fileStream = new FileStream(FPathname, FileMode.Append, System.IO.FileAccess.Write))
                    {
                        stream_.CopyTo(fileStream);
                    }
                }
            }
            catch (Exception ex)
            {
                ReportCls.Report(new ReportStatus() { Finished = true });
                if (ex.Message.ToString().ToLower().Contains("a task was canceled"))
                    ReportCls.Report(new ReportStatus() { TextStatus = ex.Message });
                else
                    throw ExceptionCls.CreateException(ex.Message, 1001);
            }
        }
        #endregion

        #region DownloadFileAsStream
        public async Task<System.IO.Stream> F_DownloadAsStream(IProgress<ReportStatus> ReportCls = null, CancellationToken token = default)
        {
            if (ReportCls == null) ReportCls = new Progress<ReportStatus>();
            ReportCls.Report(new ReportStatus() { Finished = false, TextStatus = "Initializing..." });
            try
            {
                System.Net.Http.Handlers.ProgressMessageHandler progressHandler = new System.Net.Http.Handlers.ProgressMessageHandler(new HCHandler());
                progressHandler.HttpReceiveProgress += (sender, e) => {ReportCls.Report(new ReportStatus() { ProgressPercentage = e.ProgressPercentage, BytesTransferred = e.BytesTransferred, TotalBytes = e.TotalBytes  ?? 0 , TextStatus = "Downloading..." });};
                HtpClient localHttpClient = new HtpClient(progressHandler);
                var RequestUri = new pUri(string.Format("content/{0}", ID), new AuthDictionary());
                // ''''''''''''''''will write the whole content to H.D WHEN download completed'''''''''''''''''''''''''''''
                HttpResponseMessage ResPonse = await localHttpClient.GetAsync(RequestUri, HttpCompletionOption.ResponseContentRead, token).ConfigureAwait(false);
                if (ResPonse.IsSuccessStatusCode)
                    ReportCls.Report(new ReportStatus() { Finished = true, TextStatus = ("File Downloaded successfully.") });
                else
                    ReportCls.Report(new ReportStatus() { Finished = true, TextStatus = ((string.Format("Error code: {0}", ResPonse.StatusCode))) });

                ResPonse.EnsureSuccessStatusCode();
                var stream_ = await ResPonse.Content.ReadAsStreamAsync();
                return stream_;
            }
            catch (Exception ex)
            {
                ReportCls.Report(new ReportStatus() { Finished = true });
                if (ex.Message.ToString().ToLower().Contains("a task was canceled"))
                    ReportCls.Report(new ReportStatus() { TextStatus = ex.Message });
                else
                    throw ExceptionCls.CreateException(ex.Message, 1001);
                return null;
            }
        }
        #endregion

        #region FileRevisionDetails
        public async Task<JSON_RevisionMetadata> F_RevisionMetadata(RevisionTypeEnum RevisionType)
        {
            var parameters = new AuthDictionary() { { "docid", ID }, { "type", RevisionType.ToString() } };

            using (HtpClient localHttpClient = new HtpClient(new HCHandler()))
            {
                HttpRequestMessage HtpReqMessage = new HttpRequestMessage(HttpMethod.Get, new pUri("revision/details", parameters)) { Content = new FormUrlEncodedContent(parameters) };
                using (HttpResponseMessage response = await localHttpClient.SendAsync(HtpReqMessage, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false))
                {
                    var result = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                        return JsonConvert.DeserializeObject<JSON_RevisionMetadata>(result, JSONhandler);
                    else
                        throw ExceptionCls.CreateException(result.Jobj().SelectToken("response[1].message").ToString(), (int)response.StatusCode);
                }
            }
        }
        #endregion

        #region AddTags
        public async Task<bool> F_AddTags(List<string> TagsNames)
        {
            var parameters = new AuthDictionary();
            parameters.Add("docid", ID);
            parameters.Add("tagname", string.Join(",", TagsNames));

            using (HtpClient localHttpClient = new HtpClient(new HCHandler()))
            {
                HttpRequestMessage HtpReqMessage = new HttpRequestMessage(HttpMethod.Post, new pUri("tags/add")) {Content = new FormUrlEncodedContent(parameters)};
                using (HttpResponseMessage response = await localHttpClient.SendAsync(HtpReqMessage, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false))
                {
                    var result = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                        return result.Jobj().SelectToken("response.result.TagDetails.message").ToString().Contains("SUCCESSFULLY");
                    else
                        throw ExceptionCls.CreateException(result.Jobj().SelectToken("response.result.TagDetails.message").ToString(), (int)response.StatusCode);
                }
            }
        }
        #endregion

        #region RemoveTags
        public async Task<bool> F_RemoveTags(List<string> TagsNames)
        {
            var parameters = new AuthDictionary
            {
                { "docid", ID },
                { "tagname", string.Join(",", TagsNames) }
            };

            using (HtpClient localHttpClient = new HtpClient(new HCHandler()))
            {
                HttpRequestMessage HtpReqMessage = new HttpRequestMessage(HttpMethod.Post, new pUri("tags/remove")) {Content = new FormUrlEncodedContent(parameters)};
                using (HttpResponseMessage response = await localHttpClient.SendAsync(HtpReqMessage, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false))
                {
                    var result = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                        return result.Jobj().SelectToken("response.result.message").ToString().Contains("SUCCESSFULLY");
                    else
                        throw ExceptionCls.CreateException(result.Jobj().SelectToken("response.result.message").ToString(), (int)response.StatusCode);
                }
            }
        }
        #endregion

        #region ListSubFilesAndFolders
        public async Task<JSON_ListFolder> D_List()
        {
            using (HtpClient localHttpClient = new HtpClient(new HCHandler()))
            {
                var RequestUri = new Uri(Convert.ToString(new pUri("folders", new AuthDictionary())) + string.Concat("&folderid=", ID));
                using (HttpResponseMessage response = await localHttpClient.GetAsync(RequestUri).ConfigureAwait(false))
                {
                    var result = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                        return JsonConvert.DeserializeObject<JSON_ListFolder>(result, JSONhandler);
                    else
                        throw ExceptionCls.CreateException(result.Jobj().SelectToken("response[1].message").ToString(), (int)response.StatusCode);
                }
            }
        }
        #endregion

    }
}
