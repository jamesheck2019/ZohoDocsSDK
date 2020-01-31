
Public Class utilitiez


    Public Shared Function AsQueryString(parameters As Dictionary(Of String, String)) As String
        If Not parameters.Any() Then Return String.Empty
        Dim builder = New Text.StringBuilder("?")
        Dim separator = String.Empty
        For Each kvp In parameters.Where(Function(P) Not String.IsNullOrEmpty(P.Value))
            builder.AppendFormat("{0}{1}={2}", separator, Net.WebUtility.UrlEncode(kvp.Key), Net.WebUtility.UrlEncode(kvp.Value.ToString()))
            separator = "&"
        Next
        Return builder.ToString()
    End Function

    'Public Shared Function GetJArrayValue(JArray As Newtonsoft.Json.Linq.JObject, key As String) As String
    '    For Each keyValuePair As KeyValuePair(Of String, Newtonsoft.Json.Linq.JToken) In JArray
    '        If key = keyValuePair.Key Then
    '            Return keyValuePair.Value.ToString()
    '        End If
    '    Next
    'End Function

    Public Shared Function Between(source As String, leftString As String, rightString As String) As String
        Return System.Text.RegularExpressions.Regex.Match(source, String.Format("{0}(.*){1}", leftString, rightString)).Groups(1).Value
    End Function

    Enum PermissionEnum
        [readonly]
        readwrite
        coowner
    End Enum
    Enum Visibility
        orglinkshare
        orgshare
        linkshare
        [private]
    End Enum
    Enum PPermission
        [readonly]
        readwrite
    End Enum
    Enum CategoryEnum
        _ALL_
        documents
        spreadsheets
        presentations
        pictures
        music
        videos
        sharedbyme
        sharedtome
        thrashed
    End Enum
    Enum RevisionTypeEnum
        owned
        [shared]
    End Enum
    Enum UploadTypes
        FilePath
        Stream
        BytesArry
        [String]
    End Enum
End Class

Public Class ConnectionSettings
    Public Property TimeOut As System.TimeSpan = Nothing
    Public Property CloseConnection As Boolean? = True
    Public Property Proxy As ProxyConfig = Nothing
End Class