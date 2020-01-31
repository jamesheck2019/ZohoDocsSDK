Imports Newtonsoft.Json
Imports ZohoDocsSDK.JSON

Module Base

    ''api doc
    'https://www.zoho.com/docs/zoho-docs-api.html

    Public Property APIbase As String = "https://apidocs.zoho.com/files/v1/"
    Public Property JSONhandler As New JsonSerializerSettings() With {.MissingMemberHandling = MissingMemberHandling.Ignore, .NullValueHandling = NullValueHandling.Ignore}
    Friend Property authToken() As String
    Public Property m_TimeOut As System.TimeSpan = Threading.Timeout.InfiniteTimeSpan ' TimeSpan.FromMinutes(60)
    Public Property m_CloseConnection As Boolean = True
    Friend Property ConnectionSetting As ConnectionSettings


    Private _proxy As ProxyConfig
    Public Property m_proxy As ProxyConfig
        Get
            Return If(_proxy, New ProxyConfig)
        End Get
        Set(value As ProxyConfig)
            _proxy = value
        End Set
    End Property

    Public Class HCHandler
        Inherits Net.Http.HttpClientHandler
        Sub New()
            MyBase.New()
            If m_proxy.SetProxy Then
                MaxRequestContentBufferSize = 1 * 1024 * 1024
                Proxy = New Net.WebProxy(String.Format("http://{0}:{1}", m_proxy.ProxyIP, m_proxy.ProxyPort), True, Nothing, New Net.NetworkCredential(m_proxy.ProxyUsername, m_proxy.ProxyPassword))
                UseProxy = m_proxy.SetProxy
            End If
        End Sub
    End Class

    Public Class HttpClient
        Inherits Net.Http.HttpClient
        Sub New(HCHandler As HCHandler)
            MyBase.New(HCHandler)
            DefaultRequestHeaders.UserAgent.ParseAdd("ZohoDocsSDK")
            DefaultRequestHeaders.ConnectionClose = m_CloseConnection
            Timeout = m_TimeOut
        End Sub
        Sub New(progressHandler As Net.Http.Handlers.ProgressMessageHandler)
            MyBase.New(progressHandler)
            DefaultRequestHeaders.UserAgent.ParseAdd("ZohoDocsSDK")
            DefaultRequestHeaders.ConnectionClose = m_CloseConnection
            Timeout = m_TimeOut
        End Sub
    End Class

    Public Class AuthDictionary
        Inherits Dictionary(Of String, String)
        Sub New()
            MyBase.Add("SCOPE", "ZohoPC/docsapi")
            'MyBase.Add("scope", "docsapi")
            MyBase.Add("authtoken", authToken)
        End Sub
    End Class
    Public Class pUri
        Inherits Uri
        Sub New(Action As String, Optional Parameters As Dictionary(Of String, String) = Nothing)
            MyBase.New(APIbase + Action + If(Parameters Is Nothing, Nothing, utilitiez.AsQueryString(Parameters)))
        End Sub
    End Class

    <Runtime.CompilerServices.Extension()>
    Public Function Jobj(response As String) As Linq.JObject
        Return Linq.JObject.Parse(response)
    End Function
End Module
