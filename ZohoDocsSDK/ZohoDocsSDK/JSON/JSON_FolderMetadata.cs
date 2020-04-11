using Newtonsoft.Json;
using System.ComponentModel;

namespace ZohoDocsSDK.JSON
{
    public class JSON_FolderMetadata
    {
        // <JsonProperty("AUTHOR_ID")>'Public Property AuthorID As String
        // <JsonProperty("AUTHOR_NAME")>'Public Property AuthorName As String

        // <JsonProperty("LAST_MODIFIED_AUTHOR_NAME")>'Public Property _LastModifiedByAuthorName As String
        [JsonProperty("LAST_MODIFIED_TIME")]
        public string _LastModified_InMilliseconds { get; set; }
        // <JsonProperty("CREATED_TIME")>'Public Property _CreatedDate As String
        // <JsonProperty("LAST_MODIFIEDBY")>'Public Property _LastModifiedByAuthorID As String
        // <JsonProperty("LAST_OPENED_TIME")>'Public Property _LastOpenedDate As String
        // Public ReadOnly Property _ModifiedDate As String
        // Get

        // Dim dtDateTime As System.DateTime = New DateTime(1970, 1, 1, 0, 0, 0, 0)
        // dtDateTime = dtDateTime.AddMilliseconds(_LastModified_InMilliseconds).ToLocalTime()
        // Return dtDateTime
        // End Get
        // End Property

        [Browsable(false)]
        [Bindable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [JsonProperty("FOLDER_NAME")]
        private string TheName { get; set; }
        [Browsable(false)]
        [Bindable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [JsonProperty("FOLDERNAME")]
        private string TheName2 { get; set; }
        [Browsable(false)]
        [Bindable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [JsonProperty("FOLDER_ID")]
        private string TheID { get; set; }
        [Browsable(false)]
        [Bindable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [JsonProperty("FOLDERID")]
        private string TheID2 { get; set; }
        [JsonProperty("PERMISSION")]
        public int Permission { get; set; }
        // <JsonProperty("SCOPE")> Public Property Scope As Integer
        [JsonProperty("PARENT_FOLDER_ID")]
        public string ParentFolderID { get; set; }

        [JsonProperty("IS_SHARED")]
        public bool IsShared { get; set; }
        // <JsonProperty("IS_FAVOURITE")>Public Property IsFavourite As Boolean

        public string Name
        {
            get
            {
                var nme = !string.IsNullOrEmpty(TheName) ? TheName : TheName2;
                return nme;
            }
        }
        public string ID
        {
            get
            {
                var dd = !string.IsNullOrEmpty(TheID) ? TheID : TheID2;
                return dd;
            }
        }
    }
}
