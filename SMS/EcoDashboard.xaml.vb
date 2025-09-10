Imports System.Windows.Threading
Imports System.Windows.Media.Animation

Partial Public Class EcoDashboard
    Inherits System.Windows.Controls.UserControl

    Private rnd As New Random()
    Private updateTimer As DispatcherTimer

    Public Sub New()
        InitializeComponent()

        ' Simulate live updates every 3 seconds for demo
        updateTimer = New DispatcherTimer()
        updateTimer.Interval = TimeSpan.FromSeconds(3)
        AddHandler updateTimer.Tick, AddressOf UpdateValues
        updateTimer.Start()

        ' small entrance animation
        Dim fadeIn As New DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.6))
        Me.BeginAnimation(OpacityProperty, fadeIn)
    End Sub

    ' Example public setters (use these to push real data into the control)
    Public Sub SetSolar(value As Double, todayKwh As Double)
        SolarValue.Text = $"{value:F1} kW"
        SolarToday.Text = $"{todayKwh:F1} kWh"
    End Sub

    Public Sub SetCo2(valueKg As Double)
        Co2Value.Text = $"{valueKg:F1} kg"
        Co2ValueSmall.Text = $"{valueKg:F1} kg"
        Co2Today.Text = $"Today: {valueKg:F1} kg"
    End Sub

    Public Sub SetTrees(count As Double)
        TreesValue.Text = $"{count:F1}"
    End Sub

    ' Demo updater — replace with real telemetry feed
    Private Sub UpdateValues(sender As Object, e As EventArgs)
        ' simulate values
        Dim solarNow = 10.0 + rnd.NextDouble() * 12.0       ' 10..22 kW
        Dim todayKwh = 50 + rnd.NextDouble() * 120         ' 50..170 kWh
        Dim co2 = rnd.NextDouble() * 15.0                  ' 0..15 kg
        Dim trees = Math.Round(1.0 + rnd.NextDouble() * 6.0, 1)

        ' animate numeric change (simple fade)
        AnimateTextChange(SolarValue, $"{solarNow:F1} kW")
        AnimateTextChange(SolarToday, $"{todayKwh:F1} kWh")
        AnimateTextChange(Co2Value, $"{co2:F1} kg")
        AnimateTextChange(Co2ValueSmall, $"{co2:F1} kg")
        AnimateTextChange(TreesValue, $"{trees:F1}")

        ' update clock
        ClockText.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm")
    End Sub

    Private Sub AnimateTextChange(tb As TextBlock, newText As String)
        Dim fadeOut As New DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(150))
        AddHandler fadeOut.Completed, Sub()
                                          tb.Text = newText
                                          Dim fadeIn As New DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(200))
                                          tb.BeginAnimation(TextBlock.OpacityProperty, fadeIn)
                                      End Sub
        tb.BeginAnimation(TextBlock.OpacityProperty, fadeOut)
    End Sub
End Class