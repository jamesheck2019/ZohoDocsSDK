using System.Threading.Tasks;

namespace ZohoDocsSDK
{
    public interface IFiles
    {

        /// <summary>
        ///Copies multiple files to a new location
        ///https://apidocs.zoho.com/files/v1/copy?authtoken=AuthToken&scope=docsapi
        ///</summary>
        ///<param name="DestinationFolderID">Mandatory. Id of folder to copy the new files</param>
        Task<bool> Copy(string DestinationFolderID);

        /// <summary>
        ///Moves multiple files to a specified location
        ///</summary>
        ///<param name="DestinationFolderID">Mandatory. Id of folder to copy the new files/folders</param>
        Task<bool> Move(string DestinationFolderID);
    }
}