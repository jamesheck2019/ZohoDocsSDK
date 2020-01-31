Imports Newtonsoft.Json.Linq
Imports System.Net.Http.HttpMethod
Imports System.Net.Http

Public Class ItemsClient
    Implements IItems


    Private Property IDs As List(Of String)

    Sub New(IDs As List(Of String))
        Me.IDs = IDs
    End Sub


#Region "MoveMultipleFileFolder"
    Public Async Function POST_MoveMultipleFileFolder(DestinationFolderID As String) As Task(Of Boolean) Implements IItems.FD_Move
        Dim client As New ZClient(authToken, ConnectionSetting)
        Return Await client.Item(String.Join(",", IDs)).FD_Move(DestinationFolderID)
    End Function
#End Region

#Region "CopyMultipleFile"
    Public Async Function POST_CopyMultipleFile(DestinationFolderID As String) As Task(Of Boolean) Implements IItems.F_Copy
        Dim client As New ZClient(authToken, ConnectionSetting)
        Return Await client.Item(String.Join(",", IDs)).F_Copy(DestinationFolderID)
    End Function
#End Region

#Region "CopyMultipleFolder"
    Public Async Function POST_CopyMultipleFolder(DestinationFolderID As String) As Task(Of Boolean) Implements IItems.D_Copy
        Dim client As New ZClient(authToken, ConnectionSetting)
        Return Await client.Item(String.Join(",", IDs)).D_Copy(DestinationFolderID)
    End Function
#End Region



End Class
