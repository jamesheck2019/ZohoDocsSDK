Imports System
Imports System.Collections.Generic
Imports System.Collections.Specialized
Imports System.Globalization
Imports System.IO
Imports System.IO.Compression
Imports System.Net
Imports System.Net.Security
Imports System.Net.Sockets
Imports System.Security.Authentication
Imports System.Security.Cryptography.X509Certificates
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading
Imports System.Web

Namespace DeQmaTek


    Public Class TcpClientHttpRequest
        Implements IDisposable

        Public Sub New()
            Me._headers.Add("Connection", "Keep-Alive")
            Me._headers.Add("Accept", "*/*")
            Me._headers.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2)")
            Me._headers.Add("Accept-Language", "en-us")
            Me._headers.Add("Accept-Encoding", "gzip, deflate")
        End Sub

        Public Sub Close()
            If Me._client IsNot Nothing Then
                Dim flag As Boolean = False
                Try
                    Dim close_lock As Object = Me._close_lock
                    Dim obj As Object = close_lock
                    Monitor.Enter(close_lock, flag)
                    If Me._client IsNot Nothing Then
                        If Me._stream IsNot Nothing Then
                            Try
                                Me._stream.Close()
                                Me._stream.Dispose()
                            Finally
                                Me._stream = Nothing
                            End Try
                        End If
                        Me._client.Close()
                        Me._client = Nothing
                    End If
                Finally
                    If flag Then
                        Dim obj As Object
                        Monitor.[Exit](obj)
                    End If
                End Try
            End If
        End Sub

        Public Sub Send()
            Me.Send(String.Empty)
        End Sub

        Public Sub Send(vars As NameValueCollection)
            Dim encode As Encoding = Encoding.GetEncoding(Me._charset)
            Dim sb As List(Of String) = New List(Of String)()
            For Each key As String In vars.AllKeys
                'sb.Add(key + "=" + System.Net.WebUtility.UrlEncode(vars(key), encode))
                sb.Add(key + "=" + encode.GetString(Encoding.Default.GetBytes(System.Net.WebUtility.UrlEncode(vars(key)))))
            Next
            Me.Send(String.Join("&", sb.ToArray()))
        End Sub

        Public Overridable Sub Send(data As String)
            Me.Send(data, 0)
        End Sub

        Private Sub Send(data As String, redirections As Integer)
            Me._data = data
            Dim encode As Encoding = Encoding.GetEncoding(Me._charset)
            Me._headers.Remove("Content-Length")
            If Not String.IsNullOrEmpty(data) AndAlso String.Compare(Me._method, "post", True) = 0 Then
                Me._headers("Content-Length") = String.Concat(Encoding.GetEncoding(Me._charset).GetBytes(data).Length)
                If String.IsNullOrEmpty(Me._headers("Content-Type")) Then
                    Me._headers("Content-Type") = "application/x-www-form-urlencoded; charset=" + Me._charset
                ElseIf Me._headers("Content-Type").IndexOf("multipart/form-data") = -1 Then
                    If Me._headers("Content-Type").IndexOf("application/x-www-form-urlencoded") = -1 Then
                        Dim headers As NameValueCollection = Me._headers
                        Dim nameValueCollection As NameValueCollection = headers
                        headers("Content-Type") = nameValueCollection("Content-Type") + "; application/x-www-form-urlencoded"
                    End If
                    If Me._headers("Content-Type").IndexOf("charset=") = -1 Then
                        Dim headers2 As NameValueCollection = Me._headers
                        Dim nameValueCollection As NameValueCollection = headers2
                        headers2("Content-Type") = nameValueCollection("Content-Type") + "; charset=" + Me._charset
                    End If
                End If
                data += vbCrLf & vbCrLf
            End If
            Dim uri As Uri = New Uri(Me._action)
            If Me._cookieContainer IsNot Nothing Then
                Dim cc As CookieContainer = New CookieContainer()
                If Me._headers("Cookie") <> Nothing Then
                    cc.SetCookies(uri, Me._headers("Cookie"))
                End If
                Dim uri2 As Uri = New Uri(uri.AbsoluteUri.Insert(uri.Scheme.Length + 3, "httprequest."))
                Dim cookies As CookieCollection = Me._cookieContainer.GetCookies(uri)
                For Each obj As Object In cookies
                    Dim cookie As Cookie = CType(obj, Cookie)
                    cc.SetCookies(uri, String.Concat(cookie))
                Next
                cookies = Me._cookieContainer.GetCookies(uri2)
                For Each obj2 As Object In cookies
                    Dim cookie As Cookie = CType(obj2, Cookie)
                    cc.SetCookies(uri, String.Concat(cookie))
                Next
                Me._headers("Cookie") = cc.GetCookieHeader(uri)
                If String.IsNullOrEmpty(Me._headers("Cookie")) Then
                    Me._headers.Remove("Cookie")
                End If
            End If
            Me._headers("Host") = uri.Authority
            Dim http As String = Me._method + " " + uri.PathAndQuery + " HTTP/1.1" & vbCrLf
            For Each obj3 As Object In Me._headers
                Dim head As String = CStr(obj3)
                Dim text As String = http
                http = String.Concat(New String() {text, head, ": ", Me._headers(head), vbCrLf})
            Next
            http = http + vbCrLf + data
            Me._head = http
            If Me._proxy IsNot Nothing Then
                Dim pr As TcpClientHttpRequest.RequestProxy = New TcpClientHttpRequest.RequestProxy()
                pr.Method = Me._method
                pr.Action = Me._action
                pr.Charset = Me._charset
                pr.Head = Me._head
                pr.Data = data
                pr.Connection = Me._proxyConnection
                pr.Timeout = Me._timeout
                pr.MaximumAutomaticRedirections = Me._maximumAutomaticRedirections
                Dim response As TcpClientHttpRequest.ResponseProxy = Me._proxy.Send(pr)
                Me.Action = response.RequestAction
                Me._method = response.RequestMethod
                Me._headers = TcpClientHttpRequest.Utils.ParseHttpRequestHeader(response.RequestHead)
                Me._response = New TcpClientHttpRequest.HttpResponse(Me, response.ResponseHead)
                Me._response.SetStream(response.Response)
            Else
                Dim request As Byte() = encode.GetBytes(http)
                If Me._client Is Nothing OrElse Me._remote Is Nothing OrElse String.Compare(Me._remote.Authority, uri.Authority, True) <> 0 Then
                    Me._remote = uri
                    Me.Close()
                    Me._client = New TcpClient(uri.Host, uri.Port)
                End If
                Try
                    Me._stream = Me.getStream(uri)
                    Me._stream.Write(request, 0, request.Length)
                Catch
                    Me.Close()
                    Me._client = New TcpClient(uri.Host, uri.Port)
                    Me._stream = Me.getStream(uri)
                    Me._stream.Write(request, 0, request.Length)
                End Try
                Me.receive(Me._stream, redirections, uri, encode)
            End If
        End Sub

        Protected Sub receive(stream As Stream, redirections As Integer, uri As Uri, encode As Encoding)
            stream.ReadTimeout = Me._timeout
            Me._response = Nothing
            Dim bytes As Byte() = New Byte(1023) {}
            Dim overs As Integer = bytes.Length
            Dim headStream As MemoryStream = New MemoryStream()
            Dim bodyStream As MemoryStream = New MemoryStream()
            Dim chunkStream As MemoryStream = New MemoryStream()
            Dim exception As Exception = Nothing
            While overs > 0
                Try
                    overs = stream.Read(bytes, 0, bytes.Length)
                    If headStream.Length = 0L AndAlso overs = 0 Then
                        Throw New Exception("Connection is closed")
                    End If
                Catch e As Exception
                    Dim flag As Boolean
                    If headStream.Length = 0L Then
                        Dim autoSendError As Integer = Me._autoSendError
                        Dim num As Integer = autoSendError
                        Me._autoSendError = autoSendError + 1
                        flag = (num >= 5)
                    Else
                        flag = True
                    End If
                    If Not flag Then
                        headStream.Close()
                        bodyStream.Close()
                        chunkStream.Close()
                        Me._remote = Nothing
                        Me.Send(Me._data, 0)
                        Return
                    End If
                    exception = e
                    Exit Try
                End Try
                If Me._response Is Nothing Then
                    headStream.Write(bytes, 0, overs)
                    bytes = headStream.ToArray()
                    Dim idx As Integer = TcpClientHttpRequest.Utils.findBytes(bytes, New Byte() {13, 10, 13, 10}, 0)
                    If idx <> -1 Then
                        headStream.Close()
                        headStream = New MemoryStream()
                        headStream.Write(bytes, 0, idx)
                        chunkStream.Write(bytes, idx + 4, bytes.Length - idx - 4)
                        Me._response = New TcpClientHttpRequest.HttpResponse(Me, headStream.ToArray())
                        Me._response.Received += bytes.Length - idx - 4
                        If Me._response.StatusCode = HttpStatusCode.Found OrElse Me._response.StatusCode = HttpStatusCode.MovedPermanently Then
                            If String.Compare(Me._method, "post", True) = 0 Then
                                Me._headers.Remove("Content-Length")
                                If Not String.IsNullOrEmpty(Me._headers("Content-Type")) Then
                                    Me._headers("Content-Type") = Me._headers("Content-Type").Replace("; application/x-www-form-urlencoded", String.Empty).Replace("application/x-www-form-urlencoded", String.Empty)
                                    If String.IsNullOrEmpty(Me._headers("Content-Type")) Then
                                        Me._headers.Remove("Content-Type")
                                    End If
                                End If
                            End If
                            headStream.Close()
                            bodyStream.Close()
                            chunkStream.Close()
                            Me.closeTcp()
                            Dim num2 As Integer = redirections + 1
                            redirections = num2
                            If num2 > Me._maximumAutomaticRedirections Then
                                Throw New WebException("Redirect more than " + Me._maximumAutomaticRedirections + " Times.")
                            End If
                            Dim url As String = Me._response.Headers("Location")
                            If Not String.IsNullOrEmpty(url) Then
                                If uri.IsWellFormedUriString(url, UriKind.Relative) Then
                                    If Not url.StartsWith("/") Then
                                        Dim eidx As Integer = Me.Address.AbsolutePath.LastIndexOf("/"c)
                                        url = Me.Address.AbsolutePath.Remove(eidx) + "/" + url
                                    End If
                                    url = Me.Address.Scheme + "://" + Me.Address.Authority + url
                                    url = New Uri(url).AbsoluteUri
                                End If
                            End If
                            Me.Action = url
                            Me.Method = "get"
                            Me.Headers.Remove("Cookie")
                            Me.Send(Nothing, redirections)
                            Return
                        ElseIf Me._response.StatusCode = HttpStatusCode.[Continue] Then
                            Me._response = Nothing
                            headStream = New MemoryStream()
                        End If
                    End If
                Else
                    Me._response.Received += overs
                    chunkStream.Write(bytes, 0, overs)
                End If
                If Me._response IsNot Nothing Then
                    If String.Compare(Me._response.TransferEncoding, "chunked") = 0 Then
                        Dim chunks As Byte() = chunkStream.ToArray()
                        Dim lidx As Integer = 0
                        Dim chunkSize As Integer = -1
                        Dim isBreak As Boolean = False
                        Dim isContinue As Boolean
                        Do
                            isContinue = False
                            Dim idx As Integer = TcpClientHttpRequest.Utils.findBytes(chunks, New Byte() {13, 10}, lidx)
                            If idx <> -1 Then
                                Dim chu As String() = encode.GetString(chunks, lidx, idx - lidx).Split(New Char() {";"c}, 1)
                                If Integer.TryParse(chu(0), NumberStyles.HexNumber, Nothing, chunkSize) Then
                                    Dim esize As Integer = chunks.Length - idx - 2
                                    If esize >= chunkSize + 2 Then
                                        chunkStream.Close()
                                        chunkStream = New MemoryStream()
                                        chunkStream.Write(chunks, idx + 2 + chunkSize + 2, esize - chunkSize - 2)
                                        bodyStream.Write(chunks, idx + 2, chunkSize)
                                        lidx = idx + 2 + chunkSize + 2
                                        If chunkStream.Length = 5L Then
                                            idx = TcpClientHttpRequest.Utils.findBytes(chunks, New Byte() {48, 13, 10, 13, 10}, 0)
                                            If idx <> 0 Then
                                                GoTo Block_20
                                            End If
                                        End If
                                        isContinue = True
                                    End If
                                Else
                                    chunkSize = -1
                                End If
                            End If
                        Loop While isContinue
