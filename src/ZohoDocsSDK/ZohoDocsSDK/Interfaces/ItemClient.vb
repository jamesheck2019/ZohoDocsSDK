Imports Newtonsoft.Json.Linq
Imports System.Net.Http.HttpMethod
Imports System.Net.Http
Imports ZohoDocsSDK.utilitiez
Imports Newtonsoft.Json
Imports ZohoDocsSDK.JSON

Public Class ItemClient
    Implements IItem

    Private Property ID As String

    Sub New(ID As String)
        Me.ID = ID
    End Sub


#Region "RenameFileFolder"
    Public Async Function POST_RenameFileFolder(NewName As String) As Task(Of Boolean) Implements IItem.FD_Rename
        Dim parameters = New AuthDictionary
        parameters.Add("docid", ID)
        parameters.Add("docname", NewName)

        Using localHttpClient As New HttpClient(New HCHandler)
            Dim HtpReqMessage As New Net.Http.HttpRequestMessage(Post, New pUri("rename"))
            HtpReqMessage.Content = New FormUrlEncodedContent(parameters)
            Using response As Net.Http.HttpResponseMessage = Await localHttpClient.SendAsync(HtpReqMessage, Net.Http.HttpCompletionOption.ResponseContentRead).ConfigureAwait(False)
                Dim result = Await response.Content.ReadAsStringAsync()

                If response.IsSuccessStatusCode Then
                    Return JObject.Parse(result).SelectToken("response.result.DocumentDetails.DocumentDetail.message").ToString.Contains("SUCCESSFULLY") '("response")("result")("DocumentDetails")("DocumentDetail")("message").ToString().Contains("SUCCESSFULLY")
                Else
                    Throw ExceptionCls.CreateException(result.Jobj.SelectToken("response.result.DocumentDetails.DocumentDetail.message").ToString, response.StatusCode)
                End If
            End Using
        End Using
    End Function
#End Region

#Region "MoveFileFolder"
    Public Async Function POST_MoveFileFolder(DestinationFolderID As String) As Task(Of Boolean) Implements IItem.FD_Move
        Dim parameters = New AuthDictionary From {{"docid", ID}, {"folderid", DestinationFolderID}}

        Using localHttpClient As New HttpClient(New HCHandler)
            Dim HtpReqMessage As New Net.Http.HttpRequestMessage(Post, New pUri("move"))
            HtpReqMessage.Content = New FormUrlEncodedContent(parameters)
            Using response As Net.Http.HttpResponseMessage = Await localHttpClient.SendAsync(HtpReqMessage, HttpCompletionOption.ResponseContentRead).ConfigureAwait(False)
                Dim result = Await response.Content.ReadAsStringAsync()

                If response.IsSuccessStatusCode Then
                    Return result.Jobj.SelectToken("response.result.message").ToString.Contains("SUCCESSFULLY")
                Else
                    Throw ExceptionCls.CreateException(result.Jobj.SelectToken("response.result.message").ToString, response.StatusCode)
                End If
            End Using
        End Using
    End Function
#End Region

#Region "TrashFileFolder"
    Public Async Function POST_TrashFileFolder() As Task(Of Boolean) Implements IItem.FD_Trash
        Dim parameters = New AuthDictionary
        parameters.Add("docid", ID)

        Using localHttpClient As New HttpClient(New HCHandler)
            Dim HtpReqMessage As New Net.Http.HttpRequestMessage(Post, New pUri("trash"))
            HtpReqMessage.Content = New FormUrlEncodedContent(parameters)
            Using response As Net.Http.HttpResponseMessage = Await localHttpClient.SendAsync(HtpReqMessage, Net.Http.HttpCompletionOption.ResponseContentRead).ConfigureAwait(False)
                Dim result = Await response.Content.ReadAsStringAsync()

                If response.IsSuccessStatusCode Then
                    Return result.Jobj.SelectToken("response.result.message").ToString.Contains("SUCCESSFULLY")
                Else
                    Throw ExceptionCls.CreateException(result.Jobj.SelectToken("response.result.message").ToString, response.StatusCode)
                End If
            End Using
        End Using
    End Function
#End Region

