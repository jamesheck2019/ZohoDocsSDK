using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ZohoDocsSDK.JSON;
using static ZohoDocsSDK.Utilitiez;

namespace ZohoDocsSDK
{
    public interface IRoot
    {

        /// <summary>
        /// get root id
        /// </summary>
        string RootID { get; set; }

        /// <summary>
        ///Creates a new folder
        ///https://apidocs.zoho.com/files/v1/folders/create?authtoken=AuthToken&scope=docsapi
        ///</summary>
        ///<param name="FolderName">Mandatory. Name of the folder in which folder to be created.</param>
        Task<JSON_NewFolder> Create(string FolderName);

        /// <summary>
        ///return root files and folders
        ///</summary>
        Task<JSON_ListFolder> List();

        /// <summary>
        ///Retrieves the user's files
        ///https://apidocs.zoho.com/files/v1/files?authtoken=AuthToken&scope=docsapi
        ///</summary>
        ///<param name="Filter">Optional. List of files based on category</param>
        ///<param name="Limit">Optional. This value is set to get the files list limit. If the both start and limit is not set, default value 0 and 200 will be taken respectively</param>
        ///<param name="Offset">Optional. This value is set to get the list of files from that entry</param>
        Task<List<JSON_FileMetadata>> ListSubFilesRecursively(CategoryEnum Filter = CategoryEnum._ALL_, int? Limit = 200, int? Offset = 0);

        /// <summary>
        ///Returns the list of folders present in user's account
        ///https://apidocs.zoho.com/files/v1/folders?authtoken=AuthToken&scope=docsapi
        ///</summary>
        ///<param name="Limit">Optional. This value is set to get the files list limit. If the both start and limit is not set, default value 0 and 200 will be taken respectively</param>
        ///<param name="Offset">Optional. This value is set to get the list of files from that entry</param>
        Task<List<JSON_FolderMetadata>> ListSubFoldersRecursively(int? Limit = 200, int? Offset = 0);

        /// <summary>
        ///Uploads a file
        ///https://apidocs.zoho.com/files/v1/upload?authtoken=AuthToken&scope=docsapi
        ///</summary>
        ///<param name="FileToUpload">file to upload</param>
        ///<param name="UploadType">the file object definition path or stream or bytes array</param>
        ///<param name="FileName">the object file name </param>
        ///<param name="DestinationWorkspaceID">Optional. id of the workspace where files to be uploaded to a workspace</param>
        ///<param name="ReportCls">IProgress</param>
        ///<param name="token">Cancellation Token</param>
        Task<JSON_Upload> Upload(object FileToUpload, Utilitiez.UploadTypes UploadType, string FileName, string DestinationWorkspaceID = null, IProgress<ReportStatus> ReportCls = null, CancellationToken token = default);
    }
}