IL_591:
                        If isBreak Then
                            Exit While
                        End If
                        Continue While
Block_20:
                        isBreak = True
                        GoTo IL_591
                    End If
                    If Me._response.ContentLength >= 0 Then
                        If CLng(Me._response.ContentLength) <= chunkStream.Length Then
                            Exit While
                        End If
                    End If
                End If
            End While
            Me._autoSendError = 0
            If Me._response IsNot Nothing Then
                If String.Compare(Me._response.TransferEncoding, "chunked") = 0 Then
                    Me._response.SetStream(bodyStream.ToArray())
                ElseIf Me._response.ContentLength >= 0 Then
                    Me._response.SetStream(chunkStream.ToArray())
                Else
                    Me._response.SetStream(chunkStream.ToArray())
                End If
                headStream.Close()
                bodyStream.Close()
                chunkStream.Close()
                Me.closeTcp()
                Return
            End If
            headStream.Close()
            bodyStream.Close()
            chunkStream.Close()
            Me.closeTcp()
            Dim sb As List(Of String) = New List(Of String)()
            sb.Add(Me._method.ToUpper() + " " + New Uri(Me._action).PathAndQuery + " HTTP/1.1")
            For Each obj As Object In Me._headers
                Dim header As String = CStr(obj)
                sb.Add(header + ": " + Me._headers(header))
            Next
            If exception Is Nothing Then
                Throw New WebException("Read failed. " + String.Join(vbCrLf, sb.ToArray()))
            End If
            Throw New WebException(exception.Message + vbCrLf + String.Join(vbCrLf, sb.ToArray()), exception)
        End Sub

        Protected Function closeTcp() As Boolean
            Dim result As Boolean
            If Me._client IsNot Nothing AndAlso Me._response IsNot Nothing AndAlso (String.Compare(Me._headers("Connection"), "keep-alive", True) <> 0 OrElse String.Compare(Me._response.Headers("Connection"), "keep-alive", True) <> 0) Then
                Me.Close()
                result = True
            Else
                result = False
            End If
            Return result
        End Function

        Protected Function getStream(uri As Uri) As Stream
            Dim result As Stream
            If String.Compare(uri.Scheme, "https", False) = 0 Then
                Dim ssl As SslStream = New SslStream(Me._client.GetStream(), False, Function(sender As Object, certificate As X509Certificate, chain As X509Chain, sslPolicyErrors As SslPolicyErrors) sslPolicyErrors = sslPolicyErrors.None, Nothing)
                ssl.AuthenticateAsClient(uri.Host, New X509CertificateCollection(), SslProtocols.Ssl3 Or SslProtocols.Tls Or SslProtocols.Tls11 Or SslProtocols.Tls12, False)
                result = ssl
            Else
                result = Me._client.GetStream()
            End If
            Return result
        End Function

        Public Property Method As String
            Get
                Return Me._method
            End Get
            Set(value As String)
                Me._method = value.ToUpper()
            End Set
        End Property

        Public Property Action As String
            Get
                Return Me._action
            End Get
            Set(value As String)
                If String.Compare(Me._action, value, True) <> 0 Then
                    If Not System.Uri.IsWellFormedUriString(value, UriKind.Absolute) Then
                        Throw New WebException("Incorrect url"" + value + """)
                    End If
                    Dim uri As Uri = New Uri(value)
                    Me._action = uri.AbsoluteUri
                End If
            End Set
        End Property


        Public ReadOnly Property Address As Uri
            Get
                Return New Uri(Me._action)
            End Get
        End Property

        ' Token: 0x17000004 RID: 4
        ' (get) Token: 0x0600000F RID: 15 RVA: 0x00003288 File Offset: 0x00001488
        ' (set) Token: 0x06000010 RID: 16 RVA: 0x000032A0 File Offset: 0x000014A0
        Public Property Charset As String
            Get
                Return Me._charset
            End Get
            Set(value As String)
                Me._charset = value
            End Set
        End Property

        ' Token: 0x17000005 RID: 5
        ' (get) Token: 0x06000011 RID: 17 RVA: 0x000032AC File Offset: 0x000014AC
        Public ReadOnly Property Head As String
            Get
                Return Me._head
            End Get
        End Property

        ' Token: 0x17000006 RID: 6
        ' (get) Token: 0x06000012 RID: 18 RVA: 0x000032C4 File Offset: 0x000014C4
        ' (set) Token: 0x06000013 RID: 19 RVA: 0x000032DC File Offset: 0x000014DC
        Public Property ProxyConnection As String
            Get
                Return Me._proxyConnection
            End Get
            Set(value As String)
                Me._proxyConnection = value
            End Set
        End Property

        ' Token: 0x17000007 RID: 7
        ' (get) Token: 0x06000014 RID: 20 RVA: 0x000032E8 File Offset: 0x000014E8
        ' (set) Token: 0x06000015 RID: 21 RVA: 0x00003300 File Offset: 0x00001500
        Public Property MaximumAutomaticRedirections As Integer
            Get
                Return Me._maximumAutomaticRedirections
            End Get
            Set(value As Integer)
                Me._maximumAutomaticRedirections = value
            End Set
        End Property

        ' Token: 0x17000008 RID: 8
        ' (get) Token: 0x06000016 RID: 22 RVA: 0x0000330C File Offset: 0x0000150C
        ' (set) Token: 0x06000017 RID: 23 RVA: 0x00003324 File Offset: 0x00001524
        Public Property Timeout As Integer
            Get
                Return Me._timeout
            End Get
            Set(value As Integer)
                Me._timeout = value
            End Set
        End Property

        ' Token: 0x17000009 RID: 9
        ' (get) Token: 0x06000018 RID: 24 RVA: 0x00003330 File Offset: 0x00001530
        ' (set) Token: 0x06000019 RID: 25 RVA: 0x00003348 File Offset: 0x00001548
        Public Property CookieContainer As CookieContainer
            Get
                Return Me._cookieContainer
            End Get
            Set(value As CookieContainer)
                Me._cookieContainer = value
            End Set
        End Property

        ' Token: 0x1700000A RID: 10
        ' (get) Token: 0x0600001A RID: 26 RVA: 0x00003354 File Offset: 0x00001554
        ' (set) Token: 0x0600001B RID: 27 RVA: 0x0000336C File Offset: 0x0000156C
        Public Property Proxy As TcpClientHttpRequest.IRequestProxy
            Get
                Return Me._proxy
            End Get
            Set(value As TcpClientHttpRequest.IRequestProxy)
                Me._proxy = value
            End Set
        End Property

        ' Token: 0x1700000B RID: 11
        ' (get) Token: 0x0600001C RID: 28 RVA: 0x00003378 File Offset: 0x00001578
        Public ReadOnly Property Response As TcpClientHttpRequest.HttpResponse
            Get
                Return Me._response
            End Get
        End Property

        ' Token: 0x1700000C RID: 12
        ' (get) Token: 0x0600001D RID: 29 RVA: 0x00003390 File Offset: 0x00001590
        Public ReadOnly Property Headers As NameValueCollection
            Get
                Return Me._headers
            End Get
        End Property

        ' Token: 0x0600001E RID: 30 RVA: 0x000033A8 File Offset: 0x000015A8
        Public Sub SetHeaders(headers As String)
            Me._headers = TcpClientHttpRequest.Utils.ParseHttpRequestHeader(headers)
        End Sub

        ' Token: 0x1700000D RID: 13
        ' (get) Token: 0x0600001F RID: 31 RVA: 0x000033B8 File Offset: 0x000015B8
        ' (set) Token: 0x06000020 RID: 32 RVA: 0x000033DA File Offset: 0x000015DA
        Public Property Accept As String
            Get
                Return Me._headers("Accept")
            End Get
            Set(value As String)
                Me._headers("Accept") = value
            End Set
        End Property

        ' Token: 0x1700000E RID: 14
        ' (get) Token: 0x06000021 RID: 33 RVA: 0x000033F0 File Offset: 0x000015F0
        ' (set) Token: 0x06000022 RID: 34 RVA: 0x00003412 File Offset: 0x00001612
        Public Property AcceptLanguage As String
            Get
                Return Me._headers("Accept-Language")
            End Get
            Set(value As String)
                Me._headers("Accept-Language") = value
            End Set
        End Property

        ' Token: 0x1700000F RID: 15
        ' (get) Token: 0x06000023 RID: 35 RVA: 0x00003428 File Offset: 0x00001628
        ' (set) Token: 0x06000024 RID: 36 RVA: 0x0000344A File Offset: 0x0000164A
        Public Property Connection As String
            Get
                Return Me._headers("Connection")
            End Get
            Set(value As String)
                Me._headers("Connection") = value
            End Set
        End Property

        ' Token: 0x17000010 RID: 16
        ' (get) Token: 0x06000025 RID: 37 RVA: 0x00003460 File Offset: 0x00001660
        ' (set) Token: 0x06000026 RID: 38 RVA: 0x0000348B File Offset: 0x0000168B
        Public Property ContentLength As Integer
            Get
                Dim tryint As Integer
                Integer.TryParse(Me._headers("Content-Length"), tryint)
                Return tryint
            End Get
            Set(value As Integer)
                Me._headers("Content-Length") = String.Concat(value)
            End Set
        End Property

        ' Token: 0x17000011 RID: 17
        ' (get) Token: 0x06000027 RID: 39 RVA: 0x000034AC File Offset: 0x000016AC
        ' (set) Token: 0x06000028 RID: 40 RVA: 0x000034CE File Offset: 0x000016CE
        Public Property ContentType As String
            Get
                Return Me._headers("Content-Type")
            End Get
            Set(value As String)
                Me._headers("Content-Type") = value
            End Set
        End Property

        ' Token: 0x17000012 RID: 18
        ' (get) Token: 0x06000029 RID: 41 RVA: 0x000034E4 File Offset: 0x000016E4
        ' (set) Token: 0x0600002A RID: 42 RVA: 0x00003506 File Offset: 0x00001706
        Public Property Expect As String
            Get
                Return Me._headers("Expect")
            End Get
            Set(value As String)
                Me._headers("Expect") = value
            End Set
        End Property

        ' Token: 0x17000013 RID: 19
        ' (get) Token: 0x0600002B RID: 43 RVA: 0x0000351C File Offset: 0x0000171C
        ' (set) Token: 0x0600002C RID: 44 RVA: 0x0000353E File Offset: 0x0000173E
        Public Property MediaType As String
            Get
                Return Me._headers("Media-Type")
            End Get
            Set(value As String)
                Me._headers("Media-Type") = value
            End Set
        End Property

        ' Token: 0x17000014 RID: 20
        ' (get) Token: 0x0600002D RID: 45 RVA: 0x00003554 File Offset: 0x00001754
        ' (set) Token: 0x0600002E RID: 46 RVA: 0x00003576 File Offset: 0x00001776
        Public Property Referer As String
            Get
                Return Me._headers("Referer")
            End Get
            Set(value As String)
                Me._headers("Referer") = value
            End Set
        End Property

        ' Token: 0x17000015 RID: 21
        ' (get) Token: 0x0600002F RID: 47 RVA: 0x0000358C File Offset: 0x0000178C
        ' (set) Token: 0x06000030 RID: 48 RVA: 0x000035AE File Offset: 0x000017AE
        Public Property TransferEncoding As String
            Get
                Return Me._headers("Transfer-Encoding")
            End Get
            Set(value As String)
                Me._headers("Transfer-Encoding") = value
            End Set
        End Property

        ' Token: 0x17000016 RID: 22
        ' (get) Token: 0x06000031 RID: 49 RVA: 0x000035C4 File Offset: 0x000017C4
        ' (set) Token: 0x06000032 RID: 50 RVA: 0x000035E6 File Offset: 0x000017E6
        Public Property UserAgent As String
            Get
                Return Me._headers("User-Agent")
            End Get
            Set(value As String)
                Me._headers("User-Agent") = value
            End Set
        End Property

        ' Token: 0x06000033 RID: 51 RVA: 0x000035FC File Offset: 0x000017FC
        Public Sub Dispose() Implements System.IDisposable.Dispose
            Try
                Me._headers.Clear()
                Me.Close()
            Catch
            End Try
        End Sub

        ' Token: 0x04000001 RID: 1
        Private Shared _cookies As CookieContainer = New CookieContainer()

        ' Token: 0x04000002 RID: 2
        Private _client As TcpClient

        ' Token: 0x04000003 RID: 3
        Private _stream As Stream

        ' Token: 0x04000004 RID: 4
        Private _remote As Uri

        ' Token: 0x04000005 RID: 5
        Private _close_lock As Object = New Object()

        ' Token: 0x04000006 RID: 6
        Private _method As String = "GET"

        ' Token: 0x04000007 RID: 7
        Private _action As String

        ' Token: 0x04000008 RID: 8
        Private _charset As String = "utf-8"

        ' Token: 0x04000009 RID: 9
        Private _head As String

        ' Token: 0x0400000A RID: 10
        Private _data As String

        ' Token: 0x0400000B RID: 11
        Private _proxyConnection As String = "default"

        ' Token: 0x0400000C RID: 12
        Private _autoSendError As Integer

        ' Token: 0x0400000D RID: 13
        Private _timeout As Integer = 20000

        ' Token: 0x0400000E RID: 14
        Private _maximumAutomaticRedirections As Integer = 500

        ' Token: 0x0400000F RID: 15
        Private _cookieContainer As CookieContainer = TcpClientHttpRequest._cookies

        ' Token: 0x04000010 RID: 16
        Friend _cookieContainer_lock As Object = New Object()

        ' Token: 0x04000011 RID: 17
        Private _proxy As TcpClientHttpRequest.IRequestProxy

        ' Token: 0x04000012 RID: 18
        Private _response As TcpClientHttpRequest.HttpResponse

        ' Token: 0x04000013 RID: 19
        Private _headers As NameValueCollection = New NameValueCollection()

        ' Token: 0x02000003 RID: 3
        Public Class Utils
            ' Token: 0x06000036 RID: 54 RVA: 0x00003644 File Offset: 0x00001844
            Public Shared Function ParseHttpRequestHeader(head As String) As NameValueCollection
                Dim headers As NameValueCollection = New NameValueCollection()
                If Not String.IsNullOrEmpty(head) Then
                    Dim hs As String() = head.Split(New String() {vbCrLf}, StringSplitOptions.None)
                    For Each h As String In hs
                        Dim nv As String() = h.Split(New Char() {":"c}, 2)
                        If nv.Length = 2 Then
                            Dim i As String = nv(0).Trim()
                            Dim v As String = nv(1).Trim()
                            headers.Add(i, v)
                        End If
                    Next
                End If
                Return headers
            End Function

            ' Token: 0x06000037 RID: 55 RVA: 0x00003720 File Offset: 0x00001920
            Public Shared Function findBytes(source As Byte(), find As Byte(), startIndex As Integer) As Integer
                Dim result As Integer
                If find Is Nothing Then
                    result = -1
                ElseIf find.Length = 0 Then
                    result = -1
                ElseIf source Is Nothing Then
                    result = -1
                ElseIf source.Length = 0 Then
                    result = -1
                Else
                    If startIndex < 0 Then
                        startIndex = 0
                    End If
                    Dim idx2 As Integer = startIndex - 1
                    Dim idx3 As Integer
                    Do
                        Dim num As Integer = Array.FindIndex(Of Byte)(source, Math.Min(idx2 + 1, source.Length), Function(b As Byte) b = find(0))
                        idx3 = num
                        idx2 = num
                        If idx2 <> -1 Then
                            For a As Integer = 1 To find.Length - 1
                                Dim num2 As Integer = idx2 + 1
                                idx2 = num2
                                If num2 >= source.Length OrElse source(idx2) <> find(a) Then
                                    idx3 = -1
                                    Exit For
                                End If
                            Next
                            If idx3 <> -1 Then
                                Exit Do
                            End If
                        End If
                    Loop While idx2 <> -1
                    result = idx3
                End If
                Return result
            End Function
        End Class

        ' Token: 0x02000004 RID: 4
        Public NotInheritable Class GZip
            ' Token: 0x06000039 RID: 57 RVA: 0x00003860 File Offset: 0x00001A60
            Public Shared Function Decompress(stream As Stream) As Byte()
                Dim result As Byte()
                Try
                    stream.Position = 0L
                    Using ms As MemoryStream = New MemoryStream()
                        Using gzip As GZipStream = New GZipStream(stream, CompressionMode.Decompress)
                            Dim data As Byte() = New Byte(1023) {}
                            While True
                                Dim num As Integer = gzip.Read(data, 0, data.Length)
                                Dim size As Integer = num
                                If num <= 0 Then
                                    Exit While
                                End If
                                ms.Write(data, 0, size)
                            End While
                        End Using
                        result = ms.ToArray()
                    End Using
                Catch
                    result = TryCast(stream, MemoryStream).ToArray()
                End Try
                Return result
            End Function

            ' Token: 0x0600003A RID: 58 RVA: 0x00003928 File Offset: 0x00001B28
            Public Shared Function Decompress(bt As Byte()) As Byte()
                Return TcpClientHttpRequest.GZip.Decompress(New MemoryStream(bt))
            End Function

            ' Token: 0x0600003B RID: 59 RVA: 0x00003948 File Offset: 0x00001B48
            Public Shared Function Compress(text As String) As Byte()
                Return TcpClientHttpRequest.GZip.Compress(Encoding.UTF8.GetBytes(text))
            End Function

            ' Token: 0x0600003C RID: 60 RVA: 0x0000396C File Offset: 0x00001B6C
            Public Shared Function Compress(bt As Byte()) As Byte()
                Return TcpClientHttpRequest.GZip.Compress(bt, 0, bt.Length)
            End Function

            ' Token: 0x0600003D RID: 61 RVA: 0x00003988 File Offset: 0x00001B88
            Public Shared Function Compress(bt As Byte(), startIndex As Integer, length As Integer) As Byte()
                Dim result As Byte()
                Using ms As MemoryStream = New MemoryStream()
                    Using gzip As GZipStream = New GZipStream(ms, CompressionMode.Compress)
                        gzip.Write(bt, startIndex, length)
                    End Using
                    result = ms.ToArray()
                End Using
                Return result
            End Function
        End Class

        ' Token: 0x02000005 RID: 5
        Public NotInheritable Class Deflate
            ' Token: 0x0600003E RID: 62 RVA: 0x000039FC File Offset: 0x00001BFC
            Public Shared Function Decompress(stream As Stream) As Byte()
                Dim result As Byte()
                Try
                    stream.Position = 0L
                    Using ms As MemoryStream = New MemoryStream()
                        Using def As DeflateStream = New DeflateStream(stream, CompressionMode.Decompress)
                            Dim data As Byte() = New Byte(1023) {}
                            While True
                                Dim num As Integer = def.Read(data, 0, data.Length)
                                Dim size As Integer = num
                                If num <= 0 Then
                                    Exit While
                                End If
                                ms.Write(data, 0, size)
                            End While
                        End Using
                        result = ms.ToArray()
                    End Using
                Catch
                    result = TryCast(stream, MemoryStream).ToArray()
                End Try
                Return result
            End Function

            ' Token: 0x0600003F RID: 63 RVA: 0x00003AC4 File Offset: 0x00001CC4
            Public Shared Function Decompress(bt As Byte()) As Byte()
                Return TcpClientHttpRequest.Deflate.Decompress(New MemoryStream(bt))
            End Function

            ' Token: 0x06000040 RID: 64 RVA: 0x00003AE4 File Offset: 0x00001CE4
            Public Shared Function Compress(text As String) As Byte()
                Return TcpClientHttpRequest.Deflate.Compress(Encoding.UTF8.GetBytes(text))
            End Function

            ' Token: 0x06000041 RID: 65 RVA: 0x00003B08 File Offset: 0x00001D08
            Public Shared Function Compress(bt As Byte()) As Byte()
                Return TcpClientHttpRequest.Deflate.Compress(bt, 0, bt.Length)
            End Function

            ' Token: 0x06000042 RID: 66 RVA: 0x00003B24 File Offset: 0x00001D24
            Public Shared Function Compress(bt As Byte(), startIndex As Integer, length As Integer) As Byte()
                Dim result As Byte()
                Using ms As MemoryStream = New MemoryStream()
                    Using def As DeflateStream = New DeflateStream(ms, CompressionMode.Compress)
                        def.Write(bt, startIndex, length)
                    End Using
                    result = ms.ToArray()
                End Using
                Return result
            End Function
        End Class

        ' Token: 0x02000006 RID: 6
        Public Interface IRequestProxy
            ' Token: 0x06000043 RID: 67
            Function Send(request As TcpClientHttpRequest.RequestProxy) As TcpClientHttpRequest.ResponseProxy
        End Interface

        ' Token: 0x02000007 RID: 7
        <Serializable()>
        Public Class RequestProxy
            ' Token: 0x17000017 RID: 23
            ' (get) Token: 0x06000044 RID: 68 RVA: 0x00003B98 File Offset: 0x00001D98
            ' (set) Token: 0x06000045 RID: 69 RVA: 0x00003BB0 File Offset: 0x00001DB0
            Public Property Method As String
                Get
                    Return Me._method
                End Get
                Set(value As String)
                    Me._method = value
                End Set
            End Property

            ' Token: 0x17000018 RID: 24
            ' (get) Token: 0x06000046 RID: 70 RVA: 0x00003BBC File Offset: 0x00001DBC
            ' (set) Token: 0x06000047 RID: 71 RVA: 0x00003BD4 File Offset: 0x00001DD4
            Public Property Action As String
                Get
                    Return Me._action
                End Get
                Set(value As String)
                    Me._action = value
                End Set
            End Property

            ' Token: 0x17000019 RID: 25
            ' (get) Token: 0x06000048 RID: 72 RVA: 0x00003BE0 File Offset: 0x00001DE0
            ' (set) Token: 0x06000049 RID: 73 RVA: 0x00003BF8 File Offset: 0x00001DF8
            Public Property Charset As String
                Get
                    Return Me._charset
                End Get
                Set(value As String)
                    Me._charset = value
                End Set
            End Property

            ' Token: 0x1700001A RID: 26
            ' (get) Token: 0x0600004A RID: 74 RVA: 0x00003C04 File Offset: 0x00001E04
            ' (set) Token: 0x0600004B RID: 75 RVA: 0x00003C1C File Offset: 0x00001E1C
            Public Property Head As String
                Get
                    Return Me._head
                End Get
                Set(value As String)
                    Me._head = value
                End Set
            End Property

            ' Token: 0x1700001B RID: 27
            ' (get) Token: 0x0600004C RID: 76 RVA: 0x00003C28 File Offset: 0x00001E28
            ' (set) Token: 0x0600004D RID: 77 RVA: 0x00003C40 File Offset: 0x00001E40
            Public Property Data As String
                Get
                    Return Me._data
                End Get
                Set(value As String)
                    Me._data = value
                End Set
            End Property

            ' Token: 0x1700001C RID: 28
            ' (get) Token: 0x0600004E RID: 78 RVA: 0x00003C4C File Offset: 0x00001E4C
            ' (set) Token: 0x0600004F RID: 79 RVA: 0x00003C64 File Offset: 0x00001E64
            Public Property Connection As String
                Get
                    Return Me._connection
                End Get
                Set(value As String)
                    Me._connection = value
                End Set
            End Property

            ' Token: 0x1700001D RID: 29
            ' (get) Token: 0x06000050 RID: 80 RVA: 0x00003C70 File Offset: 0x00001E70
            ' (set) Token: 0x06000051 RID: 81 RVA: 0x00003C88 File Offset: 0x00001E88
            Public Property Timeout As Integer
                Get
                    Return Me._timeout
                End Get
                Set(value As Integer)
                    Me._timeout = value
                End Set
            End Property

            ' Token: 0x1700001E RID: 30
            ' (get) Token: 0x06000052 RID: 82 RVA: 0x00003C94 File Offset: 0x00001E94
            ' (set) Token: 0x06000053 RID: 83 RVA: 0x00003CAC File Offset: 0x00001EAC
            Public Property MaximumAutomaticRedirections As Integer
                Get
                    Return Me._maximumAutomaticRedirections
                End Get
                Set(value As Integer)
                    Me._maximumAutomaticRedirections = value
                End Set
            End Property

            ' Token: 0x04000015 RID: 21
            Private _method As String

            ' Token: 0x04000016 RID: 22
            Private _action As String

            ' Token: 0x04000017 RID: 23
            Private _charset As String

            ' Token: 0x04000018 RID: 24
            Private _head As String

            ' Token: 0x04000019 RID: 25
            Private _data As String

            ' Token: 0x0400001A RID: 26
            Private _connection As String

            ' Token: 0x0400001B RID: 27
            Private _timeout As Integer

            ' Token: 0x0400001C RID: 28
            Private _maximumAutomaticRedirections As Integer
        End Class

        ' Token: 0x02000008 RID: 8
        <Serializable()>
        Public Class ResponseProxy
            ' Token: 0x1700001F RID: 31
            ' (get) Token: 0x06000055 RID: 85 RVA: 0x00003CC0 File Offset: 0x00001EC0
            ' (set) Token: 0x06000056 RID: 86 RVA: 0x00003CD8 File Offset: 0x00001ED8
            Public Property RequestMethod As String
                Get
                    Return Me._requestMethod
                End Get
                Set(value As String)
                    Me._requestMethod = value
                End Set
            End Property

            ' Token: 0x17000020 RID: 32
            ' (get) Token: 0x06000057 RID: 87 RVA: 0x00003CE4 File Offset: 0x00001EE4
            ' (set) Token: 0x06000058 RID: 88 RVA: 0x00003CFC File Offset: 0x00001EFC
            Public Property RequestAction As String
                Get
                    Return Me._requestAction
                End Get
                Set(value As String)
                    Me._requestAction = value
                End Set
            End Property

            ' Token: 0x17000021 RID: 33
            ' (get) Token: 0x06000059 RID: 89 RVA: 0x00003D08 File Offset: 0x00001F08
            ' (set) Token: 0x0600005A RID: 90 RVA: 0x00003D20 File Offset: 0x00001F20
            Public Property RequestHead As String
                Get
                    Return Me._requestHead
                End Get
                Set(value As String)
                    Me._requestHead = value
                End Set
            End Property

            ' Token: 0x17000022 RID: 34
            ' (get) Token: 0x0600005B RID: 91 RVA: 0x00003D2C File Offset: 0x00001F2C
            ' (set) Token: 0x0600005C RID: 92 RVA: 0x00003D44 File Offset: 0x00001F44
            Public Property ResponseHead As Byte()
                Get
                    Return Me._responseHead
                End Get
                Set(value As Byte())
                    Me._responseHead = value
                End Set
            End Property

            ' Token: 0x17000023 RID: 35
            ' (get) Token: 0x0600005D RID: 93 RVA: 0x00003D50 File Offset: 0x00001F50
            ' (set) Token: 0x0600005E RID: 94 RVA: 0x00003D68 File Offset: 0x00001F68
            Public Property Response As Byte()
                Get
                    Return Me._response
                End Get
                Set(value As Byte())
                    Me._response = value
                End Set
            End Property

            ' Token: 0x0400001D RID: 29
            Private _requestMethod As String

            ' Token: 0x0400001E RID: 30
            Private _requestAction As String

            ' Token: 0x0400001F RID: 31
            Private _requestHead As String

            ' Token: 0x04000020 RID: 32
            Private _responseHead As Byte()

            ' Token: 0x04000021 RID: 33
            Private _response As Byte()
        End Class

        ' Token: 0x02000009 RID: 9
        Public Class HttpResponse
            ' Token: 0x06000060 RID: 96 RVA: 0x00003D7C File Offset: 0x00001F7C
            Public Sub New(ie As TcpClientHttpRequest, headBytes As Byte())
                Me._action = ie.Action
                Me._method = ie.Method
                Me._charset = ie.Charset
                Dim encode As Encoding = Encoding.GetEncoding(Me._charset)
                Dim head As String = encode.GetString(headBytes)
                Dim text As String = head.Trim()
                head = text
                Me._head = text
                Dim idx As Integer = head.IndexOf(" "c)
                If idx <> -1 Then
                    head = head.Substring(idx + 1)
                End If
                idx = head.IndexOf(" "c)
                If idx <> -1 Then
                    Me._statusCode = CType(Integer.Parse(head.Remove(idx)), HttpStatusCode)
                    head = head.Substring(idx + 1)
                End If
                idx = head.IndexOf(vbCrLf)
                If idx <> -1 Then
                    head = head.Substring(idx + 2)
                End If
                Dim heads As String() = head.Split(New String() {vbCrLf}, StringSplitOptions.None)
                For Each h As String In heads
                    Dim nv As String() = h.Split(New Char() {":"c}, 2)
                    If nv.Length = 2 Then
                        Dim i As String = nv(0).Trim()
                        Dim v As String = nv(1).Trim()
                        If v.EndsWith("; Secure") Then
                            v = v.Replace("; Secure", "")
                        End If
                        If v.EndsWith("; version=1") Then
                            v = v.Replace("; version=1", "")
                        End If
                        Dim text2 As String = i.ToLower()
                        If text2 Is Nothing Then
                            GoTo IL_3EB
                        End If
                        If Not (text2 = "set-cookie") Then
                            If Not (text2 = "content-length") Then
                                If Not (text2 = "content-type") Then
                                    If Not (text2 = "server") Then
                                        If Not (text2 = "content-encoding") Then
                                            GoTo IL_3EB
                                        End If
                                        Me._contentEncoding = v
                                    Else
                                        Me._server = v
                                    End If
                                Else
                                    idx = v.IndexOf("charset=", StringComparison.CurrentCultureIgnoreCase)
                                    If idx <> -1 Then
                                        Dim charset As String = v.Substring(idx + 8)
                                        idx = charset.IndexOf(";")
                                        If idx <> -1 Then
                                            charset = charset.Remove(idx)
                                        End If
                                        If String.Compare(Me._charset, charset, True) <> 0 Then
                                            Try
                                                Dim testEncode As Encoding = Encoding.GetEncoding(charset)
                                                Me._charset = charset
                                            Catch
                                            End Try
                                        End If
                                    End If
                                    Me._contentType = v
                                End If
                            ElseIf Not Integer.TryParse(v, Me._contentLength) Then
                                Me._contentLength = -1
                            End If
                        ElseIf ie.CookieContainer IsNot Nothing Then
                            Dim addr As Uri = Me.Address
                            Dim v2d As String() = Regex.Split(v, "\bdomain=", RegexOptions.IgnoreCase Or RegexOptions.ExplicitCapture)
                            If v2d.Length > 1 Then
                                Dim domain As String = v2d(1)
                                idx = domain.IndexOf(";")
                                If idx <> -1 Then
                                    domain = domain.Remove(idx)
                                End If
                                While domain.StartsWith(".")
                                    domain = domain.Substring(1)
                                End While
                                domain = "http://" + domain + "/"
                                If Uri.IsWellFormedUriString(domain, UriKind.Absolute) Then
                                    Dim du As Uri = New Uri(domain)
                                    If String.Compare(addr.Authority, du.Authority, True) <> 0 Then
                                        addr = du
                                    End If
                                End If
                            End If
                            Dim flag As Boolean = False
                            Try
                                Dim cookieContainer_lock As Object = ie._cookieContainer_lock
                                Dim obj As Object = cookieContainer_lock
                                Monitor.Enter(cookieContainer_lock, flag)
                                ie.CookieContainer.SetCookies(addr, v)
                            Finally
                                ''isisv2.0
                                'If flag Then
                                '    Dim obj As Object
                                '    Monitor.[Exit](obj)
                                'End If
                            End Try
                        End If
                        GoTo IL_3FE
