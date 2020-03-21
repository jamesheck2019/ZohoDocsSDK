using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZohoDocsSDK.JSON
{
    public class JSON_SharesMetadata
    {
        public JSON_SharedDetails_Response response { get; set; }

        public List<string> MailsList
        {
            get
            {
                var co_Owner = response.result.coOwner.sharedUsers.Split(',').ToList();
                var Read_Only = response.result.ReadOnly.sharedUsers.Split(',').ToList();
                var Read_Write = response.result.ReadWrite.sharedUsers.Split(',').ToList();
                var all = co_Owner.Concat(Read_Only).Concat(Read_Write).Distinct().ToList(); // merege all lists
                return all.Where(s => !string.IsNullOrEmpty(s)).ToList(); // remove empty
            }
        }

        public string SharedFileUrl
        {
            get
            {
                return response.result.permaLink;
            }
        }

      public enum SharedORPublic
        {
            Shared,
            Public,
            none
        }
        public SharedORPublic Shared_OR_Public
        {
            get
            {
                switch (response.result.visibility)
                {
                    case "private":
                        return SharedORPublic.Shared;
                    case "linkshare":
                        return SharedORPublic.Public;
                    default : return  SharedORPublic.none;
                }
            }
        }
    }
    public class JSON_SharedDetails_Response
    {
        public JSON_SharedDetails_Result result { get; set; }
        public string uri { get; set; }
        public string message { get; set; }
    }
    public class JSON_SharedDetails_Result
    {
        public JSON_SharedDetails_Readonly ReadOnly { get; set; }
        public string permaLink { get; set; }
        public JSON_SharedDetails_Coowner coOwner { get; set; }
        public string visibility { get; set; }
        public JSON_SharedDetails_Readcomment ReadComment { get; set; }
        public string permission { get; set; }
        public JSON_SharedDetails_Readwrite ReadWrite { get; set; }
    }
    public class JSON_SharedDetails_Readonly
    {
        public string sharedUsers { get; set; }
        public string isDirectlyShared { get; set; }
        public string sharedUserZuids { get; set; }
        public string sharedGroups { get; set; }
        public string sharedEmails { get; set; }
        public string isDirectlyEmailShared { get; set; }
        public string sharedGroupNames { get; set; }
        public string isDirectlyGroupShared { get; set; }
    }
    public class JSON_SharedDetails_Coowner
    {
        public string sharedUsers { get; set; }
        public string isDirectlyShared { get; set; }
        public string sharedUserZuids { get; set; }
        public string sharedEmails { get; set; }
        public string isDirectlyEmailShared { get; set; }
    }
    public class JSON_SharedDetails_Readcomment
    {
        public string sharedUsers { get; set; }
        public string sharedUserZuids { get; set; }
        public string sharedGroups { get; set; }
        public string sharedEmails { get; set; }
        public string sharedGroupNames { get; set; }
        public string isDirectlyGroupShared { get; set; }
    }
    public class JSON_SharedDetails_Readwrite
    {
        public string sharedUsers { get; set; }
        public string isDirectlyShared { get; set; }
        public string sharedUserZuids { get; set; }
        public string sharedGroups { get; set; }
        public string sharedEmails { get; set; }
        public string isDirectlyEmailShared { get; set; }
        public string sharedGroupNames { get; set; }
        public string isDirectlyGroupShared { get; set; }
    }
}
