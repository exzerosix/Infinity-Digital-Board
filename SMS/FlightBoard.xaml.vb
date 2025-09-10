Imports System.Windows.Media
Imports System.Windows.Media.Animation
Imports System.Windows.Controls
Imports System.Windows.Threading

Partial Public Class FlightBoard
    Inherits UserControl

    Private flightPageIndex As Integer = 0
    Private flightsPerPage As Integer = 10
    Private allFlights As List(Of FlightBoard.FlightInfo)
    Private flightBoardTimer As DispatcherTimer
    Private flightBoardTimerSeconds As Integer = 10

    Public Class FlightInfo
        Public Property FlightNo As String
        Public Property Destination As String
        Public Property [Time] As String
        Public Property Gate As String
        Public Property Status As String
    End Class

    Public Sub New()
        InitializeComponent()
    End Sub

    ' Public helper so MainWindow can call it
    Public Sub ShowFlights(flights As List(Of FlightInfo))
        FlightBoardGrid.Children.Clear()
        For i As Integer = 0 To flights.Count - 1
            FlightBoardGrid.Children.Add(MakeRow(flights(i), i))
        Next
    End Sub

    Private Function MakeRow(f As FlightInfo, rowIndex As Integer) As Grid
        Dim colWeights As Double() = {1, 4, 1.2, 0.8, 2}

        '    Dim rowGrid As New Grid() With {
        '    .Background = If(rowIndex Mod 2 = 0, Brushes.Black, New SolidColorBrush(Color.FromRgb(20, 20, 20))) ' Alternate black & dark gray
        '}

        Dim rowGrid As New Grid()

        For Each w In colWeights
            rowGrid.ColumnDefinitions.Add(New ColumnDefinition With {.Width = New GridLength(w, GridUnitType.Star)})
        Next

        Dim vFlightNo = If(f IsNot Nothing, f.FlightNo, "")
        Dim vDestination = If(f IsNot Nothing, f.Destination, "")
        Dim vTime = If(f IsNot Nothing, f.Time, "")
        Dim vGate = If(f IsNot Nothing, f.Gate, "")
        Dim vStatus = If(f IsNot Nothing, f.Status, "")

        rowGrid.Children.Add(MakeCell(vFlightNo, HorizontalAlignment.Left, 0))
        rowGrid.Children.Add(MakeCell(vDestination, HorizontalAlignment.Left, 1))
        rowGrid.Children.Add(MakeCell(vTime, HorizontalAlignment.Center, 2))
        rowGrid.Children.Add(MakeCell(vGate, HorizontalAlignment.Center, 3))
        rowGrid.Children.Add(MakeCell(vStatus, HorizontalAlignment.Left, 4))

        ' Animate in
        rowGrid.Opacity = 0
        Dim fade As New DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.45)) With {
            .BeginTime = TimeSpan.FromMilliseconds(rowIndex * 120)
        }
        rowGrid.BeginAnimation(UIElement.OpacityProperty, fade)

        Dim scale As New ScaleTransform(1, 0.12)
        rowGrid.RenderTransformOrigin = New Point(0.5, 0.5)
        rowGrid.RenderTransform = scale
        Dim flipAnim As New DoubleAnimation(0.12, 1, TimeSpan.FromSeconds(0.45)) With {
            .BeginTime = fade.BeginTime
        }
        scale.BeginAnimation(ScaleTransform.ScaleYProperty, flipAnim)

        Return rowGrid
    End Function

    Private Function MakeCell(text As String, hAlign As HorizontalAlignment, col As Integer) As TextBlock
        Dim tb As New TextBlock With {
            .Text = text,
            .Foreground = Brushes.LimeGreen,
            .FontFamily = New FontFamily("Consolas"),
            .FontSize = 40,
            .VerticalAlignment = VerticalAlignment.Center,
            .HorizontalAlignment = hAlign,
            .Margin = New Thickness(6, 0, 6, 0)
        }
        Grid.SetColumn(tb, col)
        Return tb
    End Function
End Class
