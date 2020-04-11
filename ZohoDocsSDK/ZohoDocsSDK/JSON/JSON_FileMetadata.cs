using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ZohoDocsSDK.JSON
{
    public class JSON_FileMetadata
    {
        // <JsonProperty("ENCATT_NAME", NullValueHandling:=NullValueHandling.Ignore)>
        // Public Property Name2 As String
        // <JsonProperty("ENCHTML_NAME", NullValueHandling:=NullValueHandling.Ignore)>
        // Public Property Name3 As String
        // <JsonProperty("ENCURL_NAME", NullValueHandling:=NullValueHandling.Ignore)>
        // Public Property Name4 As String
        // <JsonProperty("TRIM_DOCNAME", NullValueHandling:=NullValueHandling.Ignore)>
        // Public Property TrimName As String

        // <JsonProperty("AUTHOR_ID", NullValueHandling:=NullValueHandling.Ignore)>
        // Public Property AuthorID As String
        // <JsonProperty("AUTHOR", NullValueHandling:=NullValueHandling.Ignore)>
        // Public Property AuthorName As String
        // <JsonProperty("AUTHOR_EMAIL", NullValueHandling:=NullValueHandling.Ignore)>
        // Public Property AuthorEmail As String

        // <JsonProperty("LAST_MODIFIEDTIME", NullValueHandling:=NullValueHandling.Ignore)>
        // Public Property _ModifiedDate As String
        // <JsonProperty("LAST_MODIFIEDBY", NullValueHandling:=NullValueHandling.Ignore)>
        // Public Property _LastModifiedByAuthorID As String
        [JsonProperty("LAST_MODIFIEDTIME_IN_MILLISECONDS", NullValueHandling = NullValueHandling.Ignore)]
        public long _LastModified_InMilliseconds { get; set; }
        // <JsonProperty("LAST_MODIFIEDBY_AUTHOR_NAME", NullValueHandling:=NullValueHandling.Ignore)>
        // Public Property _LastModifiedByAuthorName As String
        [JsonProperty("CREATED_TIME_IN_MILLISECONDS", NullValueHandling = NullValueHandling.Ignore)]
        public long _CreatedDate_InMilliseconds { get; set; }
        // <JsonProperty("CREATED_TIME", NullValueHandling:=NullValueHandling.Ignore)>
        // Public Property _CreatedDate As String
        // <JsonProperty("LAST_OPENED_TIME", NullValueHandling:=NullValueHandling.Ignore)>
        // Public Property _LastOpenedDate As String
        [JsonProperty("LAST_OPENED_TIME_IN_MILLISECONDS", NullValueHandling = NullValueHandling.Ignore)]
        public long _LastOpenedDate_InMilliseconds { get; set; }

        [JsonProperty("DOCNAME", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
        [JsonProperty("DOCID", NullValueHandling = NullValueHandling.Ignore)]
        public string ID { get; set; }
        [JsonProperty("FILETYPE", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }
        [JsonProperty("FILE_EXTN", NullValueHandling = NullValueHandling.Ignore)]
        public string Extension { get; set; }
        [JsonProperty("FOLDER_ID", NullValueHandling = NullValueHandling.Ignore)]
        private string _ParentFolderID { get; set; }
        public string ParentFolderID
        {
            get
            {
                return _ParentFolderID == "folder" ? string.Empty : _ParentFolderID;
            }
        }

        // <JsonProperty("SCOPE", NullValueHandling:=NullValueHandling.Ignore)>
        // Public Property Scope As Integer

        [JsonProperty("IS_SHARED", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsShared { get; set; }
        // <JsonProperty("IS_FAVOURITE", NullValueHandling:=NullValueHandling.Ignore)>
        // Public Property IsFavourite As Boolean
        [JsonProperty("IS_LOCKED", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsLocked { get; set; }

        // <JsonProperty("SERVICE_TYPE", NullValueHandling:=NullValueHandling.Ignore)>
        // Public Property ServiceType As String
        [JsonProperty("EXTRA_PROP", NullValueHandling = NullValueHandling.Ignore)]
        public string EXTRA_PROP { get; set; }
    }
}
