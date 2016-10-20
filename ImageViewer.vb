Imports System.Drawing
Imports System.Drawing.Drawing2D

Public Enum ImageViewerZoom

    Fit = 0
    Actual = 1

End Enum

Public Class ImageViewer

    Private OffsetValue As Point = New Point(100, 100)

    Private ImageShown As Image = Nothing


    Public Property Offset() As Point

        Get
            Return OffsetValue
        End Get

        Set(ByVal Value As Point)

            If Not ZoomValue = ImageViewerZoom.Fit Then

                OffsetValue = Value

            End If

        End Set

    End Property


    Public Property Zoom() As ImageViewerZoom

        Get
            Return ZoomValue
        End Get

        Set(ByVal Value As ImageViewerZoom)

            ZoomValue = Value

            CenterImage()

        End Set

    End Property

    Private ZoomValue As ImageViewerZoom

    Public Sub SetImageShown(ByVal image As String)

        ImageShown = New Bitmap(image)

        Me.Invalidate()

    End Sub

    Protected Overrides Sub OnPaintBackground(ByVal e As System.Windows.Forms.PaintEventArgs)

        e.Graphics.FillRectangle(Brushes.DimGray, New Rectangle(0, 0, Me.Width, Me.Height))

    End Sub


    Private ScaleValue As Single = 1

    Protected Overrides Sub OnPaint(ByVal e As System.Windows.Forms.PaintEventArgs)
        MyBase.OnPaint(e)

        Dim mx As New Matrix(ScaleValue, 0, 0, ScaleValue, 0, 0)

        mx.Translate(OffsetValue.X / ScaleValue, OffsetValue.Y / ScaleValue)

        e.Graphics.Transform = mx
        e.Graphics.InterpolationMode = InterpolationMode.High

        e.Graphics.DrawImage(ImageShown, New Rectangle(0, 0, Me.ImageShown.Width, Me.ImageShown.Height), 0, 0, ImageShown.Width, ImageShown.Height, GraphicsUnit.Pixel)

        MyBase.OnPaint(e)

    End Sub

    Sub CenterImage()

        If ZoomValue = ImageViewerZoom.Fit Then

            Dim ScaleX As Single = Me.Width / ImageShown.Width
            Dim ScaleY As Single = Me.Height / ImageShown.Height

            ScaleValue = If(ScaleX > ScaleY, ScaleY, ScaleX)

            Dim ImageShownSize As Point = New Point(ScaleValue * ImageShown.Width, ScaleValue * ImageShown.Height)

            OffsetValue.X = (Me.Width - ImageShownSize.X) / 2
            OffsetValue.Y = (Me.Height - ImageShownSize.Y) / 2
        Else

            ScaleValue = 1

            OffsetValue.X = (Me.Width - ImageShown.Width) / 2
            OffsetValue.Y = (Me.Height - ImageShown.Height) / 2

        End If

    End Sub

    Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.     

        Me.DoubleBuffered = True

    End Sub

End Class
