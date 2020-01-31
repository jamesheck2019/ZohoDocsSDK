Imports System.ComponentModel
Imports Newtonsoft.Json

Namespace JSON


#Region "JSON_CreateNewFolder"
    Public Class JSON_NewFolder
        Public Property folder_name As String
        Public Property folder_id As String
    End Class
#End Region

#Region "JSON_ListPublicLink"
    Public Class JSON_ListPublicLink
        <JsonProperty("DOC")> Public Property ResultsList As List(Of JSON_DOC)

        Public Property TOTALDOCS As List(Of JSON_TOTALDOC)
        Public ReadOnly Property Total As String
            Get
                Return TOTALDOCS(0).NOOFDOCS.ToString
            End Get
        End Property
    End Class
    Public Class JSON_DOC
        <JsonProperty("IS_FAVOURITE")> Public Property IsFavourite As Boolean
        <JsonProperty("IS_LOCKED")> Public Property IsLocked As Boolean
        <JsonProperty("SCOPE")> Public Property Scope As Integer
        <JsonProperty("SERVICE_TYPE")> Public Property ServiceType As String
        <JsonProperty("FOLDER_ID")> Public Property ParentFolderID As String
        <JsonProperty("FOLDER_NAME")> Public Property ParentFolderName As String
        <JsonProperty("DOCNAME")> Public Property Name As String
        <JsonProperty("DOCID")> Public Property ID As String
        <JsonProperty("FILETYPE")> Public Property Type As String
        <JsonProperty("FILE_EXTN")> Public Property Extension As String
        <JsonProperty("LAST_MODIFIEDTIME")> Public Property _ModifiedDate As String
        <JsonProperty("LAST_MODIFIEDBY")> Public Property _LastModifiedByAuthorID As String
        <JsonProperty("LAST_MODIFIEDTIME_IN_MILLISECONDS")> Public Property _LastModified_InMilliseconds As Long
        <JsonProperty("LAST_MODIFIEDBY_AUTHOR_NAME")> Public Property _LastModifiedByAuthorName As String
        <JsonProperty("CREATED_TIME_IN_MILLISECONDS")> Public Property _CreatedDate_InMilliseconds As Long
        <JsonProperty("CREATED_TIME")> Public Property _CreatedDate As String
        <JsonProperty("LAST_OPENEDTIME")> Public Property _LastOpenedDate As String
        <JsonProperty("LAST_OPENEDTIME_IN_MILLISECONDS")> Public Property _LastOpenedDate_InMilliseconds As Long
        <JsonProperty("AUTHOR_ID")> Public Property AuthorID As String
        <JsonProperty("DOC_AUTHOR_NAME")> Public Property AuthorName As String
        <JsonProperty("AUTHOR_EMAIL")> Public Property AuthorEmail As String

        Public Property PERMISSION As Integer
        Public Property SHARED_TYPE As String
        Public Property IS_DOC_SHARED As Boolean
        Enum FileFolder
            file
            folder
        End Enum
        Public ReadOnly Property FileOrFolder As FileFolder
            Get
                Return If(Type = FileFolder.folder.ToString, FileFolder.folder, FileFolder.file)
            End Get
        End Property
        Public ReadOnly Property Url As String
            Get
                Return If(FileOrFolder = FileFolder.folder, String.Format("https://apidocs.zoho.com/folder/{0}", ID), String.Format("https://apidocs.zoho.com/file/{0}", ID))
            End Get
        End Property
        Public ReadOnly Property FileDownloadLink As String
            Get
                Return If(FileOrFolder = FileFolder.file, String.Format("https://docs.zoho.com/downloaddocument.do?docId={0}&docExtn={1}", ID, Extension), Nothing)
            End Get
        End Property
        Public ReadOnly Property FileJSON As JSON_FileJSON
            Get
                If FileOrFolder = FileFolder.file Then
                    Dim hr As New ZohoDocsSDK.DeQmaTek.TcpClientHttpRequest()
                    hr.Action = String.Format("https://docs.zoho.com/services/oembed?type=json&url={0}", Url)
                    hr.Method = "GET"
                    hr.UserAgent = "DeQma.TcpClientHttp"
                    hr.Send()
                    Dim result = System.Text.Encoding.UTF8.GetString(hr.Response.GetStream)
                    Return JsonConvert.DeserializeObject(Of JSON_FileJSON)(result, JSONhandler)
                End If
            End Get
        End Property
    End Class
    Public Class JSON_TOTALDOC
        Public Property NOOFDOCS As Integer
    End Class
    Public Class JSON_FileJSON
        Public Property author_name As String
        Public Property author_url As String
        Public Property thumbnail_width As Integer
        Public Property description As String
        Public Property provider_url As String
        Public Property title As String
        Public Property provider_name As String
        Public Property thumbnail_url As String
        Public Property type As String
        Public Property version As String
        Public Property url As String
        Public Property thumbnail_height As Integer
    End Class

#End Region

#Region "JSON_RevisionMetadata"
    Public Class JSON_RevisionMetadata
        <JsonProperty("DOCID")> Public Property ID As String
        <JsonProperty("FORMAT")> Public Property Extension As String
        <JsonProperty("DOCNAME")> Public Property Name As String
        <JsonProperty("DOCTYPE")> Public Property Type As String
        <JsonProperty("VERSION_INFO")> Public Property VersionsList As List(Of frd4_VERSION_INFO)
    End Class
    Public Class frd4_VERSION_INFO
        <JsonProperty("DOCUPLOADTIME")> Public Property UploadedDate As String
        <JsonProperty("DOCSIZE")> Public Property Size As String
        <JsonProperty("VERSION")> Public Property Version As String
    End Class
#End Region

#Region "JSON_Upload"
    Public Class JSON_Upload
        <JsonProperty("documentname")> Public Property name As String
        <JsonProperty("uploaddocid")> Public Property id As String
    End Class
#End Region

#Region "JSON_ListFolder"
    Public Class JSON_ListFolder
        Public Property FILES As List(Of JSON_FileMetadata)
        Public Property FOLDER As List(Of JSON_FolderMetadata)
    End Class
#End Region


End Namespace






