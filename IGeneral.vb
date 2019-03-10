Imports DeQma.ZohoDocsSDK.JSON

Public Interface IGeneral

    Function DeleteFileFolder(DestinationIDID As String) As Task(Of JSON_DeleteFileFolder)
    Function CopyFileFolder(SourceID As String, DestinationFolderID As String) As Task(Of JSON_CopyFileFolder)
    Function CopyMultipleFilesFolders(SourceIDs As List(Of String), DestinationFolderID As String) As Task(Of JSON_CopyMultipleFiles)
    Function TrashFileFolder(SourceID As String) As Task(Of JSON_TrashFileFolder)
    Function RenameFileFolder(DestinationID As String, NewName As String) As Task(Of JSON_RenameFileFolder)


End Interface
