Imports DeQma.ZohoDocsSDK.JSON

Public Interface ITags
    Function ListTags() As Task(Of JSON_ListTags)
    Function AddFileTag(FileID As String, TagName As String) As Task(Of JSON_AddFileTag)
    Function AddFileMultipleTags(FileID As String, TagsNames As List(Of String)) As Task(Of JSON_AddFileTag)
    'Function AddMultipleFilesTags(FilesIDs As List(Of String), TagsNames As List(Of String)) As Task(Of JSON_AddFileTag)
    Function RemoveFileTag(FileID As String, TagsNames As List(Of String)) As Task(Of JSON_RemoveFileTag)

End Interface
