using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ZohoDocsSDK.JSON;
using static ZohoDocsSDK.Basic;
using static ZohoDocsSDK.Utilitiez;

namespace ZohoDocsSDK
{
    public class FolderClient : IFolder
    {

        private string FolderID { get; set; }
        public FolderClient(string FolderID) => this.FolderID = FolderID;


        public async Task<bool> Rename(string NewName)
        {
            return await SharedFuncs.Rename(FolderID, NewName);
        }

        public async Task<bool> Move(string DestinationFolderID)
        {
            return await SharedFuncs.Move(FolderID, DestinationFolderID);
        }

        public async Task<bool> Copy(string DestinationFolderID)
        {
            var parameters = new AuthDictionary() { { "folderid", FolderID }, { "destfolderid", DestinationFolderID } };
            HttpResponseMessage response = await RequestAsync(HttpMethod.Post, "folders/copy", new FormUrlEncodedContent(parameters));
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
            return await SharedFuncs.Trash(FolderID);
        }

        public async Task<bool> Delete()
        {
            return await SharedFuncs.Delete(FolderID);
        }

        public async Task<string> Share(PermissionEnum Permission, string Password = null, DateTime ExpireDate = default)
        {
            return await SharedFuncs.Share(FolderID, Permission, Password, ExpireDate);
        }

        public async Task<bool> UnShare()
        {
            return await SharedFuncs.UnShare(FolderID);
        }

        public async Task<JSON_SharesMetadata> SharedDetails()
        {
            return await SharedFuncs.SharesMetadata(FolderID);
        }

        public async Task<JSON_NewFolder> Create(string FolderName)
        {
            return await SharedFuncs.CreateNewFolder(FolderID, FolderName);
        }

        public async Task<JSON_Upload> Upload(object FileToUpload, UploadTypes UploadType, string FileName, string DestinationWorkspaceID = null, IProgress<ReportStatus> ReportCls = null, CancellationToken token = default)
        {
            return await SharedFuncs.Upload(FolderID, FileToUpload, UploadType, FileName, DestinationWorkspaceID, ReportCls, token);
        }

        #region ListSubFilesAndFolders
        public async Task<JSON_ListFolder> List()
        {
            HttpResponseMessage response = await RequestAsync(HttpMethod.Get, "folders" + AsQueryString(new AuthDictionary()) + $"&folderid={FolderID}", null);
            string result = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<JSON_ListFolder>(result, JSONhandler);
            }
            else
            {
                throw new ZohoDocsException(result.Jobj().SelectToken("response[1].message").ToString(), (int)response.StatusCode);
            }
        }
        #endregion


        #region ListFilesAndFolders
        public async Task<JSON_ListFolder> List2( int? Limit = 200, int? Offset = 0)
        {
            var parameters = new AuthDictionary
            {
                { "folderid", FolderID ?? null },
                { "start", Offset.HasValue ? Offset.Value.ToString() : null },
                { "limit", Limit.HasValue ? Limit.Value.ToString() : null }
            };
            HttpResponseMessage response = await RequestAsync(HttpMethod.Get, "folders/files" + AsQueryString(parameters), null);
            string result = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<JSON_ListFolder>(result, JSONhandler);
            }
            else
            {
                throw new ZohoDocsException(result.Jobj().SelectToken("response[1].message").ToString(), (int)response.StatusCode);
            }
        }
        #endregion

        public async Task<List<JSON_FileMetadata>> ListSubFilesRecursively(CategoryEnum Filter = CategoryEnum._ALL_, int? Limit = 200, int? Offset = 0)
        {
            return await SharedFuncs.ListAllFiles(FolderID, Filter, Limit, Offset);
        }

        public async Task<List<JSON_FolderMetadata>> ListSubFoldersRecursively(int? Limit = 200, int? Offset = 0)
        {
            return await SharedFuncs.ListAllFolders(FolderID, Limit, Offset);
        }


    }
}
