## ZohoDocsSDK

`Download:`[https://github.com/jamesheck2019/ZohoDocsSDK/releases](https://github.com/jamesheck2019/ZohoDocsSDK/releases)<br>
`NuGet:`
[![NuGet](https://img.shields.io/nuget/v/DeQmaTech.ZohoDocsSDK.svg?style=flat-square&logo=nuget)](https://www.nuget.org/packages/DeQmaTech.ZohoDocsSDK)<br>
`Help:`
[https://github.com/jamesheck2019/ZohoDocsSDK/wiki](https://github.com/jamesheck2019/ZohoDocsSDK/wiki)<br>

# Features
* Assemblies for .NET 4.5.2 and .NET Standard 2.0 and .NET Core 2.1
* Just one external reference (Newtonsoft.Json)
* Easy installation using NuGet
* Upload/Download tracking support
* Proxy Support
* Upload/Download cancellation support


# List of functions:
**Token**
> * GenerateAuthToken
> * GetAuthTokenFromBrowser

**File**
> * AddTags
> * Copy
> * Delete
> * Download
> * DownloadAsStream
> * Move
> * RemoveTags
> * Rename
> * RevisionMetadata
> * Share
> * SharedDetails
> * Trash
> * UnShare

**Folder**
> * Copy
> * Create
> * Delete
> * List
> * List2
> * ListSubFilesRecursively
> * ListSubFoldersRecursively
> * Move
> * Rename
> * Share
> * SharedDetails
> * Trash
> * UnShare
> * Upload

**Files**
> * Copy
> * Move

**Folders**
> * Copy
> * Move

**Root**
> * RootID
> * Create
> * List
> * ListSubFilesRecursively
> * ListSubFoldersRecursively
> * Upload
> * ListPublicFolder
> * DownloadPublicFile
> * ListTags




# CodeMap:
![codemap](https://i.postimg.cc/xjWxYWZN/zd-Codemap.png)

# Code simple:
```csharp
//'first get auth token (one time only)
// API Mode
var tokn_APIMode = ZohoDocsSDK.Authentication.GetAuthTokenFromBrowser();
//'OR
// Browser Mode
var tokn_BrowserMode = await ZohoDocsSDK.Authentication.GenerateAuthToken("your_email", "your_password");
// set proxy and connection options
var con = new ZohoDocsSDK.ConnectionSettings { CloseConnection = true, TimeOut = TimeSpan.FromMinutes(30), Proxy = new ZohoDocsSDK.ProxyConfig { SetProxy = true, ProxyIP = "127.0.0.1", ProxyPort = 8888, ProxyUsername = "user", ProxyPassword = "pass" } };
// set api client without connection options
//ZohoDocsSDK.IClient CLNT   = new ZohoDocsSDK.ZClient("xxxxxxxtoknxxxxxxx")

// set api client with connection options
ZohoDocsSDK.IClient CLNT = new ZohoDocsSDK.ZClient("xxxxxxxtoknxxxxxxx", con);

// general
await CLNT.ListPublicFolder(new Uri("https://www.zoho.com/folder/xxxxxx"));
await CLNT.ListTags();

var cts = new System.Threading.CancellationTokenSource();
var _ReportCls = new Progress<ZohoDocsSDK.ReportStatus>(delegate (ZohoDocsSDK.ReportStatus ReportClass)
{
    Console.WriteLine($"{ReportClass.BytesTransferred}/{ReportClass.TotalBytes}");
    Console.WriteLine(ReportClass.ProgressPercentage);
    Console.WriteLine(ReportClass.TextStatus);
});
await CLNT.DownloadPublicFile("https://www.zoho.com/file/xxxxxx", "c:\\", "fle.zip", _ReportCls, cts.Token);

//Root
await CLNT.Root().List();
await CLNT.Root().ListSubFilesRecursively();
await CLNT.Root().ListSubFoldersRecursively();
await CLNT.Root().Upload("c:\\file.mp4", UploadTypes.FilePath, "file.mp4", null, _ReportCls, cts.Token);

// single File
await CLNT.File("file_id").Copy("folder_id");
await CLNT.File("file_id").Move("folder_id");
await CLNT.File("file_id").Trash();
await CLNT.File("file_id").Delete();
await CLNT.File("file_id").AddTags(new string[] { "tag1","tag2"});
await CLNT.File("file_id").RemoveTags (new string[] { "tag1", "tag2" });
await CLNT.File("file_id").Download("c:\\downloads", "raw.zip",null,_ReportCls,cts.Token);
await CLNT.File("file_id").DownloadAsStream(_ReportCls, cts.Token);
await CLNT.File("file_id").Rename("raw.zip");
await CLNT.File("file_id").RevisionMetadata( RevisionTypeEnum.shared);
await CLNT.File("file_id").Share( PermissionEnum.@readonly,"1234",DateTime.Now);
await CLNT.File("file_id").UnShare();
await CLNT.File("file_id").SharedDetails();

// single Folder
await CLNT.Folder("folder_id").Create("new folder");
await CLNT.Folder("folder_id").List();
await CLNT.Folder("folder_id").List2(60,0);
await CLNT.Folder("folder_id").Upload("c:\\file.mp4", UploadTypes.FilePath, "file.mp4", null, _ReportCls, cts.Token);
await CLNT.Folder("folder_id").Delete();
await CLNT.Folder("folder_id").Move("folder_id");
await CLNT.Folder("folder_id").Rename("new folder name");
await CLNT.Folder("folder_id").Share(PermissionEnum.@readonly, "12345");
await CLNT.Folder("folder_id").SharedDetails();
await CLNT.Folder("folder_id").Trash();
await CLNT.Folder("folder_id").UnShare();
await CLNT.Folder("folder_id").ListSubFilesRecursively();
await CLNT.Folder("folder_id").ListSubFoldersRecursively();
await CLNT.Folder("folder_id").Copy("folder_id");

// multiple
await CLNT.Files(new string[] { "file_id", "file_id" }).Move("folder_id");
await CLNT.Files(new string[] { "file_id", "file_id" }).Copy("folder_id");
await CLNT.Folders(new string[] { "folder_id", "folder_id" }).Copy("folder_id");
await CLNT.Folders(new string[] { "folder_id", "folder_id" }).Move("folder_id");
```
