Public Class JSON_SharesMetadata
    Public Property response As JSON_SharedDetails_Response

    Public ReadOnly Property MailsList As List(Of String)
        Get
            Dim co_Owner = response.result.coOwner.sharedUsers.Split(",").ToList()
            Dim Read_Only = response.result.ReadOnly.sharedUsers.Split(",").ToList()
            Dim Read_Write = response.result.ReadWrite.sharedUsers.Split(",").ToList()
            Dim all = co_Owner.Concat(Read_Only).Concat(Read_Write).Distinct().ToList() 'merege all lists
            Return all.Where(Function(s) Not String.IsNullOrEmpty(s)).ToList() 'remove empty
        End Get
    End Property

    Public ReadOnly Property SharedFileUrl As String
        Get
            Return response.result.permaLink
        End Get
    End Property

    Enum SharedORPublic
        [Shared]
        [Public]
    End Enum
    Public ReadOnly Property Shared_OR_Public As SharedORPublic
        Get
            If response.result.visibility = "private" Then
                Return SharedORPublic.Shared
            ElseIf response.result.visibility = "linkshare" Then
                Return SharedORPublic.Public
            End If
        End Get
    End Property
End Class
Public Class JSON_SharedDetails_Response
    Public Property result As JSON_SharedDetails_Result
    Public Property uri As String
    Public Property message As String
End Class
Public Class JSON_SharedDetails_Result
    Public Property [ReadOnly] As JSON_SharedDetails_Readonly
    Public Property permaLink As String
    Public Property coOwner As JSON_SharedDetails_Coowner
    Public Property visibility As String
    Public Property ReadComment As JSON_SharedDetails_Readcomment
    Public Property permission As String
    Public Property ReadWrite As JSON_SharedDetails_Readwrite
End Class
Public Class JSON_SharedDetails_Readonly
    Public Property sharedUsers As String
    Public Property isDirectlyShared As String
    Public Property sharedUserZuids As String
    Public Property sharedGroups As String
    Public Property sharedEmails As String
    Public Property isDirectlyEmailShared As String
    Public Property sharedGroupNames As String
    Public Property isDirectlyGroupShared As String
End Class
Public Class JSON_SharedDetails_Coowner
    Public Property sharedUsers As String
    Public Property isDirectlyShared As String
    Public Property sharedUserZuids As String
    Public Property sharedEmails As String
    Public Property isDirectlyEmailShared As String
End Class
Public Class JSON_SharedDetails_Readcomment
    Public Property sharedUsers As String
    Public Property sharedUserZuids As String
    Public Property sharedGroups As String
    Public Property sharedEmails As String
    Public Property sharedGroupNames As String
    Public Property isDirectlyGroupShared As String
End Class
Public Class JSON_SharedDetails_Readwrite
    Public Property sharedUsers As String
    Public Property isDirectlyShared As String
    Public Property sharedUserZuids As String
    Public Property sharedGroups As String
    Public Property sharedEmails As String
    Public Property isDirectlyEmailShared As String
    Public Property sharedGroupNames As String
    Public Property isDirectlyGroupShared As String
End Class
