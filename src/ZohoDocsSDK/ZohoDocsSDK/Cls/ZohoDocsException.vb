Imports System

Public Class ZohoDocsException
    Inherits Exception

    Public Sub New(ByVal errorMessage As String, ByVal errorCode As String)
        MyBase.New(errorMessage)
    End Sub
End Class


Public Class ExceptionCls
    Public Shared Function CreateException(errorMesage As String, errorCode As String) As ZohoDocsException
        Return New ZohoDocsException(errorMesage, errorCode)
    End Function
End Class

