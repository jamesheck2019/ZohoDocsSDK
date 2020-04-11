using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ZohoDocsSDK.JSON;
using static ZohoDocsSDK.Basic;
using static ZohoDocsSDK.Utilitiez;

namespace ZohoDocsSDK
{
    internal static class SharedFuncs
    {

        #region RenameFileFolder
        public static async Task<bool> Rename(string ID, string NewName)
        {
            var parameters = new AuthDictionary { { "docid", ID }, { "docname", NewName } };

            HttpResponseMessage response = await RequestAsync(HttpMethod.Post, "rename", new FormUrlEncodedContent(parameters));
            string result = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return JObject.Parse(result).SelectToken("response.result.DocumentDetails.DocumentDetail.message").ToString().Contains("SUCCESSFULLY"); // ("response")("result")("DocumentDetails")("DocumentDetail")("message").ToString().Contains("SUCCESSFULLY")
            }
            else
            {
                throw new ZohoDocsException(result.Jobj().SelectToken("response.result.DocumentDetails.DocumentDetail.message").ToString(), (int)response.StatusCode);
            }

        }
        #endregion

        #region MoveFileFolder
        public static async Task<bool> Move(string SourceID, string DestinationFolderID)
        {
            var parameters = new AuthDictionary() { { "docid", SourceID }, { "folderid", DestinationFolderID } };

            HttpResponseMessage response = await RequestAsync(HttpMethod.Post, "move", new FormUrlEncodedContent(parameters));
            string result = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return result.Jobj().SelectToken("response.result.message").ToString().Contains("SUCCESSFULLY");
            }
            else
            {
                throw new ZohoDocsException(result.Jobj().SelectToken("response.result.message").ToString(), (int)response.StatusCode);
            }
        }
        #endregion

        #region TrashFileFolder
        public static async Task<bool> Trash(string ID)
        {
            var parameters = new AuthDictionary { { "docid", ID } };
            HttpResponseMessage response = await RequestAsync(HttpMethod.Post, "trash", new FormUrlEncodedContent(parameters));
            string result = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return result.Jobj().SelectToken("response.result.message").ToString().Contains("SUCCESSFULLY");
            }
            else
            {
                throw new ZohoDocsException(result.Jobj().SelectToken("response.result.message").ToString(), (int)response.StatusCode);
            }

        }
        #endregion

        #region DeleteFileFolderPermanently
        public static async Task<bool> Delete(string ID)
        {
            var parameters = new AuthDictionary { { "docid", ID } };

            HttpResponseMessage response = await RequestAsync(HttpMethod.Post, "delete", new FormUrlEncodedContent(parameters));
            string result = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return result.Jobj().SelectToken("response.result.message").ToString().Contains("SUCCESSFULLY");
            }
            else
            {
                throw new ZohoDocsException(result.Jobj().SelectToken("response.result.message").ToString(), (int)response.StatusCode);
            }
        }
        #endregion

        #region ShareFileFolder
        public static async Task<string> Share(string ID, PermissionEnum Permission, string Password = null, DateTime ExpireDate = default)
        {
            var parameters = new AuthDictionary
            {
                { "docid", ID },
                { "visibility", "linkshare" },
                { "permission", Permission.ToString() },
                { "password", Password },
                { "expireson", ExpireDate.ToString("mm/dd/yyyy") }
            };
            HttpResponseMessage response = await RequestAsync(HttpMethod.Post, "share/visibility", new FormUrlEncodedContent(parameters));
            string result = await response.Content.ReadAsStringAsync();


            if (response.IsSuccessStatusCode)
            {
                return result.Jobj().SelectToken("response[2].result[0].permaLink").ToString();
            }
            else
            {
                throw new ZohoDocsException(result.Jobj().SelectToken("response[1].message").ToString(), (int)response.StatusCode);
            }
        }
        #endregion

        #region UnShareFileFolder
        public static async Task<bool> UnShare(string ID)
        {
            var parameters = new AuthDictionary { { "docid", ID }, { "visibility", "private" } };

            HttpResponseMessage response = await RequestAsync(HttpMethod.Post, "share/visibility", new FormUrlEncodedContent(parameters));
            string result = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return result.Jobj().SelectToken("response[1].message").ToString().Contains("SUCCESS");
            }
            else
            {
                throw new ZohoDocsException(result.Jobj().SelectToken("response[1].message").ToString(), (int)response.StatusCode);
            }
        }
        #endregion

        #region SharedDetails
        public static async Task<JSON_SharesMetadata> SharesMetadata(string ID)
        {
            var parameters = new AuthDictionary { { "docid", ID } };

            HttpResponseMessage response = await RequestAsync(HttpMethod.Post, "share/details", new FormUrlEncodedContent(parameters));
            string result = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<JSON_SharesMetadata>(result, JSONhandler);
            }
            else
            {
                throw new ZohoDocsException(result.Jobj().SelectToken("response[1].message").ToString(), (int)response.StatusCode);
            }
        }
        #endregion

        #region CreateNewFolder
        public static async Task<JSON_NewFolder> CreateNewFolder(string ID ,string FolderName)
        {
            var parameters = new AuthDictionary() { { "parentfolderid", ID }, { "foldername", FolderName } };

            HttpResponseMessage response = await RequestAsync(HttpMethod.Post, "folders/create", new FormUrlEncodedContent(parameters));
            string result = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<JSON_NewFolder>(result.Jobj().SelectToken("response.result.FolderDetails.FolderDetail").ToString(), JSONhandler);
            }
            else
            {
                throw new ZohoDocsException(result.Jobj().SelectToken("response.result.FolderDetails.FolderDetail.message").ToString(), (int)response.StatusCode);
            }
        }
        #endregion

        #region UploadLocal
        public static async Task<JSON_Upload> Upload(string ID,object FileToUpload, UploadTypes UploadType, string FileName, string DestinationWorkspaceID = null, IProgress<ReportStatus> ReportCls = null, CancellationToken token = default)
        {
            ReportCls = ReportCls ?? new Progress<ReportStatus>();
            ReportCls.Report(new ReportStatus() { Finished = false, TextStatus = "Initializing..." });
            try
            {
                System.Net.Http.Handlers.ProgressMessageHandler progressHandler = new System.Net.Http.Handlers.ProgressMessageHandler(new HCHandler());
                progressHandler.HttpSendProgress += (sender, e) => { ReportCls.Report(new ReportStatus() { ProgressPercentage = e.ProgressPercentage, BytesTransferred = e.BytesTransferred, TotalBytes = e.TotalBytes ?? 0, TextStatus = "Uploading..." }); };
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
                            streamContent = new StreamContent(new FileStream(FileToUpload.ToString(), FileMode.Open, FileAccess.Read));
                            break;
                        case UploadTypes.Stream:
                            streamContent = new StreamContent((Stream)FileToUpload);
                            break;
                        case UploadTypes.BytesArry:
                            streamContent = new StreamContent(new MemoryStream((byte[])FileToUpload));
                            break;
                        case UploadTypes.String:
                            streamContent = new StringContent(File.ReadAllText(FileToUpload.ToString()));
                            break;
                    }
                    MultipartsformData.Add(streamContent, "content", FileName);
                    MultipartsformData.Add(new StringContent(authToken), "authtoken");
                    MultipartsformData.Add(new StringContent("docsapi"), "scope");
                    MultipartsformData.Add(new StringContent(FileName), "filename");
                    if (ID != null) { MultipartsformData.Add(new StringContent(ID), "fid"); }
                    if (DestinationWorkspaceID != null) { MultipartsformData.Add(new StringContent(DestinationWorkspaceID), "wsid"); }

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
                            throw new ZohoDocsException(result.Jobj().SelectToken("response[1].message").ToString(), (int)ResPonse.StatusCode);
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
                    throw new ZohoDocsException(ex.Message, 1001);
                }
                return null;
            }
        }
        #endregion

        #region ListAllFiles
        public static async Task<List<JSON_FileMetadata>> ListAllFiles(string DestinationFolderID, CategoryEnum Filter = CategoryEnum._ALL_, int? Limit = 200, int? Offset = 0)
        {
            var parameters = new AuthDictionary
            {
                { "folderid", DestinationFolderID ?? null },
                { "category", Filter != CategoryEnum._ALL_ ? Filter.ToString() : null },
                { "start", Offset.HasValue ? Offset.Value.ToString() : null },
                { "limit", Limit.HasValue ? Limit.Value.ToString() : null }
            };

            HttpResponseMessage response = await RequestAsync(HttpMethod.Get, "files" + AsQueryString(parameters), null);
            string result = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<List<JSON_FileMetadata>>(result.Jobj().SelectToken("FILES").ToString(), JSONhandler);
            }
            else
            {
                throw new ZohoDocsException(result.Jobj().SelectToken("response[1].message").ToString(), (int)response.StatusCode);
            }
        }
        #endregion

        #region ListAllFolders
        public static async Task<List<JSON_FolderMetadata>> ListAllFolders(string DestinationFolderID, int? Limit = 200, int? Offset = 0)
        {
            var parameters = new AuthDictionary
            {
                { "folderid", DestinationFolderID ?? null },
                { "start", Offset.HasValue ? Offset.Value.ToString() : null },
                { "limit", Limit.HasValue ? Limit.Value.ToString() : null }
            };
            HttpResponseMessage response = await RequestAsync(HttpMethod.Get, "folders" + AsQueryString(parameters), null);
            string result = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<List<JSON_FolderMetadata>>(result.Jobj().SelectToken("FOLDER").ToString(), JSONhandler);
            }

            else
            {
                throw new ZohoDocsException(result.Jobj().SelectToken("response[1].message").ToString(), (int)response.StatusCode);
            }
        }
        #endregion


    }
}
