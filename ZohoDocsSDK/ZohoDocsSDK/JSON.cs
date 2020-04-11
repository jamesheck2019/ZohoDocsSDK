using Newtonsoft.Json;
using System.Collections.Generic;

namespace ZohoDocsSDK.JSON
{

    #region JSON_CreateNewFolder
    public class JSON_NewFolder
    {
        public string folder_name { get; set; }
        public string folder_id { get; set; }
    }
    #endregion

    #region JSON_ListPublicLink

    public class JSON_ListPublicLink
    {
        [JsonProperty("DOC")]
        public List<JSON_DOC> ResultsList { get; set; }

        public List<JSON_TOTALDOC> TOTALDOCS { get; set; }
        public string Total
        {
            get
            {
                return TOTALDOCS[0].NOOFDOCS.ToString();
            }
        }
    }

    public class JSON_DOC
    {
        [JsonProperty("IS_FAVOURITE")]
        public bool IsFavourite { get; set; }
        [JsonProperty("IS_LOCKED")]
        public bool IsLocked { get; set; }
        [JsonProperty("SCOPE")]
        public int Scope { get; set; }
        [JsonProperty("SERVICE_TYPE")]
        public string ServiceType { get; set; }
        [JsonProperty("FOLDER_ID")]
        public string ParentFolderID { get; set; }
        [JsonProperty("FOLDER_NAME")]
        public string ParentFolderName { get; set; }
        [JsonProperty("DOCNAME")]
        public string Name { get; set; }
        [JsonProperty("DOCID")]
        public string ID { get; set; }
        [JsonProperty("FILETYPE")]
        public string Type { get; set; }
        [JsonProperty("FILE_EXTN")]
        public string Extension { get; set; }
        [JsonProperty("LAST_MODIFIEDTIME")]
        public string _ModifiedDate { get; set; }
        [JsonProperty("LAST_MODIFIEDBY")]
        public string _LastModifiedByAuthorID { get; set; }
        [JsonProperty("LAST_MODIFIEDTIME_IN_MILLISECONDS")]
        public long _LastModified_InMilliseconds { get; set; }
        [JsonProperty("LAST_MODIFIEDBY_AUTHOR_NAME")]
        public string _LastModifiedByAuthorName { get; set; }
        [JsonProperty("CREATED_TIME_IN_MILLISECONDS")]
        public long _CreatedDate_InMilliseconds { get; set; }
        [JsonProperty("CREATED_TIME")]
        public string _CreatedDate { get; set; }
        [JsonProperty("LAST_OPENEDTIME")]
        public string _LastOpenedDate { get; set; }
        [JsonProperty("LAST_OPENEDTIME_IN_MILLISECONDS")]
        public long _LastOpenedDate_InMilliseconds { get; set; }
        [JsonProperty("AUTHOR_ID")]
        public string AuthorID { get; set; }
        [JsonProperty("DOC_AUTHOR_NAME")]
        public string AuthorName { get; set; }
        [JsonProperty("AUTHOR_EMAIL")]
        public string AuthorEmail { get; set; }

        public int PERMISSION { get; set; }
        public string SHARED_TYPE { get; set; }
        public bool IS_DOC_SHARED { get; set; }

        public enum FileFolder
        {
            file,
            folder
        }
        public FileFolder FileOrFolder
        {
            get
            {
                return Type == FileFolder.folder.ToString() ? FileFolder.folder : FileFolder.file;
            }
        }
        public string Url
        {
            get
            {
                return FileOrFolder == FileFolder.folder ? string.Format("https://apidocs.zoho.com/folder/{0}", ID) : string.Format("https://apidocs.zoho.com/file/{0}", ID);
            }
        }
        public string FileDownloadLink
        {
            get
            {
                return FileOrFolder == FileFolder.file ? string.Format("https://docs.zoho.com/downloaddocument.do?docId={0}&docExtn={1}", ID, Extension) : null;
            }
        }
        public JSON_FileJSON FileJSON
        {
            get
            {
                if (FileOrFolder == FileFolder.file)
                {
                    DeQmaTek.TcpClientHttpRequest hr = new DeQmaTek.TcpClientHttpRequest();
                    hr.Action = string.Format("https://docs.zoho.com/services/oembed?type=json&url={0}", Url);
                    hr.Method = "GET";
                    hr.UserAgent = "DeQma.TcpClientHttp";
                    hr.Send();
                    var result = System.Text.Encoding.UTF8.GetString(hr.Response.GetStream());
                    return JsonConvert.DeserializeObject<JSON_FileJSON>(result, Basic.JSONhandler);
                }
                else { return null; }
            }
        }
    }

    public class JSON_TOTALDOC
    {
        public int NOOFDOCS { get; set; }
    }

    public class JSON_FileJSON
    {
        public string author_name { get; set; }
        public string author_url { get; set; }
        public int thumbnail_width { get; set; }
        public string description { get; set; }
        public string provider_url { get; set; }
        public string title { get; set; }
        public string provider_name { get; set; }
        public string thumbnail_url { get; set; }
        public string type { get; set; }
        public string version { get; set; }
        public string url { get; set; }
        public int thumbnail_height { get; set; }
    }

    #endregion

    #region JSON_RevisionMetadata
    public class JSON_RevisionMetadata
    {
        [JsonProperty("DOCID")]
        public string ID { get; set; }
        [JsonProperty("FORMAT")]
        public string Extension { get; set; }
        [JsonProperty("DOCNAME")]
        public string Name { get; set; }
        [JsonProperty("DOCTYPE")]
        public string Type { get; set; }
        [JsonProperty("VERSION_INFO")]
        public List<frd4_VERSION_INFO> VersionsList { get; set; }
    }
    public class frd4_VERSION_INFO
    {
        [JsonProperty("DOCUPLOADTIME")]
        public string UploadedDate { get; set; }
        [JsonProperty("DOCSIZE")]
        public string Size { get; set; }
        [JsonProperty("VERSION")]
        public string Version { get; set; }
    }
    #endregion

    #region JSON_Upload
    public class JSON_Upload
    {
        [JsonProperty("documentname")]
        public string name { get; set; }
        [JsonProperty("uploaddocid")]
        public string id { get; set; }
    }
    #endregion

    #region JSON_ListFolder
    public class JSON_ListFolder
    {
        public List<JSON_FileMetadata> FILES { get; set; }
        public List<JSON_FolderMetadata> FOLDER { get; set; }
    }

    #endregion



}
