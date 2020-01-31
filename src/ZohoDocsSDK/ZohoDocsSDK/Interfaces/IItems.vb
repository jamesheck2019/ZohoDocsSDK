Public Interface IItems

    ''' <summary>
    ''' Moves multiple files/folders to a specified location
    ''' </summary>
    ''' <param name="DestinationFolderID">Mandatory. Id of folder to copy the new files/folders</param>
    Function FD_Move(DestinationFolderID As String) As Task(Of Boolean)
    ''' <summary>
    ''' Copies multiple files to a new location
    ''' https://apidocs.zoho.com/files/v1/copy?authtoken=AuthToken&scope=docsapi
    ''' </summary>
    ''' <param name="DestinationFolderID">Mandatory. Id of folder to copy the new files</param>
    Function F_Copy(DestinationFolderID As String) As Task(Of Boolean)
    ''' <summary>
    ''' Copies the existing folders to a specified folder
    ''' https://apidocs.zoho.com/files/v1/folders/copy?authtoken=AuthToken&scope=docsapi
    ''' </summary>
    ''' <param name="DestinationFolderID">Mandatory. Id of folder to copy the new folders</param>
    Function D_Copy(DestinationFolderID As String) As Task(Of Boolean)

End Interface
