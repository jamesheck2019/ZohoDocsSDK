using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZohoDocsSDK;
using ZohoDocsSDK.JSON;

namespace ZohoDocsSDK
{
   public  interface IItem
    {

        /// <summary>
        ///Renames the existing file/folder
        ///https://apidocs.zoho.com/files/v1/folders/rename?authtoken=AuthToken&scope=docsapi
        ///https://apidocs.zoho.com/files/v1/rename?authtoken=AuthToken&scope=docsapi
        ///</summary>
        ///<param name="NewName">Mandatory. New name for the file/folder</param>
        Task<bool> FD_Rename(string NewName);
        /// <summary>
        ///Moves the existing file/folder to specified location
        ///https://apidocs.zoho.com/files/v1/folders/move?authtoken=AuthToken&scope=docsapi
        ///https://apidocs.zoho.com/files/v1/move?authtoken=AuthToken&scope=docsapi
        ///</summary>
        ///<param name="DestinationFolderID">Mandatory. Id of the folder to be moved</param>
        Task<bool> FD_Move(string DestinationFolderID);
        /// <summary>
        ///Moves the file/folder to trash
        ///https://apidocs.zoho.com/files/v1/trash?authtoken=AuthToken&scope=docsapi
        ///</summary>
        Task<bool> FD_Trash();
        /// <summary>
        ///file/folder will be removed from user's account
        ///https://apidocs.zoho.com/files/v1/delete?authtoken=AuthToken&scope=docsapi
        ///</summary>
        Task<bool> FD_Delete();
        /// <summary>
        ///Sharing a file/folder via link
        ///https://apidocs.zoho.com/files/v1/share/visibility?authtoken=AuthToken&scope=docsapi
        ///</summary>
        ///<param name="Permission">Mandatory. file shared to the user with the specified permission</param>
        ///<param name="Password">Optional. password required if the doc is shared via link share with secured</param>
        ///<param name="ExpireDate">Optional. specify the date till the document can be shared via link share</param>
        Task<string> FD_Share(Utilitiez.PermissionEnum Permission, string Password = null, DateTime ExpireDate = default);
        /// <summary>
        ///unShare a file/folder
        ///https://apidocs.zoho.com/files/v1/share/visibility?authtoken=AuthToken&scope=docsapi
        ///</summary>
        Task<bool> FD_UnShare();
        /// <summary>
        ///Returns the shared details of a file/folder
        ///https://apidocs.zoho.com/files/v1/share/details?authtoken=AuthToken&scope=docsapi
        ///</summary>
        Task<JSON_SharesMetadata> FD_SharesMetadata();
        /// <summary>
        ///Copies the existing folder to a specified folder
        ///https://apidocs.zoho.com/files/v1/folders/copy?authtoken=AuthToken&scope=docsapi
        ///</summary>
        ///<param name="DestinationFolderID">Mandatory. Specifies I.D of the destination folder (where existing folder copies).</param>
        Task<bool> D_Copy(string DestinationFolderID);
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
        Task<JSON_Upload> D_Upload(object FileToUpload, Utilitiez.UploadTypes UploadType, string FileName, string DestinationWorkspaceID = null, IProgress<ReportStatus> ReportCls = null, CancellationToken token = default);
        /// <summary>
        ///Creates a new folder
        ///https://apidocs.zoho.com/files/v1/folders/create?authtoken=AuthToken&scope=docsapi
        ///</summary>
        ///<param name="FolderName">Mandatory. Name of the folder in which folder to be created.</param>
        Task<JSON_NewFolder> D_Create(string FolderName);
        /// <summary>
        ///Returns the list of files and folders
        ///</summary>
        Task<JSON_ListFolder> D_List();
        /// <summary>
        ///Copies a file or document to a new location
        ///https://apidocs.zoho.com/files/v1/copy?authtoken=AuthToken&scope=docsapi
        ///</summary>
        ///<param name="DestinationFolderID">Mandatory. Id of folder to copy the new file</param>
        Task<bool> F_Copy(string DestinationFolderID);
        /// <summary>
        ///Downloads a file
        ///https://apidocs.zoho.com/files/v1/content/<DOC_ID>?authtoken=AuthToken&scope=docsapi
        ///</summary>
        ///<param name="FileSaveDir">"D:\\Downloads"</param>
        ///<param name="FileName">file.rar</param>
        ///<param name="DestinationFileVersion">Optional. Version of the document to be downloaded. This defaults to recent one.</param>
        ///<param name="ReportCls">IProgress</param>
        ///<param name="token">Cancellation Token</param>
        Task F_Download(string FileSaveDir, string FileName, string DestinationFileVersion = null, IProgress<ReportStatus> ReportCls = null, CancellationToken token = default);
        /// <summary>
        ///Downloads a file as IO.Stream
        ///https://apidocs.zoho.com/files/v1/content/<DOC_ID>?authtoken=AuthToken&scope=docsapi
        ///</summary>
        ///<param name="ReportCls">IProgress</param>
        ///<param name="token">Cancellation Token</param>
        Task<System.IO.Stream> F_DownloadAsStream(IProgress<ReportStatus> ReportCls = null, CancellationToken token = default);
        /// <summary>
        ///Lists the revision details for the given document
        ///https://apidocs.zoho.com/files/v1/revision/details?authtoken=AuthToken&scope=docsapi
        ///</summary>
        ///<param name="RevisionType">Mandatory. specify which type of document</param>
        Task<JSON_RevisionMetadata> F_RevisionMetadata(Utilitiez.RevisionTypeEnum RevisionType);
        /// <summary>
        ///Adds a tag to a file or a document
        ///https://apidocs.zoho.com/files/v1/tags/add?authtoken=AuthToken&scope=docsapi
        ///</summary>
        ///<param name="TagsNames">Mandatory. Name of the tag to be added</param>
        Task<bool> F_AddTags(List<string> TagsNames);
        /// <summary>
        ///Removes the tag which is mapped to a document or file
        ///https://apidocs.zoho.com/files/v1/tags/remove?authtoken=AuthToken&scope=docsapi
        ///</summary>
        ///<param name="TagsNames">Mandatory. Name of the tag to be removed from file or document</param>
        Task<bool> F_RemoveTags(List<string> TagsNames);

        //Uri TestUrl();


    }
}
