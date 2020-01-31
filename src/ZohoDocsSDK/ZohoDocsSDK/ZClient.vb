Imports ZohoDocsSDK.JSON
Imports ZohoDocsSDK.utilitiez
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.Net.Http

Public Class ZClient
    Implements IClient

    Public Sub New(accessToken As String, Optional Settings As ConnectionSettings = Nothing)
        authToken = accessToken

        ConnectionSetting = Settings
        If Settings Is Nothing Then
            m_proxy = Nothing
        Else
            m_proxy = Settings.Proxy
            m_CloseConnection = If(Settings.CloseConnection, True)
            m_TimeOut = If(Settings.TimeOut = Nothing, TimeSpan.FromMinutes(60), Settings.TimeOut)
        End If
        Net.ServicePointManager.Expect100Continue = True : Net.ServicePointManager.SecurityProtocol = Net.SecurityProtocolType.Tls Or Net.SecurityProtocolType.Tls11 Or Net.SecurityProtocolType.Tls12 Or Net.SecurityProtocolType.Ssl3
    End Sub


    Public ReadOnly Property Items(IDs As List(Of String)) As IItems Implements IClient.Items
        Get
            Return New ItemsClient(IDs)
        End Get
    End Property
    Public ReadOnly Property Item(ID As String) As IItem Implements IClient.Item
        Get
            Return New ItemClient(ID)
        End Get
    End Property



