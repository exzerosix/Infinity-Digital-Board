Imports System.Windows.Threading
Imports System.Windows.Media.Animation
Imports SMS.FlightBoard

Partial Public Class MainWindow
    Inherits Window

    ' Video playlist
    Private videos As New List(Of String) From {
        "E:\aviation-school.mov",
        "E:\def.mov"
    }
    Private currentVideoIndex As Integer = 0

    ' Timer for static page and flight board
    Private staticTimer As DispatcherTimer
    Private flightBoardTimer As DispatcherTimer
    Private tickerTimer As DispatcherTimer
    Private ecoTimer As DispatcherTimer

    Private currentTickerIndex As Integer = 0

    Private staticTimerSeconds As Integer = 5
    Private flightBoardTimerSeconds As Integer = 10

    ' Flight data
    Private allFlights As List(Of FlightBoard.FlightInfo)
    Private currentFlightPage As Integer = 0

    Private tickerMessages As New List(Of String) From {
        "📅 Parent-Teacher Meeting on Sept 15, 9:00 AM",
        "🏆 Sportsfest Next Week – Don’t Miss It!",
        "🌤 Weather Today: Sunny 31°C, Light Winds",
        "📢 Reminder: Submit assignments before Friday"
    }

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub MainWindow_Loaded(sender As Object, e As RoutedEventArgs)
        ' Start with static page
        ShowStaticPage()

        ' Handle video end
        AddHandler AdVideo.MediaEnded, AddressOf VideoEnded

        ' Initialize ticker
        ShowNextTickerMessage()
        tickerTimer = New DispatcherTimer()
        tickerTimer.Interval = TimeSpan.FromMilliseconds(20)
        AddHandler tickerTimer.Tick, AddressOf MoveTicker
        tickerTimer.Start()
    End Sub

    Private Sub ShowStaticPage()

        SetSpacedText(TitleTextBlock, "DPR - PHILIPPINE ACADEMY TECHNOLOGY & SCIENCE", 2)

        currentVideoIndex = 0
        StaticPage.Visibility = Visibility.Visible
        StaticPage.Opacity = 1

        EcoDashboardPage.Visibility = Visibility.Collapsed

        FlightBoardAds.Visibility = Visibility.Collapsed
        AdVideo.Stop()
        AdVideo.Source = Nothing
        AdVideo.Visibility = Visibility.Collapsed

        staticTimer = New DispatcherTimer()
        staticTimer.Interval = TimeSpan.FromSeconds(staticTimerSeconds)
        AddHandler staticTimer.Tick,
            Sub()
                staticTimer.Stop()
                ShowEcoDashboardPage()
            End Sub
        staticTimer.Start()
    End Sub

    Private Sub ShowEcoDashboardPage()
        StaticPage.Visibility = Visibility.Collapsed
        EcoDashboardPage.Visibility = Visibility.Visible
        FlightBoardAds.Visibility = Visibility.Collapsed
        AdVideo.Visibility = Visibility.Collapsed

        ' Show EcoDashboard for 20 seconds, then go to FlightBoard
        flightBoardTimer = New DispatcherTimer()
        flightBoardTimer.Interval = TimeSpan.FromSeconds(20)
        AddHandler flightBoardTimer.Tick,
        Sub()
            flightBoardTimer.Stop()
            ShowFlightBoardPage(Nothing, Nothing)
        End Sub
        flightBoardTimer.Start()
    End Sub

    Private Sub SetSpacedText(tb As TextBlock, text As String, spacing As Double)
        tb.Inlines.Clear()
        For Each c As Char In text
            Dim charBlock As New TextBlock With {
            .Text = c,
            .FontFamily = tb.FontFamily,
            .FontSize = tb.FontSize,
            .FontWeight = tb.FontWeight,
            .Foreground = tb.Foreground,
            .Margin = New Thickness(spacing / 2, 0, spacing / 2, 0)
        }
            tb.Inlines.Add(New InlineUIContainer(charBlock))
        Next
    End Sub

    Private Sub ShowFlightBoardPage(sender As Object, e As EventArgs)
        StaticPage.Visibility = Visibility.Collapsed
        AdVideo.Visibility = Visibility.Collapsed
        EcoDashboardPage.Visibility = Visibility.Collapsed
        FlightBoardAds.Visibility = Visibility.Visible

        ' Initialize flights once
        allFlights = New List(Of FlightBoard.FlightInfo) From {
            New FlightBoard.FlightInfo With {.FlightNo = "PR 102", .Destination = "Tokyo", .Time = "08:45", .Gate = "A3", .Status = "Boarding"},
            New FlightBoard.FlightInfo With {.FlightNo = "SQ 921", .Destination = "Singapore", .Time = "09:15", .Gate = "B1", .Status = "On Time"},
            New FlightBoard.FlightInfo With {.FlightNo = "CX 905", .Destination = "Hong Kong", .Time = "10:30", .Gate = "C4", .Status = "Delayed"},
            New FlightBoard.FlightInfo With {.FlightNo = "JL 745", .Destination = "Osaka", .Time = "11:00", .Gate = "D2", .Status = "Cancelled"},
            New FlightBoard.FlightInfo With {.FlightNo = "AA 300", .Destination = "New York", .Time = "11:45", .Gate = "E1", .Status = "Boarding"},
            New FlightBoard.FlightInfo With {.FlightNo = "BA 220", .Destination = "London", .Time = "12:15", .Gate = "F3", .Status = "On Time"},
            New FlightBoard.FlightInfo With {.FlightNo = "AF 750", .Destination = "Paris", .Time = "12:45", .Gate = "G2", .Status = "Delayed"},
            New FlightBoard.FlightInfo With {.FlightNo = "LH 820", .Destination = "Frankfurt", .Time = "13:00", .Gate = "H4", .Status = "On Time"},
            New FlightBoard.FlightInfo With {.FlightNo = "EK 500", .Destination = "Dubai", .Time = "13:30", .Gate = "I1", .Status = "Boarding"},
            New FlightBoard.FlightInfo With {.FlightNo = "QF 100", .Destination = "Sydney", .Time = "14:00", .Gate = "J2", .Status = "On Time"},
            New FlightBoard.FlightInfo With {.FlightNo = "NH 200", .Destination = "Seoul", .Time = "14:30", .Gate = "K3", .Status = "Boarding"},
            New FlightBoard.FlightInfo With {.FlightNo = "CX 801", .Destination = "Hong Kong", .Time = "15:00", .Gate = "C4", .Status = "Delayed"},
            New FlightBoard.FlightInfo With {.FlightNo = "SQ 910", .Destination = "Singapore", .Time = "15:30", .Gate = "B1", .Status = "On Time"},
            New FlightBoard.FlightInfo With {.FlightNo = "JL 752", .Destination = "Osaka", .Time = "16:00", .Gate = "D2", .Status = "Cancelled"},
            New FlightBoard.FlightInfo With {.FlightNo = "PR 120", .Destination = "Tokyo", .Time = "16:30", .Gate = "A3", .Status = "Boarding"},
            New FlightBoard.FlightInfo With {.FlightNo = "AA 320", .Destination = "New York", .Time = "17:00", .Gate = "E1", .Status = "On Time"},
            New FlightBoard.FlightInfo With {.FlightNo = "BA 240", .Destination = "London", .Time = "17:30", .Gate = "F3", .Status = "Delayed"},
            New FlightBoard.FlightInfo With {.FlightNo = "AF 770", .Destination = "Paris", .Time = "18:00", .Gate = "G2", .Status = "On Time"},
            New FlightBoard.FlightInfo With {.FlightNo = "LH 830", .Destination = "Frankfurt", .Time = "18:30", .Gate = "H4", .Status = "Boarding"},
            New FlightBoard.FlightInfo With {.FlightNo = "EK 520", .Destination = "Dubai", .Time = "19:00", .Gate = "I1", .Status = "On Time"}
        }

        currentFlightPage = 0
        ShowCurrentFlightPage()

        ' Setup flight board cycling timer
        flightBoardTimer = New DispatcherTimer()
        flightBoardTimer.Interval = TimeSpan.FromSeconds(flightBoardTimerSeconds)
        AddHandler flightBoardTimer.Tick, AddressOf ShowNextFlightPage
        flightBoardTimer.Start()
    End Sub

    Private Sub ShowCurrentFlightPage()
        Dim pageFlights = allFlights.Skip(currentFlightPage * 10).Take(10).ToList()
        FlightBoardAds.ShowFlights(pageFlights)
    End Sub

    Private Sub ShowNextFlightPage(sender As Object, e As EventArgs)
        currentFlightPage += 1

        If currentFlightPage * 10 >= allFlights.Count Then
            ' All flights shown → stop and go to video
            flightBoardTimer.Stop()
            SwitchToVideo(Nothing, Nothing)
        Else
            ShowCurrentFlightPage()
        End If
    End Sub

    Private Sub SwitchToVideo(sender As Object, e As EventArgs)
        If staticTimer IsNot Nothing Then staticTimer.Stop()
        If flightBoardTimer IsNot Nothing Then flightBoardTimer.Stop()

        If videos.Count = 0 Then
            ShowStaticPage()
            Return
        End If

        PlayVideo(currentVideoIndex)
    End Sub

    Private Sub PlayVideo(index As Integer)
        If index < 0 OrElse index >= videos.Count Then
            ShowStaticPage()
            Return
        End If

        StaticPage.Visibility = Visibility.Collapsed
        FlightBoardAds.Visibility = Visibility.Collapsed

        AdVideo.Visibility = Visibility.Visible
        AdVideo.Opacity = 1
        AdVideo.Source = New Uri(videos(index))
        AdVideo.Position = TimeSpan.Zero
        AdVideo.Play()
    End Sub

    Private Sub VideoEnded(sender As Object, e As RoutedEventArgs)
        currentVideoIndex += 1
        If currentVideoIndex < videos.Count Then
            PlayVideo(currentVideoIndex)
        Else
            ShowStaticPage()
        End If
    End Sub

    ' --- Ticker ---
    Private Sub ShowNextTickerMessage()
        TickerText.Text = tickerMessages(currentTickerIndex)
        Canvas.SetLeft(TickerText, 1600)
        currentTickerIndex = (currentTickerIndex + 1) Mod tickerMessages.Count
    End Sub

    Private Sub MoveTicker(sender As Object, e As EventArgs)
        Dim left As Double = Canvas.GetLeft(TickerText)
        If Double.IsNaN(left) Then
            Canvas.SetLeft(TickerText, Me.ActualWidth)
            Return
        End If

        Canvas.SetLeft(TickerText, left - 2)

        If left + TickerText.ActualWidth < 0 Then
            ShowNextTickerMessage()
        End If
    End Sub
End Class
