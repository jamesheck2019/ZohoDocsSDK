Imports ZohoDocsSDK
Imports ZohoDocsSDK.JSON
Imports ZohoDocsSDK.utilitiez

Public Interface IItem

    ''' <summary>
    ''' Renames the existing file/folder
    ''' https://apidocs.zoho.com/files/v1/folders/rename?authtoken=AuthToken&scope=docsapi
    ''' https://apidocs.zoho.com/files/v1/rename?authtoken=AuthToken&scope=docsapi
    ''' </summary>
    ''' <param name="NewName">Mandatory. New name for the file/folder</param>
    Function FD_Rename(NewName As String) As Task(Of Boolean)
    ''' <summary>
    ''' Moves the existing file/folder to specified location
    ''' https://apidocs.zoho.com/files/v1/folders/move?authtoken=AuthToken&scope=docsapi
    ''' https://apidocs.zoho.com/files/v1/move?authtoken=AuthToken&scope=docsapi
    ''' </summary>
    ''' <param name="DestinationFolderID">Mandatory. Id of the folder to be moved</param>
    Function FD_Move(DestinationFolderID As String) As Task(Of Boolean)
    ''' <summary>
    ''' Moves the file/folder to trash
    ''' https://apidocs.zoho.com/files/v1/trash?authtoken=AuthToken&scope=docsapi
    ''' </summary>
    Function FD_Trash() As Task(Of Boolean)
    ''' <summary>
    ''' file/folder will be removed from user's account
    ''' https://apidocs.zoho.com/files/v1/delete?authtoken=AuthToken&scope=docsapi
    ''' </summary>
    Function FD_Delete() As Task(Of Boolean)
    ''' <summary>
    ''' Sharing a file/folder via link
    ''' https://apidocs.zoho.com/files/v1/share/visibility?authtoken=AuthToken&scope=docsapi
    ''' </summary>
    ''' <param name="Permission">Mandatory. file shared to the user with the specified permission</param>
    ''' <param name="Password">Optional. password required if the doc is shared via link share with secured</param>
    ''' <param name="ExpireDate">Optional. specify the date till the document can be shared via link share</param>
    Function FD_Share(Permission As PermissionEnum, Optional Password As String = Nothing, Optional ExpireDate As Date = Nothing) As Task(Of String)
    ''' <summary>
    ''' unShare a file/folder
    ''' https://apidocs.zoho.com/files/v1/share/visibility?authtoken=AuthToken&scope=docsapi
    ''' </summary>
    Function FD_UnShare() As Task(Of Boolean)
    ''' <summary>
    ''' Returns the shared details of a file/folder
    ''' https://apidocs.zoho.com/files/v1/share/details?authtoken=AuthToken&scope=docsapi
    ''' </summary>
    Function FD_SharesMetadata() As Task(Of JSON_SharesMetadata)
    ''' <summary>
    ''' Copies the existing folder to a specified folder
    ''' https://apidocs.zoho.com/files/v1/folders/copy?authtoken=AuthToken&scope=docsapi
    ''' </summary>
    ''' <param name="DestinationFolderID">Mandatory. Specifies I.D of the destination folder (where existing folder copies).</param>
    Function D_Copy(DestinationFolderID As String) As Task(Of Boolean)
    ''' <summary>
    ''' Uploads a file
    ''' https://apidocs.zoho.com/files/v1/upload?authtoken=AuthToken&scope=docsapi
    ''' </summary>
    ''' <param name="FileToUpload">file to upload</param>
    ''' <param name="UploadType">the file object definition path or stream or bytes array</param>
    ''' <param name="FileName">the object file name </param>
    ''' <param name="DestinationWorkspaceID">Optional. id of the workspace where files to be uploaded to a workspace</param>
    ''' <param name="ReportCls">IProgress</param>
    ''' <param name="token">Cancellation Token</param>
    Function D_Upload(FileToUpload As Object, UploadType As UploadTypes, FileName As String, Optional DestinationWorkspaceID As String = Nothing, Optional ReportCls As IProgress(Of ReportStatus) = Nothing, Optional token As Threading.CancellationToken = Nothing) As Task(Of JSON_Upload)
    ''' <summary>
    ''' Creates a new folder
    ''' https://apidocs.zoho.com/files/v1/folders/create?authtoken=AuthToken&scope=docsapi
    ''' </summary>
    ''' <param name="FolderName">Mandatory. Name of the folder in which folder to be created.</param>
    Function D_Create(FolderName As String) As Task(Of JSON_NewFolder)
    ''' <summary>
    ''' Returns the list of files and folders
    ''' </summary>
    Function D_List() As Task(Of JSON_ListFolder)
    ''' <summary>
    ''' Copies a file or document to a new location
    ''' https://apidocs.zoho.com/files/v1/copy?authtoken=AuthToken&scope=docsapi
    ''' </summary>
    ''' <param name="DestinationFolderID">Mandatory. Id of folder to copy the new file</param>
    Function F_Copy(DestinationFolderID As String) As Task(Of Boolean)
    ''' <summary>
    ''' Downloads a file
    ''' https://apidocs.zoho.com/files/v1/content/<DOC_ID>?authtoken=AuthToken&scope=docsapi
    ''' </summary>
    ''' <param name="FileSaveDir">"D:\\Downloads"</param>
    ''' <param name="FileName">file.rar</param>
    ''' <param name="DestinationFileVersion">Optional. Version of the document to be downloaded. This defaults to recent one.</param>
    ''' <param name="ReportCls">IProgress</param>
    ''' <param name="token">Cancellation Token</param>
    Function F_Download(FileSaveDir As String, FileName As String, Optional DestinationFileVersion As String = Nothing, Optional ReportCls As IProgress(Of ReportStatus) = Nothing, Optional token As Threading.CancellationToken = Nothing) As Task
    ''' <summary>
    ''' Downloads a file as IO.Stream
    ''' https://apidocs.zoho.com/files/v1/content/<DOC_ID>?authtoken=AuthToken&scope=docsapi
    ''' </summary>
    ''' <param name="ReportCls">IProgress</param>
    ''' <param name="token">Cancellation Token</param>
    Function F_DownloadAsStream(Optional ReportCls As IProgress(Of ReportStatus) = Nothing, Optional token As Threading.CancellationToken = Nothing) As Task(Of IO.Stream)
    ''' <summary>
    ''' Lists the revision details for the given document
    ''' https://apidocs.zoho.com/files/v1/revision/details?authtoken=AuthToken&scope=docsapi
    ''' </summary>
    ''' <param name="RevisionType">Mandatory. specify which type of document</param>
    Function F_RevisionMetadata(RevisionType As utilitiez.RevisionTypeEnum) As Task(Of JSON_RevisionMetadata)
    ''' <summary>
    ''' Adds a tag to a file or a document
    ''' https://apidocs.zoho.com/files/v1/tags/add?authtoken=AuthToken&scope=docsapi
    ''' </summary>
    ''' <param name="TagsNames">Mandatory. Name of the tag to be added</param>
    Function F_AddTags(TagsNames As List(Of String)) As Task(Of Boolean)
    ''' <summary>
    ''' Removes the tag which is mapped to a document or file
    ''' https://apidocs.zoho.com/files/v1/tags/remove?authtoken=AuthToken&scope=docsapi
    ''' </summary>
    ''' <param name="TagsNames">Mandatory. Name of the tag to be removed from file or document</param>
    Function F_RemoveTags(TagsNames As List(Of String)) As Task(Of Boolean)

End Interface