#Region "ListAllFiles"
    Public Async Function _ListAllFiles(DestinationFolderID As String, Optional Filter As CategoryEnum = CategoryEnum._ALL_, Optional Limit As Integer? = 200, Optional Offset As Integer? = 0) As Task(Of List(Of JSON_FileMetadata)) Implements IClient.ListAllFiles
        Dim parameters = New AuthDictionary
        If DestinationFolderID IsNot Nothing Then parameters.Add("folderid", DestinationFolderID)
        If Not Filter = CategoryEnum._ALL_ Then parameters.Add("category", Filter.ToString)
        If Offset.HasValue Then parameters.Add("start", Offset.Value)
        If Limit.HasValue Then parameters.Add("limit", Limit.Value)
        ''''''''''''''''''''''''''''''''''
        Using localHttpClient As New HttpClient(New HCHandler)
            Using response As HttpResponseMessage = Await localHttpClient.GetAsync(New pUri("files", parameters), Net.Http.HttpCompletionOption.ResponseContentRead).ConfigureAwait(False)
                Dim result = Await response.Content.ReadAsStringAsync()

                If response.IsSuccessStatusCode Then
                    Return JsonConvert.DeserializeObject(Of List(Of JSON_FileMetadata))(result.Jobj.SelectToken("FILES").ToString, JSONhandler)
                Else
                    Throw ExceptionCls.CreateException(result.Jobj.SelectToken("response[1].message").ToString, response.StatusCode)
                End If
            End Using
        End Using
    End Function
#End Region

#Region "ListAllFolders"
    Public Async Function _ListAllFolders(DestinationFolderID As String, Optional Limit As Integer? = 200, Optional Offset As Integer? = 0) As Task(Of List(Of JSON_FolderMetadata)) Implements IClient.ListAllFolders
        Dim parameters = New AuthDictionary
        If DestinationFolderID IsNot Nothing Then parameters.Add("folderid", DestinationFolderID)
        If Offset.HasValue Then parameters.Add("start", Offset.Value)
        If Limit.HasValue Then parameters.Add("limit", Limit.Value)

        Using localHttpClient As New HttpClient(New HCHandler)
            Using response As HttpResponseMessage = Await localHttpClient.GetAsync(New pUri("folders", parameters), HttpCompletionOption.ResponseContentRead).ConfigureAwait(False)
                Dim result = Await response.Content.ReadAsStringAsync()

                If response.IsSuccessStatusCode Then
                    Return JsonConvert.DeserializeObject(Of List(Of JSON_FolderMetadata))(result.Jobj.SelectToken("FOLDER").ToString, JSONhandler)
                Else
                    Throw ExceptionCls.CreateException(result.Jobj.SelectToken("response[1].message").ToString, response.StatusCode)
                End If
            End Using
        End Using
    End Function
#End Region

#Region "ListFilesAndFolders"
    Public Async Function _ListFilesAndFolders(DestinationFolderID As String, Optional Limit As Integer? = 200, Optional Offset As Integer? = 0) As Task(Of JSON_ListFolder) Implements IClient.ListFilesAndFolders
        Dim parameters = New AuthDictionary
        If DestinationFolderID IsNot Nothing Then parameters.Add("folderid", DestinationFolderID)
        If Offset.HasValue Then parameters.Add("start", Offset.Value)
        If Limit.HasValue Then parameters.Add("limit", Limit.Value)

        Using localHttpClient As New HttpClient(New HCHandler)
            Using response As Net.Http.HttpResponseMessage = Await localHttpClient.GetAsync(New pUri("folders/files", parameters)).ConfigureAwait(False)
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

#Region "ListRootFilesFolders"
    Public Async Function _ListRootFilesFolders() As Task(Of JSON_ListFolder) Implements IClient.ListRoot
        Using localHttpClient As New HttpClient(New HCHandler)
            Dim RequestUri = New pUri("folders/files", New AuthDictionary)
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

#Region "RootID"
    Public Function POST_RootID() As String Implements IClient.RootID
        Return "1"
    End Function
#End Region

#Region "ListPublicLink"
    Public Async Function _ListPublicLink(PublicFolderUrl As Uri) As Task(Of JSON_ListPublicLink) Implements IClient.ListPublicFolder
        If PublicFolderUrl.ToString.Contains("/folder/") Then Throw CType(ExceptionCls.CreateException("Not a folder url", 404), ZohoDocsException)
        Try
            Dim tsk = Await Task.Factory.StartNew(Function()
                                                      Dim hr As New DeQmaTek.TcpClientHttpRequest()
                                                      hr.Action = PublicFolderUrl.ToString '"https://apidocs.zoho.com/folder/buoe2b5ddc16d217447eb9077d60c01b595a2"
                                                      hr.Method = "GET"
                                                      'hr.ContentType = "application/json"
                                                      'hr.Accept = "application/json"
                                                      hr.UserAgent = "DeQma.TcpClientHttp"
                                                      hr.Send()
                                                      Dim result = System.Text.Encoding.UTF8.GetString(hr.Response.GetStream)
                                                      Dim partONE As String = "var folderObj = "
                                                      Dim partTWO As String = ";"
                                                      'Dim FullUrl As String() = result.Split(New String() {partONE, partTWO}, StringSplitOptions.None)
                                                      Dim ExtractTheJSON = Between(result, partONE, partTWO)
                                                      Dim TheRsult = JsonConvert.DeserializeObject(Of JSON_ListPublicLink)(ExtractTheJSON, JSONhandler)
                                                      Return TheRsult
                                                  End Function)
            Return tsk
        Catch ex As Exception
            Throw ExceptionCls.CreateException(ex.Message, ex.Message)
        End Try
    End Function
#End Region

#Region "DownloadPublicFile"
    Public Async Function _DownloadPublicFile(FilePublicUrl As String, FileSaveDir As String, FileName As String, Optional ReportCls As IProgress(Of ReportStatus) = Nothing, Optional token As Threading.CancellationToken = Nothing) As Task Implements IClient.DownloadPublicFile
        If ReportCls Is Nothing Then ReportCls = New Progress(Of ReportStatus)
        ReportCls.Report(New ReportStatus With {.Finished = False, .TextStatus = "Initializing..."})
        Try
            Dim progressHandler As New Net.Http.Handlers.ProgressMessageHandler(New HCHandler)
            AddHandler progressHandler.HttpReceiveProgress, (Function(sender, e)
                                                                 ReportCls.Report(New ReportStatus With {.ProgressPercentage = e.ProgressPercentage, .BytesTransferred = e.BytesTransferred, .TotalBytes = If(e.TotalBytes Is Nothing, 0, e.TotalBytes), .TextStatus = "Downloading..."})
                                                             End Function)
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            Dim localHttpClient As New HttpClient(progressHandler)
            '''''''''''''''''will write the whole content to H.D WHEN download completed'''''''''''''''''''''''''''''
            Using ResPonse As Net.Http.HttpResponseMessage = Await localHttpClient.GetAsync(New Uri(FilePublicUrl), Net.Http.HttpCompletionOption.ResponseContentRead, token).ConfigureAwait(False)
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
                Throw ExceptionCls.CreateException(ex.Message, ex.Message)
            End If
        End Try
    End Function
#End Region

#Region "ListTags"
    Public Async Function _Tags_List() As Task(Of List(Of JSON_TagMetadata)) Implements IClient.ListTags
        Using localHttpClient As New HttpClient(New HCHandler)
            Dim RequestUri = New pUri("tags", New AuthDictionary)
            Using response As Net.Http.HttpResponseMessage = Await localHttpClient.GetAsync(RequestUri).ConfigureAwait(False)
                Dim result = Await response.Content.ReadAsStringAsync()

                If response.IsSuccessStatusCode Then
                    Return JsonConvert.DeserializeObject(Of List(Of JSON_TagMetadata))(JObject.Parse(result)("UserTagDetails").ToString, JSONhandler)
                Else
                    Throw ExceptionCls.CreateException(result.Jobj.SelectToken("response[1].message").ToString, response.StatusCode)
                End If
            End Using
        End Using
    End Function
#End Region


End Class
