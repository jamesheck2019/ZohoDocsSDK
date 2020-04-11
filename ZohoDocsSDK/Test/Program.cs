using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static ZohoDocsSDK.Utilitiez;

namespace Test
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //'Dim CLNT As ZohoDocsSDK.IClient = New ZohoDocsSDK.ZClient("")
            //'Dim outp = CLNT.ListAllFiles(Nothing, CategoryEnum._ALL_, 50, 0).Result
            //'For Each f In outp
            //'    Console.WriteLine("{0,-80} {1,-50} {2,-50}", "{Name}", "{ID}", "{Type}")
            //'    Console.WriteLine("{0,-80} {1,-50} {2,-50}", f.Name, f.ID, f.Type)
            //'Next


            List<System.Type> interfaces = new List<System.Type>();

            var FileClient = new ZohoDocsSDK.FileClient(string.Empty);
            interfaces.AddRange(FileClient.GetType().GetInterfaces());

            var FolderClient = new ZohoDocsSDK.FolderClient(string.Empty);
            interfaces.AddRange(FolderClient.GetType().GetInterfaces());

            var FilesClient = new ZohoDocsSDK.FilesClient(new string[] { });
            interfaces.AddRange(FilesClient.GetType().GetInterfaces());

            var FoldersClient = new ZohoDocsSDK.FoldersClient(new string[] { });
            interfaces.AddRange(FoldersClient.GetType().GetInterfaces());

            var ZClient = new ZohoDocsSDK.ZClient(string.Empty, null);
            interfaces.AddRange(ZClient.GetType().GetInterfaces());

            foreach (Type iface in interfaces)
            {
                var methods = iface.GetMethods();
                foreach (System.Reflection.MethodInfo method in methods)
                {
                    var methodName = method.Name;
                    Console.WriteLine(methodName);
                }
            }

            Console.WriteLine("press any key to exit...");
            Console.ReadKey();
        }

        private async Task AllTasks()
        {
            //'first get auth token (one time only)
            // API Mode
            var tokn_APIMode = ZohoDocsSDK.Authentication.GetAuthTokenFromBrowser();
            //'OR
            // Browser Mode
            var tokn_BrowserMode = await ZohoDocsSDK.Authentication.GenerateAuthToken("your_email", "your_password");
            // set proxy and connection options
            var con = new ZohoDocsSDK.ConnectionSettings { CloseConnection = true, TimeOut = TimeSpan.FromMinutes(30), Proxy = new ZohoDocsSDK.ProxyConfig { SetProxy = true, ProxyIP = "127.0.0.1", ProxyPort = 8888, ProxyUsername = "user", ProxyPassword = "pass" } };
            // set api client without connection options
            //ZohoDocsSDK.IClient CLNT   = new ZohoDocsSDK.ZClient("403e45315ec42b3ba9c367515084ed7b")

            // set api client with connection options
            ZohoDocsSDK.IClient CLNT = new ZohoDocsSDK.ZClient("403e45315ec42b3ba9c367515084ed7b", con);

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
            await CLNT.Folder("file_id OR folder_id").Delete();
            await CLNT.Folder("file_id OR folder_id").Move("folder_id");
            await CLNT.Folder("file_id OR folder_id").Rename("new folder name");
            await CLNT.Folder("file_id OR folder_id").Share(PermissionEnum.@readonly, "12345");
            await CLNT.Folder("file_id OR folder_id").SharedDetails();
            await CLNT.Folder("file_id OR folder_id").Trash();
            await CLNT.Folder("file_id OR folder_id").UnShare();
            await CLNT.Folder("file_id").ListSubFilesRecursively();
            await CLNT.Folder("file_id").ListSubFoldersRecursively();
            await CLNT.Folder("file_id").Copy("folder_id");


            // multiple
            await CLNT.Files(new string[] { "file_id", "file_id" }).Move("folder_id");
            await CLNT.Files(new string[] { "file_id", "file_id" }).Copy("folder_id");
            await CLNT.Folders(new string[] { "folder_id", "folder_id" }).Copy("folder_id");
            await CLNT.Folders(new string[] { "folder_id", "folder_id" }).Move("folder_id");


        }

    }
}