#Region "DeleteFileFolder"
    Public Async Function POST_DeleteFileFolderPermanently() As Task(Of Boolean) Implements IItem.FD_Delete
        Dim parameters = New AuthDictionary From {{"docid", ID}}

        Using localHttpClient As New HttpClient(New HCHandler)
            Dim HtpReqMessage As New Net.Http.HttpRequestMessage(Post, New pUri("delete"))
            HtpReqMessage.Content = New FormUrlEncodedContent(parameters)
            Using response As Net.Http.HttpResponseMessage = Await localHttpClient.SendAsync(HtpReqMessage, Net.Http.HttpCompletionOption.ResponseContentRead).ConfigureAwait(False)
                Dim result = Await response.Content.ReadAsStringAsync()

                If response.IsSuccessStatusCode Then
                    Return result.Jobj.SelectToken("response.result.message").ToString.Contains("SUCCESSFULLY")
                Else
                    Throw ExceptionCls.CreateException(result.Jobj.SelectToken("response.result.message").ToString, response.StatusCode)
                End If
            End Using
        End Using
    End Function
#End Region

#Region "ShareFileFolder"
    Public Async Function POST_PublicFileFolder(Permission As PermissionEnum, Optional Password As String = Nothing, Optional ExpireDate As Date = Nothing) As Task(Of String) Implements IItem.FD_Share
        Dim parameters = New AuthDictionary
        parameters.Add("docid", ID)
        parameters.Add("visibility", "linkshare")
        parameters.Add("permission", Permission.ToString)
        parameters.Add("password", Password)
        parameters.Add("expireson", ExpireDate.ToString("mm/dd/yyyy"))

        Using localHttpClient As New HttpClient(New HCHandler)
            Dim HtpReqMessage As New Net.Http.HttpRequestMessage(Post, New pUri("share/visibility"))
            HtpReqMessage.Content = New FormUrlEncodedContent(parameters)
            Using response As Net.Http.HttpResponseMessage = Await localHttpClient.SendAsync(HtpReqMessage, Net.Http.HttpCompletionOption.ResponseContentRead).ConfigureAwait(False)
                Dim result = Await response.Content.ReadAsStringAsync()

                If response.IsSuccessStatusCode Then
                    Return result.Jobj.SelectToken("response[2].result[0].permaLink").ToString
                Else
                    Throw ExceptionCls.CreateException(result.Jobj.SelectToken(result)("response[1].message").ToString, response.StatusCode)
                End If
            End Using
        End Using
    End Function
#End Region

#Region "UnShareFileFolder"
    Public Async Function POST_UnPublicFileFolder() As Task(Of Boolean) Implements IItem.FD_UnShare
        Using localHttpClient As New HttpClient(New HCHandler)
            Dim HtpReqMessage As New Net.Http.HttpRequestMessage(Post, New pUri("share/visibility"))
            HtpReqMessage.Content = New FormUrlEncodedContent(New AuthDictionary From {{"docid", ID}, {"visibility", "private"}})
            Using response As Net.Http.HttpResponseMessage = Await localHttpClient.SendAsync(HtpReqMessage, Net.Http.HttpCompletionOption.ResponseContentRead).ConfigureAwait(False)
                Dim result = Await response.Content.ReadAsStringAsync()

                If response.IsSuccessStatusCode Then
                    Return result.Jobj.SelectToken("response[1].message").ToString.Contains("SUCCESS")
                Else
                    Throw ExceptionCls.CreateException(result.Jobj.SelectToken("response[1].message").ToString, response.StatusCode)
                End If
            End Using
        End Using
    End Function
#End Region

#Region "SharedDetails"
    Public Async Function POST_SharedDetails() As Task(Of JSON_SharesMetadata) Implements IItem.FD_SharesMetadata
        Using localHttpClient As New HttpClient(New HCHandler)
            Dim RequestUri = New pUri("share/details", New AuthDictionary From {{"docid", ID}})
            Using response As Net.Http.HttpResponseMessage = Await localHttpClient.GetAsync(RequestUri).ConfigureAwait(False)
                Dim result = Await response.Content.ReadAsStringAsync()

                If response.IsSuccessStatusCode Then
                    Return JsonConvert.DeserializeObject(Of JSON_SharesMetadata)(result, JSONhandler)
                Else
                    Throw ExceptionCls.CreateException(result.Jobj.SelectToken("response[1].message").ToString, response.StatusCode)
                End If
            End Using
        End Using
    End Function
#End Region

