using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static ZohoDocsSDK.Basic;

namespace ZohoDocsSDK
{
   public class FoldersClient : IFolders
    {

        private string[] FoldersID { get; set; }
        public FoldersClient(string[] FoldersID) => this.FoldersID = FoldersID;

        public async Task<bool> Move(string DestinationFolderID)
        {
            ZClient client = new ZClient(authToken, ConnectionSetting);
            return await client.Folder(string.Join(",", FoldersID)).Move(DestinationFolderID);
        }

        public async Task<bool> Copy(string DestinationFolderID)
        {
            ZClient client = new ZClient(authToken, ConnectionSetting);
            return await client.Folder(string.Join(",", FoldersID)).Copy(DestinationFolderID);
        }


    }
}
