Imports System.Net
Imports System.Net.WebSockets
Imports System.Text
Imports System.Threading
Imports System.Threading.Tasks
Imports MonitoredApplication
Imports Newtonsoft.Json.Linq

Public Class Form1

    Private ws As ClientWebSocket
    Private cts As CancellationTokenSource
    Private clientLoopTask As Task

    Private reconnectEnabled As Boolean = False
    Private initialDelayMs As Integer = 1000
    Private maxDelayMs As Integer = 30000
    Private mbReceivedPING As Boolean = True
    Dim delayMs As Integer


    Public Sub startServer()
        OnStart()
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'Start the web socket
        Try
            Dim sIPAddress As String = ReadSetting.ReadString("Interface Server", "IPAddress", "127.0.0.1")
            Dim sPort As String = ReadSetting.ReadString("Interface Server", "Port", "80")

            ReadSettings()

            Me.nudPort.Value = Integer.Parse(sPort)

            Dim asIPAddress As String() = sIPAddress.Split(".")
            Me.nudIP1.Value = Integer.Parse(asIPAddress(0))
            Me.nudIP2.Value = Integer.Parse(asIPAddress(1))
            Me.nudIP3.Value = Integer.Parse(asIPAddress(2))
            Me.nudIP4.Value = Integer.Parse(asIPAddress(3))

            Me.WindowState = FormWindowState.Minimized
            startServer()

            'ByPass SSL Certificate Validation Checking
            'System.Net.ServicePointManager.ServerCertificateValidationCallback =
            '  Function(se As Object,
            '  cert As System.Security.Cryptography.X509Certificates.X509Certificate,
            '  chain As System.Security.Cryptography.X509Certificates.X509Chain,
            '  sslerror As System.Net.Security.SslPolicyErrors) True

            'Call web application/web service with HTTPS URL

            'Restore SSL Certificate Validation Checking
            'System.Net.ServicePointManager.ServerCertificateValidationCallback = Nothing

            'for .net > 3.5
            Try
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 Or SecurityProtocolType.Tls12 Or SecurityProtocolType.Tls13
            Catch
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 Or SecurityProtocolType.Tls12
            End Try


            mWatchdogWatcher = New WatchdogWatcher("WatchDogPOS", "WatchDogPOS.exe", 5000)

                If (mbPrintServerUseWebsocket) Then
                    lblWebsocket.Visible = True
                    lblWebsocketStatus.Visible = True
                    reconnectEnabled = True
                    clientLoopTask = ClientLoopAsync(msPrintServerUseWebsocketServerUrl) ' fire-and-forget
                Else
                    lblWebsocket.Visible = False
                    lblWebsocketStatus.Visible = False
                End If

            Catch ex As Exception
                MsgBox(ex.Message)
        End Try



    End Sub

    Public Sub EnableSaveSettingsbutton()
        Me.btnSaveSettings.Enabled = True
    End Sub

    Private Sub Form1_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
        If (Me.WindowState = FormWindowState.Minimized) Then
            Me.ShowInTaskbar = False
            Me.Hide()
            NotifyIcon1.Visible = True
        Else
            ' Me.Show()
            Me.WindowState = FormWindowState.Normal
            Me.ShowInTaskbar = True
            NotifyIcon1.Visible = False
        End If
    End Sub

    Private Sub btnSaveSettings_Click(sender As Object, e As EventArgs) Handles btnSaveSettings.Click
        Dim sIPAddress As String = Me.nudIP1.Value.ToString & "." & Me.nudIP2.Value.ToString & "." & Me.nudIP3.Value.ToString & "." & Me.nudIP4.Value.ToString
        ReadSetting.Write("Interface Server", "IPAddress", sIPAddress)
        ReadSetting.Write("Interface Server", "Port", Me.nudPort.Value.ToString)

        MsgBox("OK!  Please Restart This Program Again.")
    End Sub

    Private Sub NotifyIcon1_MouseClick_1(sender As Object, e As MouseEventArgs) Handles NotifyIcon1.MouseClick
        Me.Show()
        Me.WindowState = FormWindowState.Normal
        Me.ShowInTaskbar = True
        NotifyIcon1.Visible = False
    End Sub

    Private Sub btnSystemSettings_Click_1(sender As Object, e As EventArgs) Handles btnSystemSettings.Click
        frmSettingsPassword.ShowDialog(Me)
    End Sub

    Private Sub Form1_Closed(sender As Object, e As EventArgs) Handles Me.Closed

        mWatchdogWatcher.KillWatchDog()
        mWatchdogWatcher = Nothing
        Process.GetCurrentProcess.Kill()
        Application.Exit()

    End Sub

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing

        If (MsgBox("Are Your Sure To Close This Program?  You Will Not Able To Print To Kitchen And Cashier Printer After This.",
                MsgBoxStyle.Question + MsgBoxStyle.YesNo, "IDBPOSIf") = MsgBoxResult.Yes) Then

            e.Cancel = False
        Else
            e.Cancel = True
        End If

    End Sub

    ' ========== MAIN LOOP (UNLIMITED RECONNECT) ==========
    Private Async Function ClientLoopAsync(url As String) As Task
        delayMs = initialDelayMs

        While reconnectEnabled

            'Do not check heartbeat when trying to reconnect.
            'tmWsHeartbeat.Enabled = False

            cts = New CancellationTokenSource()
            ws = New ClientWebSocket()

            Try
                Log("Websocket Url: " & url)
                Log("Connecting…")

                Await ws.ConnectAsync(New Uri(url), cts.Token)
                Log("Connected")
                lblWebsocketStatus.Text = "Online"
                lblWebsocketStatus.BackColor = Color.Lime

                'tmWsHeartbeat.Enabled = True

                delayMs = initialDelayMs ' reset backoff
                Await ReceiveLoopAsync()

            Catch ex As Exception

                Log("Loop error: " & ex.Message)
                lblWebsocketStatus.Text = "Offline"
                lblWebsocketStatus.BackColor = Color.Crimson

            Finally
                Try
                    If ws IsNot Nothing AndAlso ws.State <> WebSocketState.Closed Then
                        Dim T = ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "cleanup", CancellationToken.None)
                    End If
                Catch
                End Try
            End Try

            If reconnectEnabled Then
                Log($"Reconnecting in {delayMs} ms…")

                'Do not check heartbeat when trying to reconnect.
                'tmWsHeartbeat.Enabled = False

                Await Task.Delay(delayMs)
                delayMs = Math.Min(delayMs * 2, maxDelayMs)
            End If

        End While

        Log("Client loop stopped")

    End Function

    ' ========== RECEIVE LOOP ==========
    Private Async Function ReceiveLoopAsync() As Task
        Dim buffer = New Byte(4095) {}
        Dim sErrorMessage As String = ""

        Try
            While ws IsNot Nothing AndAlso ws.State = WebSocketState.Open
                Dim result = Await ws.ReceiveAsync(New ArraySegment(Of Byte)(buffer), cts.Token)

                If result.MessageType = WebSocketMessageType.Close Then Exit While

                Dim sIncomingData As String = Encoding.UTF8.GetString(buffer, 0, result.Count)
                Log("Incoming Data:" & sIncomingData)

