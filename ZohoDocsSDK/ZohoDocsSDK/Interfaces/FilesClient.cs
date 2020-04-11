using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static ZohoDocsSDK.Basic;

namespace ZohoDocsSDK
{
   public  class FilesClient : IFiles
    {

        private string[] FilesID { get; set; }
        public FilesClient(string[] FilesID) => this.FilesID = FilesID;

        public async Task<bool> Move(string DestinationFolderID)
        {
            ZClient client = new ZClient(authToken, ConnectionSetting);
            return await client.File(string.Join(",", FilesID)).Move(DestinationFolderID);
        }

        public async Task<bool> Copy(string DestinationFolderID)
        {
            ZClient client = new ZClient(authToken, ConnectionSetting);
            return await client.File(string.Join(",", FilesID)).Copy(DestinationFolderID);
        }


    }
}
