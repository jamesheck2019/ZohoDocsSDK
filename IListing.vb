Imports DeQma.ZohoDocsSDK.ZhOutilities

Public Interface IListing
    Function ListAllFiles(Optional Category As Category = Category._ALL_, Optional Offset As String = Nothing, Optional Limit As String = Nothing) As Task(Of JSON_FilesMetadata)
    Function ListAllFolders(DestinationFolderID As String) As Task(Of JSON_FoldersMetadata)
    Function ListSubFilesAndFolders(DestinationFolderID As String) As Task(Of JSON_FilesFoldersMetadata)
    Function ListSubFilesAndFolders2(DestinationFolderID As String) As Task(Of JSON_FilesFoldersMetadata)
    Function ListRootFilesFolders() As Task(Of JSON_FilesFoldersMetadata)
End Interface
