Imports Ionic.Zip

Public Class FormMain

    Private MyFormFulscreen As FormFulscreen = New FormFulscreen

    Structure TreeNode_Tag

        Public Sub New(ByVal isZip As Boolean, ByVal isRead As Boolean, ByVal isInsideZip As Boolean)

            Me.IsZip = isZip
            Me.IsRead = isRead

            Me.IsInsideZip = isInsideZip

        End Sub

        Public IsZip As Boolean
        Public IsRead As Boolean

        Public IsInsideZip As Boolean

    End Structure


    Public Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        TreeView_Init()

        Dim lastDir As String = GetSetting("Shadow Image Browser", "History", "LastDir")

        If lastDir.Length Then

            TreeView_ShowPath(lastDir)

        End If

    End Sub

    Private Sub TreeView_ShowPath(ByVal path As String)

        Dim parts As String() = path.ToLower().Split("\"c)
        Dim nodes As TreeNodeCollection = TreeView.Nodes

        Dim lastNode As TreeNode = Nothing

        For Each part As String In parts

            For Each node As TreeNode In nodes

                If node.Text.ToLower().Equals(part) Then

                    Dim tag As TreeNode_Tag = DirectCast(node.Tag, TreeNode_Tag)

                    If Not tag.IsRead Then

                        TreeView_Expand(node, tag)

                        node.Tag = tag

                    End If

                    lastNode = node
                    nodes = node.Nodes
                    Exit For

                End If

            Next

        Next

        If lastNode IsNot Nothing Then

            TreeView.SelectedNode = lastNode

            lastNode.Expand()
            lastNode.EnsureVisible()

        End If

    End Sub

    Private Sub TreeView_Init()

        Dim NewNode As TreeNode = New TreeNode("C:")
        NewNode.Tag = New TreeNode_Tag(False, False, False)
        TreeView.Nodes.Add(NewNode)

        NewNode = New TreeNode("D:")
        NewNode.Tag = New TreeNode_Tag(False, False, False)
        TreeView.Nodes.Add(NewNode)

        NewNode = New TreeNode("E:")
        NewNode.Tag = New TreeNode_Tag(False, False, False)
        TreeView.Nodes.Add(NewNode)

    End Sub

    Private Sub TreeView_Expand(ByRef node As TreeNode, ByRef tag As TreeNode_Tag)

        If tag.IsRead = True Then
            Exit Sub
        End If

        tag.IsRead = True

        If tag.IsZip Then

            Using zip As ZipFile = ZipFile.Read(node.FullPath)

                Dim NewNode As TreeNode

                For Each entry As ZipEntry In zip

                    If entry.IsDirectory Then

                        NewNode = New TreeNode(entry.FileName.Substring(0, entry.FileName.Length - 1))
                        NewNode.Tag = New TreeNode_Tag(False, False, True)

                        node.Nodes.Add(NewNode)

                    End If

                Next

            End Using

        ElseIf tag.IsInsideZip Then

            Dim zipPath As String = node.Text
            Dim zipNode As TreeNode = node.Parent

            While Not DirectCast(zipNode.Tag, TreeNode_Tag).IsZip

                zipPath = zipNode.Text + "\"c + zipPath
                zipNode = zipNode.Parent

            End While

            Using zip As ZipFile = ZipFile.Read(zipNode.FullPath)

                Dim NewNode As TreeNode

                For Each entry As ZipEntry In zip.SelectEntries("*", zipPath + "/")

                    If entry.IsDirectory Then

                        NewNode = New TreeNode(entry.FileName)
                        NewNode.Tag = New TreeNode_Tag(False, False, True)

                        node.Nodes.Add(NewNode)

                    End If

                Next

            End Using

        Else

            Dim dir As New IO.DirectoryInfo(node.FullPath + "\")

            Dim NewNode As TreeNode

            For Each file As IO.FileInfo In dir.GetFiles()

                If file.Extension.ToLower().Equals(".zip") Then

                    NewNode = New TreeNode(file.Name)
                    NewNode.Tag = New TreeNode_Tag(True, False, False)
                    NewNode.ImageIndex = 2
                    NewNode.SelectedImageIndex = 2

                    node.Nodes.Add(NewNode)

                End If

            Next

            For Each subdir As IO.DirectoryInfo In dir.GetDirectories()

                NewNode = New TreeNode(subdir.Name)
                NewNode.Tag = New TreeNode_Tag(False, False, False)

                node.Nodes.Add(NewNode)
            Next

        End If

    End Sub

    Private Sub TreeView_AfterSelect(ByVal sender As System.Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) Handles TreeView.AfterSelect

        Dim tag As TreeNode_Tag = DirectCast(e.Node.Tag, TreeNode_Tag)

        If tag.IsRead = False Then

            TreeView_Expand(e.Node, tag)

            e.Node.Tag = tag

        End If

        ListView_Update(e.Node, tag)

        'If TreeView.SelectedNode IsNot Nothing Then

        SaveSetting("Shadow Image Browser", "History", "LastDir", TreeView.SelectedNode.FullPath)

        'End If

    End Sub

    Private Sub ListView_Update(ByRef node As TreeNode, ByRef tag As TreeNode_Tag)

        ListView.Clear()

        If tag.IsZip Then

        ElseIf tag.IsInsideZip Then

        Else

            Dim dir As New IO.DirectoryInfo(node.FullPath + "\")

            Dim NewItem As ListViewItem

            For Each file As IO.FileInfo In dir.GetFiles()

                If file.Extension.ToLower().Equals(".jpg") Or file.Extension.ToLower().Equals(".jpeg") Or file.Extension.ToLower().Equals(".png") Or file.Extension.ToLower().Equals(".gif") Then

                    NewItem = New ListViewItem(file.Name)

                    ListView.Items.Add(NewItem)

                End If

            Next

        End If

        Dim images(ListView.Items.Count - 1) As String

        For i As Integer = 0 To ListView.Items.Count - 1

            images(i) = ListView.Items(i).Text()

        Next

        MyFormFulscreen.SetFolder(TreeView.SelectedNode.FullPath)
        MyFormFulscreen.SetImages(images)

    End Sub

    Private Sub ListView_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ListView.SelectedIndexChanged

    End Sub

    Private Sub ListView_MouseDoubleClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles ListView.MouseDoubleClick

        If ListView.SelectedItems.Count Then

            MyFormFulscreen.SetImageShown(ListView.SelectedIndices(0))
            MyFormFulscreen.Visible = True

        End If

    End Sub

    Private Sub FormMain_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown, ListView.KeyDown, TreeView.KeyDown

        If e.KeyCode = Keys.Escape Then

            Me.Close()

        End If

    End Sub

End Class
