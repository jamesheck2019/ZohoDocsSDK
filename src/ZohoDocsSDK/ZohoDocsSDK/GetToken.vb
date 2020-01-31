Imports System.Net.Http.HttpMethod
Imports System.Net.Http

Public Class GetToken

#Region "Generate AuthToken - [API Mode]"
    ''' <summary>
    ''' API Mode
    ''' To generate AuthToken, you need to send an request to Zoho Accounts
    ''' https://accounts.zoho.com/apiauthtoken/nb/create
    ''' </summary>
    ''' <param name="Email">Mandatory. Specify your Zoho Docs User name or Email Id</param>
    ''' <param name="Password">Mandatory. Specify your Zoho Docs password</param>
    ''' <returns>Returns the generated AuthToken for the specified user</returns>
    Shared Async Function GenerateAuthToken(Email As String, Password As String) As Task(Of String)
        Dim parameters = New Dictionary(Of String, String)
        parameters.Add("SCOPE", "ZohoPC/docsapi")
        parameters.Add("EMAIL_ID", Email)
        parameters.Add("PASSWORD", Password)
        parameters.Add("DISPLAY_NAME", "ZohoDocsSDK")

        Using localHttpClient As New HttpClient(New HCHandler)
            Dim HtpReqMessage As New Net.Http.HttpRequestMessage(Post, New Uri("https://accounts.zoho.com/apiauthtoken/nb/create"))
            HtpReqMessage.Content = New FormUrlEncodedContent(parameters)
            Using response As Net.Http.HttpResponseMessage = Await localHttpClient.SendAsync(HtpReqMessage, Net.Http.HttpCompletionOption.ResponseContentRead).ConfigureAwait(False)
                Dim result = Await response.Content.ReadAsStringAsync()

                Dim tokn As String() = result.ToString.Split(New String() {"=", "="}, StringSplitOptions.None)
                If tokn(2).ToString.Trim = "TRUE" Then
                    Return tokn(1).Replace("RESULT", "")
                ElseIf tokn(2).ToString.Trim = "FALSE" Then
                    Throw ExceptionCls.CreateException(tokn(1).Replace("RESULT", ""), response.StatusCode)
                End If
            End Using
        End Using
    End Function
#End Region

#Region "Generate AuthToken From Browser - [Browser Mode]"
    ''' <summary>
    ''' Browser Mode
    ''' Generates the AuthToken
    ''' https://accounts.zoho.com/apiauthtoken/create
    ''' </summary>
    ''' <returns>Returns the generated AuthToken for the specified user.</returns>
    Shared Function GetAuthTokenFromBrowser() As String
        Return ("https://accounts.zoho.com/apiauthtoken/create" + utilitiez.AsQueryString(New Dictionary(Of String, String) From {{"SCOPE", "ZohoPC/docsapi"}, {"DISPLAY_NAME", "ZohoDocsSDK"}}))
    End Function
#End Region


End Class
