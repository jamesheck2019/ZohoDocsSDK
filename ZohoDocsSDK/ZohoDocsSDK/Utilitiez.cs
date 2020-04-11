using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZohoDocsSDK
{
   public  class Utilitiez
    {

        public static string AsQueryString(Dictionary<string, string> parameters)
        {
            if (!parameters.Any()) { return string.Empty; }

            var builder = new StringBuilder("?");
            var separator = string.Empty;
            foreach (var kvp in parameters.Where(P => !string.IsNullOrEmpty(P.Value)))
            {
                builder.AppendFormat("{0}{1}={2}", separator, System.Net.WebUtility.UrlEncode(kvp.Key), System.Net.WebUtility.UrlEncode(kvp.Value.ToString()));
                separator = "&";
            }
            return builder.ToString();
        }

        public static string Between(string source, string leftString, string rightString)
        {
            return System.Text.RegularExpressions.Regex.Match(source, string.Format("{0}(.*){1}", leftString, rightString)).Groups[1].Value;
        }

        public enum PermissionEnum
        {
            @readonly,
            readwrite,
            coowner
        }

        public enum Visibility
        {
            orglinkshare,
            orgshare,
            linkshare,
            @private
        }

        public enum PPermission
        {
            @readonly,
            readwrite
        }

        public enum CategoryEnum
        {
            _ALL_,
            documents,
            spreadsheets,
            presentations,
            pictures,
            music,
            videos,
            sharedbyme,
            sharedtome,
            thrashed
        }

        public enum RevisionTypeEnum
        {
            owned,
            shared
        }

        public enum UploadTypes
        {
            FilePath,
            Stream,
            BytesArry,
            String
        }











    }
}
