using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ZohoDocsSDK
{
  public   interface IItems
    {
        /// <summary>
        ///Moves multiple files/folders to a specified location
        ///</summary>
        ///<param name="DestinationFolderID">Mandatory. Id of folder to copy the new files/folders</param>
        Task<bool> FD_Move(string DestinationFolderID);
        /// <summary>
        ///Copies multiple files to a new location
        ///https://apidocs.zoho.com/files/v1/copy?authtoken=AuthToken&scope=docsapi
        ///</summary>
        ///<param name="DestinationFolderID">Mandatory. Id of folder to copy the new files</param>
        Task<bool> F_Copy(string DestinationFolderID);
        /// <summary>
        ///Copies the existing folders to a specified folder
        ///https://apidocs.zoho.com/files/v1/folders/copy?authtoken=AuthToken&scope=docsapi
        ///</summary>
        ///<param name="DestinationFolderID">Mandatory. Id of folder to copy the new folders</param>
        Task<bool> D_Copy(string DestinationFolderID);
    }
}
