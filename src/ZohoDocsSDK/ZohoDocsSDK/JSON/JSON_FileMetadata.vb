Imports Newtonsoft.Json

Namespace JSON
    Public Class JSON_FileMetadata
        '<JsonProperty("ENCATT_NAME", NullValueHandling:=NullValueHandling.Ignore)>
        'Public Property Name2 As String
        '<JsonProperty("ENCHTML_NAME", NullValueHandling:=NullValueHandling.Ignore)>
        'Public Property Name3 As String
        '<JsonProperty("ENCURL_NAME", NullValueHandling:=NullValueHandling.Ignore)>
        'Public Property Name4 As String
        '<JsonProperty("TRIM_DOCNAME", NullValueHandling:=NullValueHandling.Ignore)>
        'Public Property TrimName As String

        '<JsonProperty("AUTHOR_ID", NullValueHandling:=NullValueHandling.Ignore)>
        'Public Property AuthorID As String
        '<JsonProperty("AUTHOR", NullValueHandling:=NullValueHandling.Ignore)>
        'Public Property AuthorName As String
        '<JsonProperty("AUTHOR_EMAIL", NullValueHandling:=NullValueHandling.Ignore)>
        'Public Property AuthorEmail As String

        '<JsonProperty("LAST_MODIFIEDTIME", NullValueHandling:=NullValueHandling.Ignore)>
        'Public Property _ModifiedDate As String
        '<JsonProperty("LAST_MODIFIEDBY", NullValueHandling:=NullValueHandling.Ignore)>
        'Public Property _LastModifiedByAuthorID As String
        <JsonProperty("LAST_MODIFIEDTIME_IN_MILLISECONDS", NullValueHandling:=NullValueHandling.Ignore)>
        Public Property _LastModified_InMilliseconds As Long
        '<JsonProperty("LAST_MODIFIEDBY_AUTHOR_NAME", NullValueHandling:=NullValueHandling.Ignore)>
        'Public Property _LastModifiedByAuthorName As String
        <JsonProperty("CREATED_TIME_IN_MILLISECONDS", NullValueHandling:=NullValueHandling.Ignore)>
        Public Property _CreatedDate_InMilliseconds As Long
        '<JsonProperty("CREATED_TIME", NullValueHandling:=NullValueHandling.Ignore)>
        'Public Property _CreatedDate As String
        '<JsonProperty("LAST_OPENED_TIME", NullValueHandling:=NullValueHandling.Ignore)>
        'Public Property _LastOpenedDate As String
        <JsonProperty("LAST_OPENED_TIME_IN_MILLISECONDS", NullValueHandling:=NullValueHandling.Ignore)>
        Public Property _LastOpenedDate_InMilliseconds As Long

        <JsonProperty("DOCNAME", NullValueHandling:=NullValueHandling.Ignore)>
        Public Property Name As String
        <JsonProperty("DOCID", NullValueHandling:=NullValueHandling.Ignore)>
        Public Property ID As String
        <JsonProperty("FILETYPE", NullValueHandling:=NullValueHandling.Ignore)>
        Public Property Type As String
        <JsonProperty("FILE_EXTN", NullValueHandling:=NullValueHandling.Ignore)>
        Public Property Extension As String
        <JsonProperty("FOLDER_ID", NullValueHandling:=NullValueHandling.Ignore)>
        Private Property _ParentFolderID As String
        Public ReadOnly Property ParentFolderID As String
            Get
                Return If(_ParentFolderID = "folder", String.Empty, _ParentFolderID)
            End Get
        End Property

        '<JsonProperty("SCOPE", NullValueHandling:=NullValueHandling.Ignore)>
        'Public Property Scope As Integer

        <JsonProperty("IS_SHARED", NullValueHandling:=NullValueHandling.Ignore)>
        Public Property IsShared As Boolean
        '<JsonProperty("IS_FAVOURITE", NullValueHandling:=NullValueHandling.Ignore)>
        'Public Property IsFavourite As Boolean
        <JsonProperty("IS_LOCKED", NullValueHandling:=NullValueHandling.Ignore)>
        Public Property IsLocked As Boolean

        '<JsonProperty("SERVICE_TYPE", NullValueHandling:=NullValueHandling.Ignore)>
        'Public Property ServiceType As String
        <JsonProperty("EXTRA_PROP", NullValueHandling:=NullValueHandling.Ignore)>
        Public Property EXTRA_PROP As String

    End Class
End Namespace


