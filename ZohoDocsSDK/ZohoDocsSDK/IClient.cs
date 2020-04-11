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


        IRoot Root();
        IFile File(string FileID);
        IFolder Folder(string FolderID);
        IFiles Files(string[] FilesID);
        IFolders Folders(string[] FoldersID);



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




    }
}
