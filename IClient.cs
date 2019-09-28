using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ZohoDocsSDK.JSON;

namespace ZohoDocsSDK
{
	public interface IClient
	{
		Task<bool> TrashFileFolder(string DestinationID);

		Task<bool> DeleteFileFolder(string DestinationID);

		Task<bool> RenameFileFolder(string DestinationID, string NewFileName);

		Task<bool> MoveFileFolder(string SourceID, string DestinationFolderID);

		Task<bool> MoveMultipleFileFolder(List<string> SourceIDs, string DestinationFolderID);

		Task<bool> CopyFile(string SourceID, string DestinationFolderID);

		Task<bool> CopyMultipleFile(List<string> SourceIDs, string DestinationFolderID);

		Task<bool> CopyFolder(string SourceID, string DestinationFolderID);

		Task<bool> CopyMultipleFolder(List<string> SourceIDs, string DestinationFolderID);

		Task DownloadFile(string FileID, string FileSaveDir, string FileName, string DestinationFileVersion = null, IProgress<ReportStatus> ReportCls = null, int TimeOut = 60, CancellationToken token = default(CancellationToken));

		Task<Stream> DownloadFileAsStream(string FileID, IProgress<ReportStatus> ReportCls = null, int TimeOut = 60, CancellationToken token = default(CancellationToken));

		Task<JSON_Upload> Upload(object FileToUpload, string FileName, ZClient.UploadTypes UploadType, string DestinationFolderID = null, string DestinationWorkspaceID = null, IProgress<ReportStatus> ReportCls = null, CancellationToken token = default(CancellationToken));

		Task<JSON_FileRevisionDetails> FileRevisionDetails(string FileID, utilities.RevisionType RevisionType);

		Task<JSON_CreateNewFolder> CreateNewFolder(string FolderName, string DestinationFolderID = null);

		Task<JSON_FilesMetadata> ListAllFiles(utilities.Category Category = utilities.Category._ALL_, string Offset = null, string Limit = null);

		Task<JSON_FoldersMetadata> ListAllFolders(string DestinationFolderID);

		Task<JSON_FilesFoldersMetadata> List(string DestinationFolderID);

		Task<JSON_FilesFoldersMetadata> List2(string DestinationFolderID);

		Task<JSON_FilesFoldersMetadata> ListRootFilesFolders();

		Task<string> PublicFileFolder(string DestinationID, utilities.PPermission Permission, string Password = null, string ExpireDate = null);

		Task<bool> UnPublicFileFolder(string DestinationID);

		Task<JSON_SharedDetails> SharedDetails(string DestinationID);

		Task<JSON_ListPublicLink> ListPublicLink(string PublicLink);

		Task DownloadPublicFile(string FilePublicUrl, string FileSaveDir, string FileName, IProgress<ReportStatus> ReportCls = null, int TimeOut = 60, CancellationToken token = default(CancellationToken));

		Task<JSON_ListTags> ListTags();

		Task<bool> AddFileTag(string FileID, List<string> TagsNames);

		Task<bool> RemoveFileTag(string FileID, List<string> TagsNames);
	}
}
