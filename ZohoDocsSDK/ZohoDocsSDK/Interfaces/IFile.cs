using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ZohoDocsSDK.JSON;

namespace ZohoDocsSDK
{
    public interface IFile
    {

        /// <summary>
        ///Adds a tag to a file or a document
        ///https://apidocs.zoho.com/files/v1/tags/add?authtoken=AuthToken&scope=docsapi
        ///</summary>
        ///<param name="TagsNames">Mandatory. Name of the tag to be added</param>
        Task<bool> AddTags(string[] TagNames);

        /// <summary>
        ///Copies a file or document to a new location
        ///https://apidocs.zoho.com/files/v1/copy?authtoken=AuthToken&scope=docsapi
        ///</summary>
        ///<param name="DestinationFolderID">Mandatory. Id of folder to copy the new file</param>
        Task<bool> Copy(string DestinationFolderID);

        /// <summary>
        ///file will be removed from user's account
        ///https://apidocs.zoho.com/files/v1/delete?authtoken=AuthToken&scope=docsapi
        ///</summary>
        Task<bool> Delete();

        /// <summary>
        ///Downloads a file
        ///https://apidocs.zoho.com/files/v1/content/<DOC_ID>?authtoken=AuthToken&scope=docsapi
        ///</summary>
        ///<param name="FileSaveDir">"D:\\Downloads"</param>
        ///<param name="FileName">file.rar</param>
        ///<param name="DestinationFileVersion">Optional. Version of the document to be downloaded. This defaults to recent one.</param>
        ///<param name="ReportCls">IProgress</param>
        ///<param name="token">Cancellation Token</param>
        Task Download(string FileSaveDir, string FileName, string DestinationFileVersion = null, IProgress<ReportStatus> ReportCls = null, CancellationToken token = default);

        /// <summary>
        ///Downloads a file as IO.Stream
        ///https://apidocs.zoho.com/files/v1/content/<DOC_ID>?authtoken=AuthToken&scope=docsapi
        ///</summary>
        ///<param name="ReportCls">IProgress</param>
        ///<param name="token">Cancellation Token</param>
        Task<Stream> DownloadAsStream(IProgress<ReportStatus> ReportCls = null, CancellationToken token = default);

        /// <summary>
        ///Moves the existing file to specified location
        ///https://apidocs.zoho.com/files/v1/move?authtoken=AuthToken&scope=docsapi
        ///</summary>
        ///<param name="DestinationFolderID">Mandatory. Id of the folder to be moved</param>
        Task<bool> Move(string DestinationFolderID);

        /// <summary>
        ///Removes the tag which is mapped to a document or file
        ///https://apidocs.zoho.com/files/v1/tags/remove?authtoken=AuthToken&scope=docsapi
        ///</summary>
        ///<param name="TagsNames">Mandatory. Name of the tag to be removed from file or document</param>
        Task<bool> RemoveTags(string[] TagNames);

        /// <summary>
        ///Renames the existing file
        ///https://apidocs.zoho.com/files/v1/rename?authtoken=AuthToken&scope=docsapi
        ///</summary>
        ///<param name="NewName">Mandatory. New name for the file/folder</param>
        Task<bool> Rename(string NewName);

        /// <summary>
        ///Lists the revision details for the given document
        ///https://apidocs.zoho.com/files/v1/revision/details?authtoken=AuthToken&scope=docsapi
        ///</summary>
        ///<param name="RevisionType">Mandatory. specify which type of document</param>
        Task<JSON_RevisionMetadata> RevisionMetadata(Utilitiez.RevisionTypeEnum RevisionType);

        /// <summary>
        ///Sharing a file via link
        ///https://apidocs.zoho.com/files/v1/share/visibility?authtoken=AuthToken&scope=docsapi
        ///</summary>
        ///<param name="Permission">Mandatory. file shared to the user with the specified permission</param>
        ///<param name="Password">Optional. password required if the doc is shared via link share with secured</param>
        ///<param name="ExpireDate">Optional. specify the date till the document can be shared via link share</param>
        Task<string> Share(Utilitiez.PermissionEnum Permission, string Password = null, DateTime ExpireDate = default);

        /// <summary>
        ///Returns the shared details of a file
        ///https://apidocs.zoho.com/files/v1/share/details?authtoken=AuthToken&scope=docsapi
        ///</summary>
        Task<JSON_SharesMetadata> SharedDetails();

        /// <summary>
        ///Moves the file to trash
        ///https://apidocs.zoho.com/files/v1/trash?authtoken=AuthToken&scope=docsapi
        ///</summary>
        Task<bool> Trash();

        /// <summary>
        ///unShare a file
        ///https://apidocs.zoho.com/files/v1/share/visibility?authtoken=AuthToken&scope=docsapi
        ///</summary>
        Task<bool> UnShare();


    }
}