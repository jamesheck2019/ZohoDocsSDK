Imports DeQma.ZohoDocsSDK.ZhOutilities
Imports DeQma.ZohoDocsSDK.JSON

Public Interface ISharing


    Function UnShareFile(FileID As String, ShareWithEmails As List(Of String)) As Task(Of JSON_UnShareFile)
    Function UnShareFolder(FolderID As String, ShareWithEmails As List(Of String)) As Task(Of JSON_UnShareFolder)
    Function SharedFileDetails(FileID As String) As Task(Of JSON_SharedDetails)
    Function SharedFolderDetails(FolderID As String) As Task(Of JSON_SharedDetails)
    Function PublicFile(FileID As String, Permission As PPermission, Optional Password As String = Nothing, Optional ExpireDate As String = Nothing) As Task(Of JSON_PublicFileFolder)
    Function UnPublicFile(FileID As String) As Task(Of JSON_PublicFileFolder)

    Function PublicFolder(FolderID As String, Permission As PPermission, Optional Password As String = Nothing, Optional ExpireDate As String = Nothing) As Task(Of JSON_PublicFileFolder)
    Function UnPublicFolder(FolderID As String) As Task(Of JSON_PublicFileFolder)
    Function ShareFolders(FolderIDs As List(Of String), ShareWithEmails As List(Of String), Permission As SPermission, Optional notify As Boolean = False, Optional Message As String = Nothing) As Task(Of JSON_ShareFolders)
    Function ShareFiles(FileIDs As List(Of String), ShareWithEmails As List(Of String), Permission As SPermission, Optional notify As Boolean = False, Optional Message As String = Nothing) As Task(Of JSON_ShareFiles)

    Function ListPublicLink(PublicLink As String) As Task(Of JSON_ListPublicLink)
    Function DownloadPublicFile(FilePublicUrl As String, FileSaveDir As String, FileName As String, Optional ReportCls As IProgress(Of ReportStatus) = Nothing, Optional _proxi As ProxyConfig = Nothing, Optional TimeOut As Integer = 60, Optional token As Threading.CancellationToken = Nothing) As Task

End Interface