IL_3EB:
                        Me._headers.Add(i, v)
                    End If
IL_3FE:
                Next
            End Sub

            ' Token: 0x06000061 RID: 97 RVA: 0x000041BC File Offset: 0x000023BC
            Public Sub SetStream(bodyBytes As Byte())
                Me._stream = bodyBytes
                Me._contentLength = bodyBytes.Length
            End Sub

            ' Token: 0x06000062 RID: 98 RVA: 0x000041D0 File Offset: 0x000023D0
            Public Function GetStream() As Byte()
                Dim text As String = Me._contentEncoding.ToLower()
                If text IsNot Nothing Then
                    If text = "gzip" Then
                        Return TcpClientHttpRequest.GZip.Decompress(Me._stream)
                    End If
                    If text = "deflate" Then
                        Return TcpClientHttpRequest.Deflate.Decompress(Me._stream)
                    End If
                End If
                Return Me._stream
            End Function

            ' Token: 0x06000063 RID: 99 RVA: 0x00004230 File Offset: 0x00002430
            Public Sub Save(filename As String)
                Using fs As FileStream = New FileStream(filename, FileMode.Create)
                    Dim text As String = Me._contentEncoding.ToLower()
                    If text IsNot Nothing Then
                        If text = "gzip" Then
                            Dim gzip As Byte() = TcpClientHttpRequest.GZip.Decompress(Me._stream)
                            fs.Write(gzip, 0, gzip.Length)
                            GoTo IL_81
                        End If
                        If text = "deflate" Then
                            Dim deflate As Byte() = TcpClientHttpRequest.Deflate.Decompress(Me._stream)
                            fs.Write(deflate, 0, deflate.Length)
                            GoTo IL_81
                        End If
                    End If
                    fs.Write(Me._stream, 0, Me._stream.Length)