#Region "CopyFolder"
    Public Async Function POST_CopyFolder(DestinationFolderID As String) As Task(Of Boolean) Implements IItem.D_Copy
        Dim parameters = New AuthDictionary From {{"folderid", ID}, {"destfolderid", DestinationFolderID}}

        Using localHttpClient As New HttpClient(New HCHandler)
            Dim HtpReqMessage As New Net.Http.HttpRequestMessage(Post, New pUri("folders/copy"))
            HtpReqMessage.Content = New FormUrlEncodedContent(parameters)
            Using response As Net.Http.HttpResponseMessage = Await localHttpClient.SendAsync(HtpReqMessage, Net.Http.HttpCompletionOption.ResponseContentRead).ConfigureAwait(False)
                Dim result = Await response.Content.ReadAsStringAsync()

                If response.IsSuccessStatusCode Then
                    Return result.Jobj.SelectToken("response.result.message").ToString.Contains("SUCCESSFULLY")
                Else
                    Throw ExceptionCls.CreateException(result.Jobj.SelectToken("response.result.message").ToString, response.StatusCode)
                End If
            End Using
        End Using
    End Function
#End Region

#Region "CreateNewFolder"
    Public Async Function POST_CreateNewFolder(FolderName As String) As Task(Of JSON_NewFolder) Implements IItem.D_Create
        Dim parameters = New AuthDictionary From {{"parentfolderid", ID}, {"foldername", FolderName}}

        Using localHttpClient As New HttpClient(New HCHandler)
            Dim HtpReqMessage As New Net.Http.HttpRequestMessage(Post, New pUri("folders/create"))
            HtpReqMessage.Content = New FormUrlEncodedContent(parameters)
            Using response As Net.Http.HttpResponseMessage = Await localHttpClient.SendAsync(HtpReqMessage, Net.Http.HttpCompletionOption.ResponseContentRead).ConfigureAwait(False)
                Dim result = Await response.Content.ReadAsStringAsync()

                If response.IsSuccessStatusCode Then
                    Return JsonConvert.DeserializeObject(Of JSON_NewFolder)(result.Jobj.SelectToken("response.result.FolderDetails.FolderDetail").ToString, JSONhandler)
                Else
                    Throw ExceptionCls.CreateException(result.Jobj.SelectToken("response.result.FolderDetails.FolderDetail.message").ToString, response.StatusCode)
                End If
            End Using
        End Using
    End Function
#End Region

