Imports DeQma.ZohoDocsSDK.JSON

Public Interface IFolder

    Function RenameFolder(FolderID As String, NewFolderName As String) As Task(Of JSON_RenameFolder)
    Function TrashFolder(FolderID As String) As Task(Of JSON_TrashFolder)
    Function CreateNewFolder(FolderName As String, Optional DestinationFolderID As String = Nothing) As Task(Of JSON_CreateNewFolder)
    Function MoveFolder(FolderID As String, ParentFolderID As String, DestinationFolderID As String) As Task(Of JSON_MoveFolder)
    Function CopyFolder(FolderID As String, DestinationFolderID As String) As Task(Of JSON_CopyFolder)
    Function DeleteFolder(FolderID As String) As Task(Of JSON_DeleteFolder)
End Interface
