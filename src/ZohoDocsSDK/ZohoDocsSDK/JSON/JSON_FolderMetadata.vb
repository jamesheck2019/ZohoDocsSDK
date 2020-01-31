Imports System.ComponentModel
Imports Newtonsoft.Json

Namespace JSON
    Public Class JSON_FolderMetadata
        '<JsonProperty("AUTHOR_ID")>'Public Property AuthorID As String
        '<JsonProperty("AUTHOR_NAME")>'Public Property AuthorName As String

        '<JsonProperty("LAST_MODIFIED_AUTHOR_NAME")>'Public Property _LastModifiedByAuthorName As String
        <JsonProperty("LAST_MODIFIED_TIME")> Public Property _LastModified_InMilliseconds As String
        '<JsonProperty("CREATED_TIME")>'Public Property _CreatedDate As String
        '<JsonProperty("LAST_MODIFIEDBY")>'Public Property _LastModifiedByAuthorID As String
        '<JsonProperty("LAST_OPENED_TIME")>'Public Property _LastOpenedDate As String
        'Public ReadOnly Property _ModifiedDate As String
        '    Get

        '        Dim dtDateTime As System.DateTime = New DateTime(1970, 1, 1, 0, 0, 0, 0)
        '        dtDateTime = dtDateTime.AddMilliseconds(_LastModified_InMilliseconds).ToLocalTime()
        '        Return dtDateTime
        '    End Get
        'End Property

        <Browsable(False)> <Bindable(False)> <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> <EditorBrowsable(EditorBrowsableState.Never)> <JsonProperty("FOLDER_NAME")> Private Property TheName As String
        <Browsable(False)> <Bindable(False)> <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> <EditorBrowsable(EditorBrowsableState.Never)> <JsonProperty("FOLDERNAME")> Private Property TheName2 As String
        <Browsable(False)> <Bindable(False)> <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> <EditorBrowsable(EditorBrowsableState.Never)> <JsonProperty("FOLDER_ID")> Private Property TheID As String
        <Browsable(False)> <Bindable(False)> <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> <EditorBrowsable(EditorBrowsableState.Never)> <JsonProperty("FOLDERID")> Private Property TheID2 As String
        <JsonProperty("PERMISSION")> Public Property Permission As Integer
        '<JsonProperty("SCOPE")> Public Property Scope As Integer
        <JsonProperty("PARENT_FOLDER_ID")> Public Property ParentFolderID As String

        <JsonProperty("IS_SHARED")>
        Public Property IsShared As Boolean
        '<JsonProperty("IS_FAVOURITE")>Public Property IsFavourite As Boolean

        Public ReadOnly Property Name As String
            Get
                Dim nme = If(Not String.IsNullOrEmpty(TheName), TheName, TheName2)
                Return nme
            End Get
        End Property
        Public ReadOnly Property ID As String
            Get
                Dim dd = If(Not String.IsNullOrEmpty(TheID), TheID, TheID2)
                Return dd
            End Get
        End Property
    End Class
End Namespace