IL_81:
                End Using
            End Sub

            ' Token: 0x06000064 RID: 100 RVA: 0x000042E4 File Offset: 0x000024E4
            Public Function TranslateUrlToAbsolute(url As String) As String
                If Not String.IsNullOrEmpty(url) Then
                    If Uri.IsWellFormedUriString(url, UriKind.Relative) Then
                        If Not url.StartsWith("/") Then
                            Dim eidx As Integer = Me.Address.AbsolutePath.LastIndexOf("/"c)
                            url = Me.Address.AbsolutePath.Remove(eidx) + "/" + url
                        End If
                        url = Me.Address.Scheme + "://" + Me.Address.Authority + url
                        url = New Uri(url).AbsoluteUri
                    End If
                Else
                    Dim eidx As Integer = Me.Address.AbsolutePath.LastIndexOf("/"c)
                    url = Me.Address.Scheme + "://" + Me.Address.Authority + Me.Address.AbsolutePath.Remove(eidx)
                End If
                Return url
            End Function

            ' Token: 0x17000024 RID: 36
            ' (get) Token: 0x06000065 RID: 101 RVA: 0x000043D4 File Offset: 0x000025D4
            Public ReadOnly Property Xml As String
                Get
                    If Me._xml = Nothing Then
                        Dim text As String = Me._contentEncoding.ToLower()
                        If text IsNot Nothing Then
                            If text = "gzip" Then
                                Me._xml = Encoding.GetEncoding(Me._charset).GetString(TcpClientHttpRequest.GZip.Decompress(Me._stream))
                                GoTo IL_A4
                            End If
                            If text = "deflate" Then
                                Me._xml = Encoding.GetEncoding(Me._charset).GetString(TcpClientHttpRequest.Deflate.Decompress(Me._stream))
                                GoTo IL_A4
                            End If
                        End If
                        Me._xml = Encoding.GetEncoding(Me._charset).GetString(Me._stream)
