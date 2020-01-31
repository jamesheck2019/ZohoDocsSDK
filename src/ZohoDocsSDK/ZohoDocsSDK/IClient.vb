Imports ZohoDocsSDK
Imports ZohoDocsSDK.JSON
Imports ZohoDocsSDK.utilitiez
Imports ZohoDocsSDK.ZClient

Public Interface IClient

    ''' <summary>
    ''' multiple
    '''[D_] = dir
    '' [F_] = file
    '' [FD_] = file & dir
    ''' </summary>
    ''' <param name="IDs">list of files or folders ids</param>
    ReadOnly Property Items(IDs As List(Of String)) As IItems
    ''' <summary>
    ''' single
    '''[D_] = dir
    '' [F_] = file
    '' [FD_] = file & dir
    ''' </summary>
    ''' <param name="ID">file or folder id</param>
    ReadOnly Property Item(ID As String) As IItem




    ''' <summary>
    ''' Retrieves the user's files
    ''' https://apidocs.zoho.com/files/v1/files?authtoken=AuthToken&scope=docsapi
    ''' </summary>
    ''' <param name="DestinationFolderID">Optional. Id of the folder to get sub folder list. This takes default as root folder</param>
    ''' <param name="Filter">Optional. List of files based on category</param>
    ''' <param name="Limit">Optional. This value is set to get the files list limit. If the both start and limit is not set, default value 0 and 200 will be taken respectively</param>
    ''' <param name="Offset">Optional. This value is set to get the list of files from that entry</param>
    Function ListAllFiles(DestinationFolderID As String, Optional Filter As CategoryEnum = CategoryEnum._ALL_, Optional Limit As Integer? = 200, Optional Offset As Integer? = 0) As Task(Of List(Of JSON_FileMetadata))
    ''' <summary>
    ''' return root files and folders
    ''' </summary>
    Function ListRoot() As Task(Of JSON_ListFolder)
    ''' <summary>
    ''' list public folder contents
    ''' </summary>
    ''' <param name="PublicFolderUrl">https://docs.zoho.com/folder/xxxxxxxxxxxxxx</param>
    Function ListPublicFolder(PublicFolderUrl As Uri) As Task(Of JSON_ListPublicLink)
    ''' <summary>
    ''' download public file
    ''' </summary>
    ''' <param name="FilePublicUrl">https://docs.zoho.com/file/xxxxxxxxxxxxxx</param>
    ''' <param name="FileSaveDir">D:\Downloads</param>
    ''' <param name="FileName">file.rar</param>
    ''' <param name="ReportCls">IProgress</param>
    ''' <param name="token">Threading Cancellation Token</param>
    Function DownloadPublicFile(FilePublicUrl As String, FileSaveDir As String, FileName As String, Optional ReportCls As IProgress(Of ReportStatus) = Nothing, Optional token As Threading.CancellationToken = Nothing) As Task
    ''' <summary>
    ''' Returns the list of user tags
    ''' https://apidocs.zoho.com/files/v1/tags?authtoken=AuthToken&scope=docsapi
    ''' </summary>
    Function ListTags() As Task(Of List(Of JSON_TagMetadata))
    ''' <summary>
    ''' get root id
    ''' </summary>
    Function RootID() As String
    ''' <summary>
    ''' Returns the list of folders present in user's account
    ''' https://apidocs.zoho.com/files/v1/folders?authtoken=AuthToken&scope=docsapi
    ''' </summary>
    ''' <param name="DestinationFolderID">Optional. Id of the folder to get sub folder list. This takes default as root folder</param>
    ''' <param name="Limit">Optional. This value is set to get the files list limit. If the both start and limit is not set, default value 0 and 200 will be taken respectively</param>
    ''' <param name="Offset">Optional. This value is set to get the list of files from that entry</param>
    Function ListAllFolders(DestinationFolderID As String, Optional Limit As Integer? = 200, Optional Offset As Integer? = 0) As Task(Of List(Of JSON_FolderMetadata))
    ''' <summary>
    ''' Retrieves the user's files and folders
    ''' </summary>
    ''' <param name="DestinationFolderID">Optional. Id of the folder to get sub folder list. This takes default as root folder</param>
    ''' <param name="Limit">Optional. This value is set to get the files list limit. If the both start and limit is not set, default value 0 and 200 will be taken respectively</param>
    ''' <param name="Offset">Optional. This value is set to get the list of files from that entry</param>
    Function ListFilesAndFolders(DestinationFolderID As String, Optional Limit As Integer? = 200, Optional Offset As Integer? = 0) As Task(Of JSON_ListFolder)
End Interface
