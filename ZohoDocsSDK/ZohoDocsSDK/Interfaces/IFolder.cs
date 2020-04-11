using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ZohoDocsSDK.JSON;

namespace ZohoDocsSDK
{
    public interface IFolder
    {

        /// <summary>
        ///Copies the existing folder to a specified folder
        ///https://apidocs.zoho.com/files/v1/folders/copy?authtoken=AuthToken&scope=docsapi
        ///</summary>
        ///<param name="DestinationFolderID">Mandatory. Specifies I.D of the destination folder (where existing folder copies).</param>
        Task<bool> Copy(string DestinationFolderID);

        /// <summary>
        ///Creates a new folder
        ///https://apidocs.zoho.com/files/v1/folders/create?authtoken=AuthToken&scope=docsapi
        ///</summary>
        ///<param name="FolderName">Mandatory. Name of the folder in which folder to be created.</param>
        Task<JSON_NewFolder> Create(string FolderName);

        /// <summary>
        ///folder will be removed from user's account
        ///https://apidocs.zoho.com/files/v1/delete?authtoken=AuthToken&scope=docsapi
        ///</summary>
        Task<bool> Delete();

        /// <summary>
        ///Returns the list of files and folders
        ///</summary>
        Task<JSON_ListFolder> List();

        /// <summary>
        ///Retrieves the user's files and folders
        ///</summary>
        ///<param name="DestinationFolderID">Optional. Id of the folder to get sub folder list. This takes default as root folder</param>
        ///<param name="Limit">Optional. This value is set to get the files list limit. If the both start and limit is not set, default value 0 and 200 will be taken respectively</param>
        ///<param name="Offset">Optional. This value is set to get the list of files from that entry</param>
        Task<JSON_ListFolder> List2( int? Limit = 200, int? Offset = 0);

        /// <summary>
        ///Retrieves the user's files
        ///https://apidocs.zoho.com/files/v1/files?authtoken=AuthToken&scope=docsapi
        ///</summary>
        ///<param name="Filter">Optional. List of files based on category</param>
        ///<param name="Limit">Optional. This value is set to get the files list limit. If the both start and limit is not set, default value 0 and 200 will be taken respectively</param>
        ///<param name="Offset">Optional. This value is set to get the list of files from that entry</param>
        Task<List<JSON_FileMetadata>> ListSubFilesRecursively(Utilitiez.CategoryEnum Filter = Utilitiez.CategoryEnum._ALL_, int? Limit = 200, int? Offset = 0);

        /// <summary>
        ///Returns the list of folders present in user's account
        ///https://apidocs.zoho.com/files/v1/folders?authtoken=AuthToken&scope=docsapi
        ///</summary>
        ///<param name="Limit">Optional. This value is set to get the files list limit. If the both start and limit is not set, default value 0 and 200 will be taken respectively</param>
        ///<param name="Offset">Optional. This value is set to get the list of files from that entry</param>
        Task<List<JSON_FolderMetadata>> ListSubFoldersRecursively(int? Limit = 200, int? Offset = 0);

        /// <summary>
        ///Moves the existing folder to specified location
        ///https://apidocs.zoho.com/files/v1/folders/move?authtoken=AuthToken&scope=docsapi
        ///</summary>
        ///<param name="DestinationFolderID">Mandatory. Id of the folder to be moved</param>
        Task<bool> Move(string DestinationFolderID);

        /// <summary>
        ///Renames the existing folder
        ///https://apidocs.zoho.com/files/v1/folders/rename?authtoken=AuthToken&scope=docsapi
        ///</summary>
        ///<param name="NewName">Mandatory. New name for the file/folder</param>
        Task<bool> Rename(string NewName);

        /// <summary>
        ///Sharing a folder via link
        ///https://apidocs.zoho.com/files/v1/share/visibility?authtoken=AuthToken&scope=docsapi
        ///</summary>
        ///<param name="Permission">Mandatory. file shared to the user with the specified permission</param>
        ///<param name="Password">Optional. password required if the doc is shared via link share with secured</param>
        ///<param name="ExpireDate">Optional. specify the date till the document can be shared via link share</param>
        Task<string> Share(Utilitiez.PermissionEnum Permission, string Password = null, DateTime ExpireDate = default);

        /// <summary>
        ///Returns the shared details of a folder
        ///https://apidocs.zoho.com/files/v1/share/details?authtoken=AuthToken&scope=docsapi
        ///</summary>
        Task<JSON_SharesMetadata> SharedDetails();

        /// <summary>
        ///Moves the folder to trash
        ///https://apidocs.zoho.com/files/v1/trash?authtoken=AuthToken&scope=docsapi
        ///</summary>
        Task<bool> Trash();

        /// <summary>
        ///unShare a folder
        ///https://apidocs.zoho.com/files/v1/share/visibility?authtoken=AuthToken&scope=docsapi
        ///</summary>
        Task<bool> UnShare();

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