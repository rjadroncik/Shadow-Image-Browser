Public Class FormFulscreen

    Private WithEvents Viewer As ImageViewer = New ImageViewer

    Private Dragging As Boolean = False

    Private Folder As String = Nothing

    Private ImageShown As Integer = -1

    Private Images() As String = Nothing

    Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.        

        Viewer.Dock = DockStyle.Fill

        Me.Controls.Add(Viewer)

        Me.Width = Screen.PrimaryScreen.Bounds.Width
        Me.Height = Screen.PrimaryScreen.Bounds.Height

    End Sub

    Public Sub SetFolder(ByVal folder As String)

        Me.Folder = folder

    End Sub

    Public Sub SetImages(ByVal images() As String)

        Me.Images = images

    End Sub

    Public Sub SetImageShown(ByVal image As Integer)

        ImageShown = image

        Viewer.SetImageShown(Folder + "\"c + Images(image))

        Viewer.Zoom = ImageViewerZoom.Fit
        
    End Sub

    Private Sub FormFulscreen_Shown(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Shown


    End Sub


    Private Sub ImageShowNext()

        SetImageShown((Me.ImageShown + 1) Mod Me.Images.Length)

    End Sub

    Private Sub ImagePreviousNext()

        If Me.ImageShown = 0 Then

            SetImageShown(Me.Images.Length - 1)
        Else

            SetImageShown(Me.ImageShown - 1)
        End If

    End Sub

    Private Sub FormFulscreen_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Viewer.KeyDown

        If e.KeyCode = Keys.Escape Then

            Me.Visible = False

        ElseIf e.KeyCode = Keys.Space Then

            ImageShowNext()

        ElseIf e.KeyCode = Keys.Back Then

            ImagePreviousNext()

        ElseIf e.KeyCode = Keys.Add Then

            Viewer.Zoom = ImageViewerZoom.Actual
            Viewer.Invalidate()
            
        ElseIf e.KeyCode = Keys.Subtract Then

            Viewer.Zoom = ImageViewerZoom.Fit
            Viewer.Invalidate()

        End If

    End Sub

    Private Sub FormFulscreen_MouseWheel(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Viewer.MouseWheel

        If e.Delta < 0 Then

            ImageShowNext()
        Else
            ImagePreviousNext()
        End If

    End Sub

    Private Sub PictureBox_DoubleClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Viewer.MouseDoubleClick

        If e.Button = Windows.Forms.MouseButtons.Left Then

            Me.Visible = False

        End If

    End Sub

    Private Sub PictureBox_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Viewer.MouseDown

        If Viewer.Zoom = ImageViewerZoom.Actual Then

            Me.Dragging = True

            MousePosStart.X = e.Location.X
            MousePosStart.Y = e.Location.Y

            OffsetValueStart.X = Viewer.Offset.X
            OffsetValueStart.Y = Viewer.Offset.Y

        End If

        If e.Button = Windows.Forms.MouseButtons.XButton2 Then

            ImageShowNext()

        ElseIf e.Button = Windows.Forms.MouseButtons.XButton1 Then

            ImagePreviousNext()

        ElseIf e.Button = Windows.Forms.MouseButtons.Right Then

            Viewer.Zoom = ImageViewerZoom.Actual
            Viewer.Invalidate()

        End If

    End Sub

    Private OffsetValueStart As Point = New Point(0, 0)
    Private MousePosStart As Point = New Point(0, 0)

    Private Sub PictureBox_MouseMove(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Viewer.MouseMove

        If Me.Dragging Then

            Viewer.Offset = New Point(OffsetValueStart.X + (e.Location.X - MousePosStart.X), OffsetValueStart.Y + (e.Location.Y - MousePosStart.Y))
            Viewer.Invalidate()

        End If

    End Sub

    Private Sub PictureBox_MouseUp(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Viewer.MouseUp

        Me.Dragging = False

    End Sub
End Class