Imports DeQma.ZohoDocsSDK.File
Imports DeQma.ZohoDocsSDK.JSON
Imports DeQma.ZohoDocsSDK.ZhOutilities

Public Interface IFile

    Function TrashFile(SourceFileID As String) As Task(Of JSON_TrashFile)
    Function RenameFile(SourceFileID As String, NewFileName As String) As Task(Of JSON_RenameFile)
    Function MoveFile(SourceFileID As String, DestinationFolderID As String) As Task(Of JSON_MoveFile)
    Function CopyFile(SourceFileID As String, DestinationFolderID As String) As Task(Of JSON_CopyFile)
    Function CopyMultipleFiles(SourceFilesIDs As List(Of String), DestinationFolderID As String) As Task(Of JSON_CopyMultipleFiles)
    Function DownloadFile(FileID As String, FileSaveDir As String, FileName As String, Optional DestinationFileVersion As String = Nothing, Optional ReportCls As IProgress(Of ReportStatus) = Nothing, Optional _proxi As ProxyConfig = Nothing, Optional TimeOut As Integer = 60, Optional token As Threading.CancellationToken = Nothing) As Task
    Function DownloadFileAsStream(FileID As String, Optional ReportCls As IProgress(Of ReportStatus) = Nothing, Optional _proxi As ProxyConfig = Nothing, Optional TimeOut As Integer = 60, Optional token As Threading.CancellationToken = Nothing) As Task(Of IO.Stream)
    Function UploadLocal(FileToUpload As Object, FileName As String, UploadType As UploadTypes, Optional DestinationFolderID As String = Nothing, Optional DestinationWorkspaceID As String = Nothing, Optional ReportCls As IProgress(Of ReportStatus) = Nothing, Optional _proxi As ProxyConfig = Nothing, Optional token As Threading.CancellationToken = Nothing) As Task(Of JSON_UploadLocal)
    Function DeleteFile(SourceFileID As String) As Task(Of JSON_DeleteFile)
    Function MoveMultipleFiles(SourceFilesIDs As List(Of String), DestinationFolderID As String) As Task(Of JSON_MoveMultipleFiles)
    Function FileRevisionDetails(FileID As String, RevisionType As RevisionType) As Task(Of JSON_FileRevisionDetails)


End Interface
