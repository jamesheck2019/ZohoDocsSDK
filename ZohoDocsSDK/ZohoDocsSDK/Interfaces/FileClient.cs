using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZohoDocsSDK.JSON;
using static ZohoDocsSDK.Basic;
using static ZohoDocsSDK.Utilitiez;

namespace ZohoDocsSDK
{
   public  class FileClient : IFile
    {
        private string FileID { get; set; }
        public FileClient(string FileID) => this.FileID = FileID;


        public async Task<bool> Rename( string NewName)
        {
            return await SharedFuncs.Rename(FileID, NewName);
        }

        public async Task<bool> Move(string DestinationFolderID)
        {
            return await SharedFuncs.Move(FileID, DestinationFolderID);
        }

        public async Task<bool> Copy(string DestinationFolderID)
        {
            var parameters = new AuthDictionary() { { "docid", FileID }, { "folderid", DestinationFolderID } };
            HttpResponseMessage response = await RequestAsync(HttpMethod.Post, "copy", new FormUrlEncodedContent(parameters));
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

        public async Task<bool> Trash()
        {
            return await SharedFuncs.Trash(FileID);
        }

        public async Task<bool> Delete()
        {
            return await SharedFuncs.Delete(FileID);
        }

        public async Task<string> Share( PermissionEnum Permission, string Password = null, DateTime ExpireDate = default)
        {
            return await SharedFuncs.Share(FileID, Permission, Password, ExpireDate);
        }

        public async Task<bool> UnShare()
        {
            return await SharedFuncs.UnShare(FileID);
        }

        public async Task<JSON_SharesMetadata> SharedDetails()
        {
            return await SharedFuncs.SharesMetadata(FileID);
        }

        public async Task<JSON_RevisionMetadata> RevisionMetadata(RevisionTypeEnum RevisionType)
        {
            var parameters = new AuthDictionary() { { "docid", FileID }, { "type", RevisionType.ToString() } };
            HttpResponseMessage response = await RequestAsync(HttpMethod.Post, "revision/details", new FormUrlEncodedContent(parameters));
            string result = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<JSON_RevisionMetadata>(result, JSONhandler);
            }
            else
            {
                throw new ZohoDocsException(result.Jobj().SelectToken("response[1].message").ToString(), (int)response.StatusCode);
            }
        }

        public async Task<bool> AddTags(string[] TagNames)
        {
            var parameters = new AuthDictionary { { "docid", FileID }, { "tagname", string.Join(",", TagNames) } };
            HttpResponseMessage response = await RequestAsync(HttpMethod.Post, "tags/add", new FormUrlEncodedContent(parameters));
            string result = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return result.Jobj().SelectToken("response.result.TagDetails.message").ToString().Contains("SUCCESSFULLY");
            }
            else
            {
                throw new ZohoDocsException(result.Jobj().SelectToken("response.result.TagDetails.message").ToString(), (int)response.StatusCode);
            }
        }

        public async Task<bool> RemoveTags(string[] TagNames)
        {
            var parameters = new AuthDictionary { { "docid", FileID }, { "tagname", string.Join(",", TagNames) } };
            HttpResponseMessage response = await RequestAsync(HttpMethod.Post, "tags/remove", new FormUrlEncodedContent(parameters));
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

        public async Task Download(string FileSaveDir, string FileName, string DestinationFileVersion = null, IProgress<ReportStatus> ReportCls = null, CancellationToken token = default)
        {
            ReportCls = ReportCls ?? new Progress<ReportStatus>();
            ReportCls.Report(new ReportStatus() { Finished = false, TextStatus = "Initializing..." });
            try
            {
                System.Net.Http.Handlers.ProgressMessageHandler progressHandler = new System.Net.Http.Handlers.ProgressMessageHandler(new HCHandler());
                progressHandler.HttpReceiveProgress += (sender, e) => { ReportCls.Report(new ReportStatus() { ProgressPercentage = e.ProgressPercentage, BytesTransferred = e.BytesTransferred, TotalBytes = e.TotalBytes ?? 0, TextStatus = "Downloading..." }); };
                HtpClient localHttpClient = new HtpClient(progressHandler);
                var RequestUri = new pUri(string.Format("content/{0}", FileID), new AuthDictionary() { { "docversion", DestinationFileVersion } });
                // ''''''''''''''''will write the whole content to H.D WHEN download completed'''''''''''''''''''''''''''''
                using (HttpResponseMessage ResPonse = await localHttpClient.GetAsync(RequestUri, HttpCompletionOption.ResponseContentRead, token).ConfigureAwait(false))
                {
                    if (ResPonse.IsSuccessStatusCode)
                    {
                        ReportCls.Report(new ReportStatus() { Finished = true, TextStatus = (string.Format("[{0}] Downloaded successfully.", FileName)) });
                    }
                    else
                    {
                        ReportCls.Report(new ReportStatus() { Finished = true, TextStatus = ((string.Format("Error code: {0}", ResPonse.StatusCode))) });
                    }

                    ResPonse.EnsureSuccessStatusCode();
                    var stream_ = await ResPonse.Content.ReadAsStreamAsync();
                    string FPathname = Path.Combine(FileSaveDir, FileName);
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
                {
                    ReportCls.Report(new ReportStatus() { TextStatus = ex.Message });
                }
                else
                {
                    throw new ZohoDocsException(ex.Message, 1001);
                }
            }
        }

        public async Task<Stream> DownloadAsStream(IProgress<ReportStatus> ReportCls = null, CancellationToken token = default)
        {
            ReportCls = ReportCls ?? new Progress<ReportStatus>();
            ReportCls.Report(new ReportStatus() { Finished = false, TextStatus = "Initializing..." });
            try
            {
                System.Net.Http.Handlers.ProgressMessageHandler progressHandler = new System.Net.Http.Handlers.ProgressMessageHandler(new HCHandler());
                progressHandler.HttpReceiveProgress += (sender, e) => { ReportCls.Report(new ReportStatus() { ProgressPercentage = e.ProgressPercentage, BytesTransferred = e.BytesTransferred, TotalBytes = e.TotalBytes ?? 0, TextStatus = "Downloading..." }); };
                HtpClient localHttpClient = new HtpClient(progressHandler);
                var RequestUri = new pUri($"content/{FileID}", new AuthDictionary());
                // ''''''''''''''''will write the whole content to H.D WHEN download completed'''''''''''''''''''''''''''''
                HttpResponseMessage ResPonse = await localHttpClient.GetAsync(RequestUri, HttpCompletionOption.ResponseContentRead, token).ConfigureAwait(false);
                if (ResPonse.IsSuccessStatusCode)
                {
                    ReportCls.Report(new ReportStatus() { Finished = true, TextStatus = ("File Downloaded successfully.") });
                }
                else
                {
                    ReportCls.Report(new ReportStatus() { Finished = true, TextStatus = ((string.Format("Error code: {0}", ResPonse.StatusCode))) });
                }

                ResPonse.EnsureSuccessStatusCode();
                var stream_ = await ResPonse.Content.ReadAsStreamAsync();
                return stream_;
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



    }
}
