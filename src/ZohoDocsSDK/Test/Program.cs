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

            var ItemClient = new ZohoDocsSDK.ItemClient(string.Empty);
            interfaces.AddRange(ItemClient.GetType().GetInterfaces());

            var ItemsClient = new ZohoDocsSDK.ItemsClient(new List<string>());
            interfaces.AddRange(ItemsClient.GetType().GetInterfaces());

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
            //ZohoDocsSDK.IClient CLNT   = new ZohoDocsSDK.ZClient("token")

            // set api client with connection options
            ZohoDocsSDK.IClient CLNT = new ZohoDocsSDK.ZClient("token", con);

            // general
            await CLNT.ListAllFiles("folder_id", CategoryEnum._ALL_, 50, 0);
            await CLNT.ListAllFolders("folder_id", 50, 0);
            await CLNT.ListFilesAndFolders("folder_id", 50, 0);
            await CLNT.ListPublicFolder(new Uri("https://www.zoho.com/folder/xxxxxx"));
            await CLNT.ListRoot();
            await CLNT.ListTags();
            CLNT.RootID();
            var cts = new System.Threading.CancellationTokenSource();
            var _ReportCls = new Progress<ZohoDocsSDK.ReportStatus>(delegate (ZohoDocsSDK.ReportStatus ReportClass)
            {
                Console.WriteLine($"{ReportClass.BytesTransferred}/{ReportClass.TotalBytes}");
                Console.WriteLine(ReportClass.ProgressPercentage);
                Console.WriteLine(ReportClass.TextStatus);
            });
            await CLNT.DownloadPublicFile("https://www.zoho.com/file/xxxxxx", "c:\\", "fle.zip", _ReportCls, cts.Token);

            // single
            // [D_] = dir
            // [F_] = file
            // [FD_] = file & dir
            await CLNT.Item("folder_id").D_Copy("folder_id");
            await CLNT.Item("folder_id").D_Create("new folder");
            await CLNT.Item("folder_id").D_List();
            await CLNT.Item("folder_id").D_Upload("c:\\file.mp4", UploadTypes.FilePath, "file.mp4", null, _ReportCls, cts.Token);
            await CLNT.Item("file_id OR folder_id").FD_Delete();
            await CLNT.Item("file_id OR folder_id").FD_Move("folder_id");
            await CLNT.Item("file_id OR folder_id").FD_Rename("new folder name");
            await CLNT.Item("file_id OR folder_id").FD_Share(PermissionEnum.@readonly, "12345");
            await CLNT.Item("file_id OR folder_id").FD_SharesMetadata();
            await CLNT.Item("file_id OR folder_id").FD_Trash();
            await CLNT.Item("file_id OR folder_id").FD_UnShare();
            await CLNT.Item("file_id").F_AddTags(new List<string> { "tag1", "tag2" });
            await CLNT.Item("file_id").F_Copy("folder_id");
            await CLNT.Item("file_id").F_Download("c:\\", "file.zip", null, _ReportCls, cts.Token);
            await CLNT.Item("file_id").F_DownloadAsStream(_ReportCls);
            await CLNT.Item("file_id").F_RemoveTags(new List<string> { "tag1" });
            await CLNT.Item("file_id").F_RevisionMetadata(RevisionTypeEnum.shared);

            // multiple
            await CLNT.Items(new List<string> { "file_id", "folder_id" }).FD_Move("folder_id");
            await CLNT.Items(new List<string> { "folder_id", "folder_id" }).D_Copy("folder_id");
            await CLNT.Items(new List<string> { "file_id", "file_id" }).F_Copy("folder_id");
        }

    }
}
