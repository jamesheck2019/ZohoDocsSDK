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
    public class RootClient : IRoot
    {

        public string RootID { get; set; } = "1";

        public async Task<JSON_NewFolder> Create(string FolderName)
        {
            return await SharedFuncs.CreateNewFolder(RootID, FolderName);
        }

        public async Task<JSON_Upload> Upload(object FileToUpload, UploadTypes UploadType, string FileName, string DestinationWorkspaceID = null, IProgress<ReportStatus> ReportCls = null, CancellationToken token = default)
        {
            return await SharedFuncs.Upload(null, FileToUpload, UploadType, FileName, DestinationWorkspaceID, ReportCls, token);
        }

        #region ListRootFilesFolders
        public async Task<JSON_ListFolder> List()
        {
            HttpResponseMessage response = await RequestAsync(HttpMethod.Get, "folders/files" + AsQueryString(new AuthDictionary()), null);
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
            return await SharedFuncs.ListAllFiles(null, Filter, Limit, Offset);
        }

        public async Task<List<JSON_FolderMetadata>> ListSubFoldersRecursively( int? Limit = 200, int? Offset = 0)
        {
            return await SharedFuncs.ListAllFolders(null, Limit, Offset);
        }

    }
}