IL_A4:
                    End If
                    Return Me._xml
                End Get
            End Property

            ' Token: 0x17000025 RID: 37
            ' (get) Token: 0x06000066 RID: 102 RVA: 0x00004490 File Offset: 0x00002690
            Public ReadOnly Property Method As String
                Get
                    Return Me._method
                End Get
            End Property

            ' Token: 0x17000026 RID: 38
            ' (get) Token: 0x06000067 RID: 103 RVA: 0x000044A8 File Offset: 0x000026A8
            Public ReadOnly Property Action As String
                Get
                    Return Me._action
                End Get
            End Property

            ' Token: 0x17000027 RID: 39
            ' (get) Token: 0x06000068 RID: 104 RVA: 0x000044C0 File Offset: 0x000026C0
            Public ReadOnly Property Address As Uri
                Get
                    Return New Uri(Me._action)
                End Get
            End Property

            ' Token: 0x17000028 RID: 40
            ' (get) Token: 0x06000069 RID: 105 RVA: 0x000044E0 File Offset: 0x000026E0
            Public ReadOnly Property Charset As String
                Get
                    Return Me._charset
                End Get
            End Property

            ' Token: 0x17000029 RID: 41
            ' (get) Token: 0x0600006A RID: 106 RVA: 0x000044F8 File Offset: 0x000026F8
            Public ReadOnly Property Head As String
                Get
                    Return Me._head
                End Get
            End Property

            ' Token: 0x1700002A RID: 42
            ' (get) Token: 0x0600006B RID: 107 RVA: 0x00004510 File Offset: 0x00002710
            Public ReadOnly Property ContentEncoding As String
                Get
                    Return Me._contentEncoding
                End Get
            End Property

            ' Token: 0x1700002B RID: 43
            ' (get) Token: 0x0600006C RID: 108 RVA: 0x00004528 File Offset: 0x00002728
            Public ReadOnly Property ContentLength As Integer
                Get
                    Return Me._contentLength
                End Get
            End Property

            ' Token: 0x1700002C RID: 44
            ' (get) Token: 0x0600006D RID: 109 RVA: 0x00004540 File Offset: 0x00002740
            ' (set) Token: 0x0600006E RID: 110 RVA: 0x00004558 File Offset: 0x00002758
            Public Property Received As Integer
                Get
                    Return Me._received
                End Get
                Friend Set(value As Integer)
                    Me._received = value
                End Set
            End Property

            ' Token: 0x1700002D RID: 45
            ' (get) Token: 0x0600006F RID: 111 RVA: 0x00004564 File Offset: 0x00002764
            Public ReadOnly Property ContentType As String
                Get
                    Return Me._contentType
                End Get
            End Property

            ' Token: 0x1700002E RID: 46
            ' (get) Token: 0x06000070 RID: 112 RVA: 0x0000457C File Offset: 0x0000277C
            Public ReadOnly Property Server As String
                Get
                    Return Me._server
                End Get
            End Property

            ' Token: 0x1700002F RID: 47
            ' (get) Token: 0x06000071 RID: 113 RVA: 0x00004594 File Offset: 0x00002794
            Public ReadOnly Property Headers As NameValueCollection
                Get
                    Return Me._headers
                End Get
            End Property

            ' Token: 0x17000030 RID: 48
            ' (get) Token: 0x06000072 RID: 114 RVA: 0x000045AC File Offset: 0x000027AC
            Public ReadOnly Property StatusCode As HttpStatusCode
                Get
                    Return Me._statusCode
                End Get
            End Property

            ' Token: 0x17000031 RID: 49
            ' (get) Token: 0x06000073 RID: 115 RVA: 0x000045C4 File Offset: 0x000027C4
            ' (set) Token: 0x06000074 RID: 116 RVA: 0x000045E6 File Offset: 0x000027E6
            Public Property TransferEncoding As String
                Get
                    Return Me._headers("Transfer-Encoding")
                End Get
                Set(value As String)
                    Me._headers("Transfer-Encoding") = value
                End Set
            End Property

            ' Token: 0x04000022 RID: 34
            Private _action As String

            ' Token: 0x04000023 RID: 35
            Private _method As String

            ' Token: 0x04000024 RID: 36
            Private _charset As String

            ' Token: 0x04000025 RID: 37
            Private _head As String

            ' Token: 0x04000026 RID: 38
            Private _contentEncoding As String = String.Empty

            ' Token: 0x04000027 RID: 39
            Private _contentLength As Integer = -1

            ' Token: 0x04000028 RID: 40
            Private _received As Integer = 0

            ' Token: 0x04000029 RID: 41
            Private _contentType As String

            ' Token: 0x0400002A RID: 42
            Private _server As String

            ' Token: 0x0400002B RID: 43
            Private _headers As NameValueCollection = New NameValueCollection()

            ' Token: 0x0400002C RID: 44
            Private _statusCode As HttpStatusCode

            ' Token: 0x0400002D RID: 45
            Private _stream As Byte() = New Byte(-1) {}

            ' Token: 0x0400002E RID: 46
            Private _xml As String
        End Class
    End Class
End Namespace