#Region "Print Order Chit"

                If (sIncomingData.Contains("#PING#")) Then
                    mbReceivedPING = True

                ElseIf (sIncomingData.Contains("#DISCONNECT#")) Then

                    'cancel any pending ReceiveAsync
                    Try
                        If cts IsNot Nothing Then cts.Cancel()
                    Catch
                    End Try

                    ' break any in-flight socket ops immediately
                    Try
                        If ws IsNot Nothing Then ws.Abort()  ' immediate abort; no Await needed
                    Catch
                    End Try

                    lblWebsocketStatus.Text = "Offline"
                    lblWebsocketStatus.BackColor = Color.Crimson
                    Log("Disconnect the websocket")

                    delayMs = maxDelayMs

                ElseIf (sIncomingData.StartsWith("#PrintOrderChit#")) Then
                    sIncomingData = sIncomingData.Replace("#PrintOrderChit#", "")

                    Dim jArray As JArray = jArray.Parse(sIncomingData)

                    For Each jObj As JObject In jArray

                        Dim sPrinterIp As String = jObj.GetValue("printerip")
                        Dim sOrderChitId As String = jObj.GetValue("filename")
                        Dim sComCd As String = "TSKL"
                        Dim sProjCd As String = "TSKL"
                        If Not msServerUrl.EndsWith("/") Then msServerUrl = msServerUrl & "/"

                        Dim remoteUri As String = msServerUrl & "ws/pos/printOrderChit?filename=" & sOrderChitId & "&node=1"
                        ' "&ComCd=" & sComCd & "&ProjCd=" & sProjCd &


                        Dim sFileName As String = Application.StartupPath & "\" & sOrderChitId & ".pdf"

                        Try
                            Using client As New WebClient()
                                client.DownloadFile(remoteUri, sFileName)
                            End Using
                        Catch ex As Exception

                        End Try

                        If sPrinterIp = "PrintOrderChitForCashierPrinter" Then
                            sPrinterIp = msCashierPrinter
                        End If

                        sErrorMessage = ""
                        If fnCallGhostScript(sPrinterIp, sFileName, sErrorMessage) Then
                            jObj("status") = 1
                            sErrorMessage = "OK"
                        Else
                            jObj("status") = 0
                            sErrorMessage = "KO " + sErrorMessage
                        End If
                    Next

                End If

#End Region

            End While
        Catch ex As Exception
            Log("Receive error: " & ex.Message)
        Finally
            Log("Receive loop ended")
        End Try
    End Function

    Private Async Sub tmWsHeartbeat_Tick(sender As Object, e As EventArgs) Handles tmWsHeartbeat.Tick

        If (mbPrintServerUseWebsocket) Then

            Try

                'by right should received #PING# already
                If Not mbReceivedPING Then
                    Log("Heartbeat start to reconnect...")
                    'tmWsHeartbeat.Enabled = False

                    ' cancel any pending ReceiveAsync
                    Try
                        If cts IsNot Nothing Then cts.Cancel()
                    Catch
                    End Try

                    ' break any in-flight socket ops immediately
                    Try
                        If ws IsNot Nothing Then ws.Abort()  ' immediate abort; no Await needed
                    Catch
                    End Try

                    Return
                End If

                mbReceivedPING = False
                If ws Is Nothing OrElse ws.State <> WebSocketState.Open Then
                    Log("Heartbeat Websocket Not Opened!")
                    Return

                Else

                    Dim payload = Encoding.UTF8.GetBytes("#PING#")
                    Log("Send #PING#")
                    Await ws.SendAsync(New ArraySegment(Of Byte)(payload),
                                       WebSocketMessageType.Text,
                                       True,
                                       CancellationToken.None)
                End If

            Catch ex As Exception
                Log("Websocket Heartbeat Error!  " & ex.Message)
            End Try

        End If


    End Sub
End Class
