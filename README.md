# ZohoDocsSDK
Zoho.Docs SDK for .NET

`Download:`[https://github.com/loudKode/ZohoDocsSDK/releases](https://github.com/loudKode/ZohoDocsSDK/releases)<br>
`NuGet:`
[![NuGet](https://img.shields.io/nuget/v/DeQmaTech.ZohoDocsSDK.svg?style=flat-square&logo=nuget)](https://www.nuget.org/packages/DeQmaTech.ZohoDocsSDK)<br>

**Features**
* Assemblies for .NET 4.5.2 and .NET Standard 2.0
* Just one external reference (Newtonsoft.Json)
* Easy installation using NuGet
* Upload/Download tracking support
* Proxy Support
* Upload/Download cancellation support


# List of functions:
* TrashFileFolder
* DeleteFileFolder
* RenameFileFolder
* MoveFileFolder
* MoveMultipleFileFolder
* CopyFile
* CopyMultipleFile
* CopyFolder
* CopyMultipleFolder
* DownloadFile
* DownloadFileAsStream
* Upload
* FileRevisionDetails
* CreateNewFolder
* ListAllFiles
* ListAllFolders
* List
* List2
* ListRootFilesFolders
* PublicFileFolder
* UnPublicFileFolder
* SharedDetails
* ListPublicLink
* DownloadPublicFile
* ListTags
* AddFileTag
* RemoveFileTag

# Code simple:
**get token**
```vb
Dim tkn = Await ZohoDocsSDK.GetToken.GenerateAuthToken("user", "pass")
```
**set client**
```vb
Dim Clnt As ZohoDocsSDK.IClient = New ZohoDocsSDK.ZClient(tkn.token)
```
**set client with proxy**
```vb
Dim m_proxy = New ZohoDocsSDK.ProxyConfig With {.SetProxy = True, .ProxyIP = "172.0.0.0", .ProxyPort = 80, .ProxyUsername = "usr", .ProxyPassword = "pas"}
Dim Clnt As ZohoDocsSDK.IClient = New ZohoDocsSDK.ZClient(tkn.token,m_proxy)
```
**list root files/folders**
```vb
Dim RSLT = Await TOK.List(Nothing)
For Each onz In RSLT.Files
    DataGridView1.Rows.Add(onz.Name, onz.ID, onz.Extension, onz.IsLocked, onz.Type, onz.IsShared)
Next
Dim RSLT2 = Await TOK.ListAllFiles(ZohoDocsSDK.utilities.Category.videos, 0, 50)
For Each onz In RSLT2.Files
    DataGridView1.Rows.Add(onz.Name, onz.ID, onz.Extension, onz.ParentFolderID, onz.Type, onz.IsShared)
Next
```
**upload local file (without progress tracking)**
```vb.net
Dim UploadCancellationToken As New Threading.CancellationTokenSource()
Dim RSLT = TOK.Upload("C:\m_118.png", "m_118.png", ZohoDocsSDK.ZClient.UploadTypes.FilePath, Nothing, Nothing, nothing, UploadCancellationToken.Token)
```
**upload local file with progress tracking**
```vb.net
Dim UploadCancellationToken As New Threading.CancellationTokenSource()
Dim progress_ReportCls As New Progress(Of ZohoDocsSDK.ReportStatus)(Sub(ReportClass As ZohoDocsSDK.ReportStatus)
                         Label1.Text = String.Format("{0}/{1}", (ReportClass.BytesTransferred), (ReportClass.TotalBytes))
                         ProgressBar1.Value = CInt(ReportClass.ProgressPercentage)
                         Label2.Text = CStr(ReportClass.TextStatus)
                         End Sub)
Dim RSLT = TOK.Upload("C:\m_118.png", "m_118.png", ZohoDocsSDK.ZClient.UploadTypes.FilePath, Nothing, Nothing, progress_ReportCls, UploadCancellationToken.Token)
```
