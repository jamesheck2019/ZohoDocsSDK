using System.Threading.Tasks;

namespace ZohoDocsSDK
{
    public interface IFolders
    {

        /// <summary>
        ///Copies the existing folders to a specified folder
        ///https://apidocs.zoho.com/files/v1/folders/copy?authtoken=AuthToken&scope=docsapi
        ///</summary>
        ///<param name="DestinationFolderID">Mandatory. Id of folder to copy the new folders</param>
        Task<bool> Copy(string DestinationFolderID);

        /// <summary>
        ///Moves multiple folders to a specified location
        ///</summary>
        ///<param name="DestinationFolderID">Mandatory. Id of folder to copy the new files/folders</param>
        Task<bool> Move(string DestinationFolderID);
    }
}