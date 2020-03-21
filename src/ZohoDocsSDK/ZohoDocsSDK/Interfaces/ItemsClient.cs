using System.Collections.Generic;
using System.Threading.Tasks;
using static ZohoDocsSDK.Basic;

namespace ZohoDocsSDK
{
    public  class ItemsClient : IItems 
    {
        private List<string> IDs { get; set; }
        public ItemsClient(List<string> IDs)
        {
            this.IDs = IDs;
        }


        #region MoveMultipleFileFolder
        public async Task<bool> FD_Move(string DestinationFolderID)
        {
            ZClient client = new ZClient(authToken, ConnectionSetting);
            return await client.Item(string.Join(",", IDs)).FD_Move(DestinationFolderID);
        }
        #endregion

        #region CopyMultipleFile
        public async Task<bool> F_Copy(string DestinationFolderID)
        {
            ZClient client = new ZClient(authToken, ConnectionSetting);
            return await client.Item(string.Join(",", IDs)).F_Copy(DestinationFolderID);
        }
        #endregion

        #region CopyMultipleFolder
        public async Task<bool> D_Copy(string DestinationFolderID)
        {
            ZClient client = new ZClient(authToken, ConnectionSetting);
            return await client.Item(string.Join(",", IDs)).D_Copy(DestinationFolderID);
        }
        #endregion
    }
}
