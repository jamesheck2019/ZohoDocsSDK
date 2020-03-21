using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ZohoDocsSDK;
using ZohoDocsSDK.JSON;
using static ZohoDocsSDK.Utilitiez;

namespace ZohoDocsSDK
{
    public interface IClient
    {

        /// <summary>
        ///multiple
        ///     '''[D_] = dir
        // ' [F_] = file
        // ' [FD_] = file & dir
        /// </summary>
        ///<param name="IDs">list of files or folders ids</param>
        IItems Items(List<string> IDs);
        /// <summary>
        ///single
        ///     '''[D_] = dir
        // ' [F_] = file
        // ' [FD_] = file & dir
        /// </summary>
        ///<param name="ID">file or folder id</param>
        IItem Item(string ID);




        /// <summary>
        ///Retrieves the user's files
        ///https://apidocs.zoho.com/files/v1/files?authtoken=AuthToken&scope=docsapi
        ///</summary>
        ///<param name="DestinationFolderID">Optional. Id of the folder to get sub folder list. This takes default as root folder</param>
        ///<param name="Filter">Optional. List of files based on category</param>
        ///<param name="Limit">Optional. This value is set to get the files list limit. If the both start and limit is not set, default value 0 and 200 will be taken respectively</param>
        ///<param name="Offset">Optional. This value is set to get the list of files from that entry</param>
        Task<List<JSON_FileMetadata>> ListAllFiles(string DestinationFolderID, CategoryEnum Filter = CategoryEnum._ALL_, int? Limit = 200, int? Offset = 0);
        /// <summary>
        ///return root files and folders
        ///</summary>
        Task<JSON_ListFolder> ListRoot();
        /// <summary>
        ///list public folder contents
        ///</summary>
        ///<param name="PublicFolderUrl">https://docs.zoho.com/folder/xxxxxxxxxxxxxx</param>
        Task<JSON_ListPublicLink> ListPublicFolder(Uri PublicFolderUrl);
        /// <summary>
        ///download public file
        ///</summary>
        ///<param name="FilePublicUrl">https://docs.zoho.com/file/xxxxxxxxxxxxxx</param>
        ///<param name="FileSaveDir">D:\Downloads</param>
        ///<param name="FileName">file.rar</param>
        ///<param name="ReportCls">IProgress</param>
        ///<param name="token">Threading Cancellation Token</param>
        Task DownloadPublicFile(string FilePublicUrl, string FileSaveDir, string FileName, IProgress<ReportStatus> ReportCls = null, CancellationToken token = default);
        /// <summary>
        ///Returns the list of user tags
        ///https://apidocs.zoho.com/files/v1/tags?authtoken=AuthToken&scope=docsapi
        ///</summary>
        Task<List<JSON_TagMetadata>> ListTags();
        /// <summary>
        ///get root id
        ///</summary>
        string RootID();
        /// <summary>
        ///Returns the list of folders present in user's account
        ///https://apidocs.zoho.com/files/v1/folders?authtoken=AuthToken&scope=docsapi
        ///</summary>
        ///<param name="DestinationFolderID">Optional. Id of the folder to get sub folder list. This takes default as root folder</param>
        ///<param name="Limit">Optional. This value is set to get the files list limit. If the both start and limit is not set, default value 0 and 200 will be taken respectively</param>
        ///<param name="Offset">Optional. This value is set to get the list of files from that entry</param>
        Task<List<JSON_FolderMetadata>> ListAllFolders(string DestinationFolderID, int? Limit = 200, int? Offset = 0);
        /// <summary>
        ///Retrieves the user's files and folders
        ///</summary>
        ///<param name="DestinationFolderID">Optional. Id of the folder to get sub folder list. This takes default as root folder</param>
        ///<param name="Limit">Optional. This value is set to get the files list limit. If the both start and limit is not set, default value 0 and 200 will be taken respectively</param>
        ///<param name="Offset">Optional. This value is set to get the list of files from that entry</param>
        Task<JSON_ListFolder> ListFilesAndFolders(string DestinationFolderID, int? Limit = 200, int? Offset = 0);
    }
}