#Region "UploadLocal"
    Public Async Function Get_UploadLocal(FileToUpload As Object, UploadType As UploadTypes, FileName As String, Optional DestinationWorkspaceID As String = Nothing, Optional ReportCls As IProgress(Of ReportStatus) = Nothing, Optional token As Threading.CancellationToken = Nothing) As Task(Of JSON_Upload) Implements IItem.D_Upload
        If ReportCls Is Nothing Then ReportCls = New Progress(Of ReportStatus)
        ReportCls.Report(New ReportStatus With {.Finished = False, .TextStatus = "Initializing..."})
        Try
            Dim progressHandler As New Net.Http.Handlers.ProgressMessageHandler(New HCHandler)
            AddHandler progressHandler.HttpSendProgress, (Function(sender, e)
                                                              ReportCls.Report(New ReportStatus With {.ProgressPercentage = e.ProgressPercentage, .BytesTransferred = e.BytesTransferred, .TotalBytes = If(e.TotalBytes Is Nothing, 0, e.TotalBytes), .TextStatus = "Uploading..."})
                                                          End Function)
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            Dim localHttpClient As New HttpClient(progressHandler)
            Dim HtpReqMessage As New Net.Http.HttpRequestMessage()
            HtpReqMessage.Method = Net.Http.HttpMethod.Post
            ''''''''''''''''''''''''''''''''''
            Dim MultipartsformData = New Net.Http.MultipartFormDataContent()
            Dim streamContent As Net.Http.HttpContent
            Select Case UploadType
                Case UploadTypes.FilePath
                    streamContent = New Net.Http.StreamContent(New IO.FileStream(FileToUpload, IO.FileMode.Open, IO.FileAccess.Read))
                Case UploadTypes.Stream
                    streamContent = New Net.Http.StreamContent(CType(FileToUpload, IO.Stream))
                Case UploadTypes.BytesArry
                    streamContent = New Net.Http.StreamContent(New IO.MemoryStream(CType(FileToUpload, Byte())))
                Case UploadTypes.String
                    streamContent = New Net.Http.StringContent(System.IO.File.ReadAllText(FileToUpload))
            End Select
            MultipartsformData.Add(streamContent, "content", FileName)
            MultipartsformData.Add(New Net.Http.StringContent(authToken), "authtoken")
            MultipartsformData.Add(New Net.Http.StringContent("docsapi"), "scope")
            MultipartsformData.Add(New Net.Http.StringContent(FileName), "filename")
            If ID IsNot Nothing Then MultipartsformData.Add(New Net.Http.StringContent(ID), "fid")
            If DestinationWorkspaceID IsNot Nothing Then MultipartsformData.Add(New Net.Http.StringContent(DestinationWorkspaceID), "wsid")
            HtpReqMessage.Content = MultipartsformData

            HtpReqMessage.RequestUri = New pUri("upload", New AuthDictionary)
            '''''''''''''''''will write the whole content to H.D WHEN download completed'''''''''''''''''''''''''''''
            Using ResPonse As Net.Http.HttpResponseMessage = Await localHttpClient.SendAsync(HtpReqMessage, Net.Http.HttpCompletionOption.ResponseHeadersRead, token).ConfigureAwait(False)
                Dim result As String = Await ResPonse.Content.ReadAsStringAsync()

                token.ThrowIfCancellationRequested()
                If ResPonse.StatusCode = Net.HttpStatusCode.OK Then
                    ReportCls.Report(New ReportStatus With {.Finished = True, .TextStatus = "Upload completed successfully"})
                    Return JsonConvert.DeserializeObject(Of JSON_Upload)(result.Jobj.SelectToken("response[2].result[0]").ToString, JSONhandler)
                Else
                    ReportCls.Report(New ReportStatus With {.Finished = True, .TextStatus = String.Format("The request returned with HTTP status code {0}", result.Jobj.SelectToken("response[1].message").ToString)})
                    Throw ExceptionCls.CreateException(result.Jobj.SelectToken("response[1].message").ToString, ResPonse.StatusCode)
                End If
            End Using
        Catch ex As Exception
            ReportCls.Report(New ReportStatus With {.Finished = True})
            If ex.Message.ToString.ToLower.Contains("a task was canceled") Then
                ReportCls.Report(New ReportStatus With {.TextStatus = ex.Message})
            Else
                Throw CType(ExceptionCls.CreateException(ex.Message, ex.Message), ZohoDocsException)
            End If
        End Try
    End Function
#End Region

#Region "CopyFile"
    Public Async Function POST_CopyFile(DestinationFolderID As String) As Task(Of Boolean) Implements IItem.F_Copy
        Dim parameters = New AuthDictionary From {{"docid", ID}, {"folderid", DestinationFolderID}}

        Using localHttpClient As New HttpClient(New HCHandler)
            Dim HtpReqMessage As New Net.Http.HttpRequestMessage(Post, New pUri("copy"))
            HtpReqMessage.Content = New FormUrlEncodedContent(parameters)
            Using response As Net.Http.HttpResponseMessage = Await localHttpClient.SendAsync(HtpReqMessage, Net.Http.HttpCompletionOption.ResponseContentRead).ConfigureAwait(False)
                Dim result = Await response.Content.ReadAsStringAsync()

                If response.IsSuccessStatusCode Then
                    Return result.Jobj.SelectToken("response.result.message").ToString.Contains("SUCCESSFULLY")
                Else
                    Throw ExceptionCls.CreateException(result.Jobj.SelectToken("response.result.message").ToString, response.StatusCode)
                End If
            End Using
        End Using
    End Function
#End Region

#Region "download File"
    Public Async Function DownloadFile(FileSaveDir As String, FileName As String, Optional DestinationFileVersion As String = Nothing, Optional ReportCls As IProgress(Of ReportStatus) = Nothing, Optional token As Threading.CancellationToken = Nothing) As Task Implements IItem.F_Download
        If ReportCls Is Nothing Then ReportCls = New Progress(Of ReportStatus)
        ReportCls.Report(New ReportStatus With {.Finished = False, .TextStatus = "Initializing..."})
        Try
            Dim progressHandler As New Net.Http.Handlers.ProgressMessageHandler(New HCHandler)
            AddHandler progressHandler.HttpReceiveProgress, (Function(sender, e)
                                                                 ReportCls.Report(New ReportStatus With {.ProgressPercentage = e.ProgressPercentage, .BytesTransferred = e.BytesTransferred, .TotalBytes = If(e.TotalBytes Is Nothing, 0, e.TotalBytes), .TextStatus = "Downloading..."})
                                                             End Function)
            Dim localHttpClient As New HttpClient(progressHandler)
            Dim RequestUri = New pUri(String.Format("content/{0}", ID), New AuthDictionary From {{"docversion", DestinationFileVersion}})
            '''''''''''''''''will write the whole content to H.D WHEN download completed'''''''''''''''''''''''''''''
            Using ResPonse As Net.Http.HttpResponseMessage = Await localHttpClient.GetAsync(RequestUri, Net.Http.HttpCompletionOption.ResponseContentRead, token).ConfigureAwait(False)
                If ResPonse.IsSuccessStatusCode Then
                    ReportCls.Report(New ReportStatus With {.Finished = True, .TextStatus = (String.Format("[{0}] Downloaded successfully.", FileName))})
                Else
                    ReportCls.Report(New ReportStatus With {.Finished = True, .TextStatus = ((String.Format("Error code: {0}", ResPonse.StatusCode)))})
                End If

                ResPonse.EnsureSuccessStatusCode()
                Dim stream_ = Await ResPonse.Content.ReadAsStreamAsync()
                Dim FPathname As String = String.Concat(FileSaveDir.TrimEnd("\"), "\", FileName)
                Using fileStream = New IO.FileStream(FPathname, IO.FileMode.Append, IO.FileAccess.Write)
                    stream_.CopyTo(fileStream)
                End Using
            End Using
        Catch ex As Exception
            ReportCls.Report(New ReportStatus With {.Finished = True})
            If ex.Message.ToString.ToLower.Contains("a task was canceled") Then
                ReportCls.Report(New ReportStatus With {.TextStatus = ex.Message})
            Else
                Throw CType(ExceptionCls.CreateException(ex.Message, ex.Message), ZohoDocsException)
            End If
        End Try
    End Function
#End Region

#Region "DownloadFileAsStream"
    Public Async Function DownloadFileAsStream(Optional ReportCls As IProgress(Of ReportStatus) = Nothing, Optional token As Threading.CancellationToken = Nothing) As Task(Of IO.Stream) Implements IItem.F_DownloadAsStream
        If ReportCls Is Nothing Then ReportCls = New Progress(Of ReportStatus)
        ReportCls.Report(New ReportStatus With {.Finished = False, .TextStatus = "Initializing..."})
        Try
            Dim progressHandler As New Net.Http.Handlers.ProgressMessageHandler(New HCHandler)
            AddHandler progressHandler.HttpReceiveProgress, (Function(sender, e)
                                                                 ReportCls.Report(New ReportStatus With {.ProgressPercentage = e.ProgressPercentage, .BytesTransferred = e.BytesTransferred, .TotalBytes = If(e.TotalBytes Is Nothing, 0, e.TotalBytes), .TextStatus = "Downloading..."})
                                                             End Function)
            Dim localHttpClient As New HttpClient(progressHandler)
            Dim RequestUri = New pUri(String.Format("content/{0}", ID), New AuthDictionary)
            '''''''''''''''''will write the whole content to H.D WHEN download completed'''''''''''''''''''''''''''''
            Dim ResPonse As Net.Http.HttpResponseMessage = Await localHttpClient.GetAsync(RequestUri, Net.Http.HttpCompletionOption.ResponseContentRead, token).ConfigureAwait(False)
            If ResPonse.IsSuccessStatusCode Then
                ReportCls.Report(New ReportStatus With {.Finished = True, .TextStatus = ("File Downloaded successfully.")})
            Else
                ReportCls.Report(New ReportStatus With {.Finished = True, .TextStatus = ((String.Format("Error code: {0}", ResPonse.StatusCode)))})
            End If

            ResPonse.EnsureSuccessStatusCode()
            Dim stream_ = Await ResPonse.Content.ReadAsStreamAsync()
            Return stream_
        Catch ex As Exception
            ReportCls.Report(New ReportStatus With {.Finished = True})
            If ex.Message.ToString.ToLower.Contains("a task was canceled") Then
                ReportCls.Report(New ReportStatus With {.TextStatus = ex.Message})
            Else
                Throw CType(ExceptionCls.CreateException(ex.Message, ex.Message), ZohoDocsException)
            End If
        End Try
    End Function
#End Region

#Region "FileRevisionDetails"
    Public Async Function POST_FileRevisionDetails(RevisionType As RevisionTypeEnum) As Task(Of JSON_RevisionMetadata) Implements IItem.F_RevisionMetadata
        Dim parameters = New AuthDictionary From {{"docid", ID}, {"type", RevisionType.ToString}}

        Using localHttpClient As New HttpClient(New HCHandler)
            Dim HtpReqMessage As New Net.Http.HttpRequestMessage([Get], New pUri("revision/details", parameters))
            HtpReqMessage.Content = New FormUrlEncodedContent(parameters)
            Using response As Net.Http.HttpResponseMessage = Await localHttpClient.SendAsync(HtpReqMessage, HttpCompletionOption.ResponseContentRead).ConfigureAwait(False)
                Dim result = Await response.Content.ReadAsStringAsync()

                If response.IsSuccessStatusCode Then
                    Return JsonConvert.DeserializeObject(Of JSON_RevisionMetadata)(result, JSONhandler)
                Else
                    Throw ExceptionCls.CreateException(result.Jobj.SelectToken("response[1].message").ToString, response.StatusCode)
                End If
            End Using
        End Using
    End Function
#End Region

#Region "AddTags"
    Public Async Function POST_AddFileTag(TagsNames As List(Of String)) As Task(Of Boolean) Implements IItem.F_AddTags
        Dim parameters = New AuthDictionary
        parameters.Add("docid", ID)
        parameters.Add("tagname", String.Join(",", TagsNames))

        Using localHttpClient As New HttpClient(New HCHandler)
            Dim HtpReqMessage As New Net.Http.HttpRequestMessage(Post, New pUri("tags/add"))
            HtpReqMessage.Content = New FormUrlEncodedContent(parameters)
            Using response As Net.Http.HttpResponseMessage = Await localHttpClient.SendAsync(HtpReqMessage, Net.Http.HttpCompletionOption.ResponseContentRead).ConfigureAwait(False)
                Dim result = Await response.Content.ReadAsStringAsync()

                If response.IsSuccessStatusCode Then
                    Return result.Jobj.SelectToken("response.result.TagDetails.message").ToString.Contains("SUCCESSFULLY")
                Else
                    Throw ExceptionCls.CreateException(result.Jobj.SelectToken("response.result.TagDetails.message").ToString, response.StatusCode)
                End If
            End Using
        End Using
    End Function
#End Region

#Region "RemoveTags"
    Public Async Function POST_RemoveFileTag(TagsNames As List(Of String)) As Task(Of Boolean) Implements IItem.F_RemoveTags
        Dim parameters = New AuthDictionary
        parameters.Add("docid", ID)
        parameters.Add("tagname", String.Join(",", TagsNames))

        Using localHttpClient As New HttpClient(New HCHandler)
            Dim HtpReqMessage As New Net.Http.HttpRequestMessage(Post, New pUri("tags/remove"))
            HtpReqMessage.Content = New FormUrlEncodedContent(parameters)
            Using response As Net.Http.HttpResponseMessage = Await localHttpClient.SendAsync(HtpReqMessage, Net.Http.HttpCompletionOption.ResponseContentRead).ConfigureAwait(False)
                Dim result = Await response.Content.ReadAsStringAsync()

                If response.IsSuccessStatusCode Then
                    Return result.Jobj.SelectToken("response.result.message").ToString.Contains("SUCCESSFULLY")
                Else
                    Throw ExceptionCls.CreateException(result.Jobj.SelectToken("response.result.message").ToString, response.StatusCode)
                End If
            End Using
        End Using
    End Function
#End Region

#Region "List"
    Public Async Function POST_ListSubFilesAndFolders() As Task(Of JSON_ListFolder) Implements IItem.D_List
        Using localHttpClient As New HttpClient(New HCHandler)
            Dim RequestUri = New Uri(New pUri("folders", New AuthDictionary).ToString & String.Concat("&folderid=", ID))
            Using response As Net.Http.HttpResponseMessage = Await localHttpClient.GetAsync(RequestUri).ConfigureAwait(False)
                Dim result = Await response.Content.ReadAsStringAsync()

                If response.IsSuccessStatusCode Then
                    Return JsonConvert.DeserializeObject(Of JSON_ListFolder)(result, JSONhandler)
                Else
                    Throw ExceptionCls.CreateException(result.Jobj.SelectToken("response[1].message").ToString, response.StatusCode)
                End If
            End Using
        End Using
    End Function
#End Region

End Class
