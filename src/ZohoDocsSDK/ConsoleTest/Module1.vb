Imports ZohoDocsSDK.utilitiez

Module Module1

    Sub Main()
        'AllTasks.GetAwaiter().GetResult()
        Dim CLNT As ZohoDocsSDK.IClient = New ZohoDocsSDK.ZClient("xxxxxxxxxx")
        Dim outp = CLNT.ListAllFiles(Nothing, CategoryEnum._ALL_, 50, 0).Result
        For Each f In outp
            Console.WriteLine("{0,-80} {1,-50} {2,-50}", "{Name}", "{ID}", "{Type}")
            Console.WriteLine("{0,-80} {1,-50} {2,-50}", f.Name, f.ID, f.Type)
        Next

        Console.WriteLine("press any key to exit...")
        Console.ReadKey()
    End Sub

    Async Function AllTasks() As Task
        'first get auth token (one time only)
        ''API Mode
        Dim tokn_APIMode = ZohoDocsSDK.GetToken.GetAuthTokenFromBrowser
        'OR
        ''Browser Mode
        Dim tokn_BrowserMode = Await ZohoDocsSDK.GetToken.GenerateAuthToken("your_email", "your_password")
        ''set proxy and connection options
        Dim con As New ZohoDocsSDK.ConnectionSettings With {.CloseConnection = True, .TimeOut = TimeSpan.FromMinutes(30), .Proxy = New ZohoDocsSDK.ProxyConfig With {.SetProxy = True, .ProxyIP = "127.0.0.1", .ProxyPort = 8888, .ProxyUsername = "user", .ProxyPassword = "pass"}}
        ''set api client
        Dim CLNT As ZohoDocsSDK.IClient = New ZohoDocsSDK.ZClient("xxxxxxxx")

        ''general
        Await CLNT.ListAllFiles("folder_id", CategoryEnum._ALL_, 50, 0)
        Await CLNT.ListAllFolders("folder_id", 50, 0)
        Await CLNT.ListFilesAndFolders("folder_id", 50, 0)
        Await CLNT.ListPublicFolder(New Uri("https://www.zoho.com/folder/xxxxxx"))
        Await CLNT.ListRoot()
        Await CLNT.ListTags()
        CLNT.RootID()
        Dim cts As New Threading.CancellationTokenSource()
        Dim _ReportCls As New Progress(Of ZohoDocsSDK.ReportStatus)(Sub(ReportClass As ZohoDocsSDK.ReportStatus)
                                                                        Console.WriteLine(String.Format("{0}/{1}", (ReportClass.BytesTransferred), (ReportClass.TotalBytes)))
                                                                        Console.WriteLine(CInt(ReportClass.ProgressPercentage))
                                                                        Console.WriteLine(ReportClass.TextStatus)
                                                                    End Sub)
        Await CLNT.DownloadPublicFile("https://www.zoho.com/file/xxxxxx", "c:\\", "fle.zip", _ReportCls, cts.Token)

        ''single
        '' [D_] = dir
        '' [F_] = file
        '' [FD_] = file & dir
        Await CLNT.Item("folder_id").D_Copy("folder_id")
        Await CLNT.Item("folder_id").D_Create("new folder")
        Await CLNT.Item("folder_id").D_List()
        Await CLNT.Item("folder_id").D_Upload("c:\\file.mp4", UploadTypes.FilePath, "file.mp4", Nothing, _ReportCls, cts.Token)
        Await CLNT.Item("file_id OR folder_id").FD_Delete
        Await CLNT.Item("file_id OR folder_id").FD_Move("folder_id")
        Await CLNT.Item("file_id OR folder_id").FD_Rename("new folder name")
        Await CLNT.Item("file_id OR folder_id").FD_Share(PermissionEnum.readonly, "12345", Nothing)
        Await CLNT.Item("file_id OR folder_id").FD_SharesMetadata
        Await CLNT.Item("file_id OR folder_id").FD_Trash
        Await CLNT.Item("file_id OR folder_id").FD_UnShare
        Await CLNT.Item("file_id").F_AddTags(New List(Of String) From {"tag1", "tag2"})
        Await CLNT.Item("file_id").F_Copy("folder_id")
        Await CLNT.Item("file_id").F_Download("c:\\", "file.zip", Nothing, _ReportCls, cts.Token)
        Await CLNT.Item("file_id").F_DownloadAsStream(_ReportCls, Nothing)
        Await CLNT.Item("file_id").F_RemoveTags(New List(Of String) From {"tag1"})
        Await CLNT.Item("file_id").F_RevisionMetadata(RevisionTypeEnum.shared)

        ''multiple
        Await CLNT.Items(New List(Of String) From {"file_id", "folder_id"}).FD_Move("folder_id")
        Await CLNT.Items(New List(Of String) From {"folder_id", "folder_id"}).D_Copy("folder_id")
        Await CLNT.Items(New List(Of String) From {"file_id", "file_id"}).F_Copy("folder_id")

    End Function

End Module
