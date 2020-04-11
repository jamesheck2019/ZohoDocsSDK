Public Class Form1

    Private Async Sub Button1_ClickAsync(sender As Object, e As EventArgs) Handles Button1.Click
        Dim clnt As ZohoDocsSDK.IClient = New ZohoDocsSDK.ZClient("xxxxxxxxxxxxxxx")

        'Dim rslt = Await clnt.Root.List()
        'rslt.FILES.ForEach(Sub(f) FlowLayoutPanel1.Controls.Add(New PropertyGridEx(f)))
        'rslt.FOLDER.ForEach(Sub(f) FlowLayoutPanel1.Controls.Add(New PropertyGridEx(f)))

        'Dim rslt = Await clnt.Root.ListFiles
        'rslt.ForEach(Sub(f) FlowLayoutPanel1.Controls.Add(New PropertyGridEx(f)))

        Dim rslt = Await clnt.Folder("bnvgv18f2c09286db43b09986db231d2adacc").List2
        rslt.FOLDER.ForEach(Sub(f) FlowLayoutPanel1.Controls.Add(New PropertyGridEx(f)))


    End Sub
End Class

Public Class PropertyGridEx
    Inherits PropertyGrid

    Public Sub New(obj As Object)
        MyBase.HelpVisible = False
        MyBase.Width = 250
        MyBase.Height = 350
        MyBase.SelectedObject = obj
    End Sub
End Class