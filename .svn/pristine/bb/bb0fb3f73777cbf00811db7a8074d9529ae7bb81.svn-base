Imports System.Net
Imports System.Net.Sockets
Imports System.Net.WebSockets
Imports System.Text
Imports System.Threading
Imports System.Threading.Tasks
Imports System.Web
Imports System.Windows.Forms.AxHost
Imports Newtonsoft.Json.Linq

Public Class frmWebsocketTest

    Private ws As ClientWebSocket
    Private cts As CancellationTokenSource
    Private clientLoopTask As Task

    Private reconnectEnabled As Boolean = False
    Private initialDelayMs As Integer = 1000
    Private maxDelayMs As Integer = 30000

    ' ========== BUTTON: CONNECT ==========
    Private Sub btnConnect_Click(sender As Object, e As EventArgs) Handles btnConnect.Click
        reconnectEnabled = True
        btnConnect.Enabled = False
        clientLoopTask = ClientLoopAsync(txtUrl.Text) ' fire-and-forget
    End Sub

    ' ========== BUTTON: SEND ==========
    Private Async Sub btnSend_Click(sender As Object, e As EventArgs) Handles btnSend.Click
        Try
            If ws Is Nothing OrElse ws.State <> WebSocketState.Open Then
                AppendLog("Socket not open")
                Return
            End If
            Dim payload = Encoding.UTF8.GetBytes(txtMessage.Text)
            Await ws.SendAsync(New ArraySegment(Of Byte)(payload),
                               WebSocketMessageType.Text,
                               True,
                               CancellationToken.None)
            AppendLog("→ " & txtMessage.Text)
            txtMessage.Clear()
        Catch ex As Exception
            AppendLog("Send error: " & ex.Message)
        End Try
    End Sub

    ' ========== BUTTON: CLOSE ==========
    Private Async Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        reconnectEnabled = False ' stop retrying
        AppendLog("Manual close requested")

        Try
            If ws IsNot Nothing AndAlso ws.State = WebSocketState.Open Then
                Await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "client bye", CancellationToken.None)
            End If
        Catch
        End Try

        Try
            If cts IsNot Nothing Then cts.Cancel()
        Catch
        End Try

        UIReset()
        btnConnect.Enabled = True
    End Sub

    ' ========== MAIN LOOP (UNLIMITED RECONNECT) ==========
    Private Async Function ClientLoopAsync(url As String) As Task
        Dim delayMs = initialDelayMs

        While reconnectEnabled
            cts = New CancellationTokenSource()
            ws = New ClientWebSocket()

            Try
                AppendLog("Connecting…")
                Await ws.ConnectAsync(New Uri(url), cts.Token)
                AppendLog("Connected")

                Me.BeginInvoke(Sub()
                                   btnSend.Enabled = True
                                   btnClose.Enabled = True
                               End Sub)

                delayMs = initialDelayMs ' reset backoff
                Await ReceiveLoopAsync()

            Catch ex As Exception
                AppendLog("Loop error: " & ex.Message)

            Finally
                Try
                    If ws IsNot Nothing AndAlso ws.State <> WebSocketState.Closed Then
                        Dim T = ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "cleanup", CancellationToken.None)
                    End If
                Catch
                End Try
                UIReset()
            End Try

            If reconnectEnabled Then
                AppendLog($"Reconnecting in {delayMs} ms…")
                Await Task.Delay(delayMs)
                delayMs = Math.Min(delayMs * 2, maxDelayMs)
            End If
        End While

        AppendLog("Client loop stopped")
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
                AppendLog("← " & sIncomingData)

#Region "Print Order Chit"

                If (sIncomingData.StartsWith("#PrintOrderChit#")) Then
                    sIncomingData = sIncomingData.Replace("#PrintOrderChit#", "")

                    Dim jArray As JArray = JArray.Parse(sIncomingData)

                    For Each jObj As JObject In jArray

                        Dim sPrinterIp As String = jObj.GetValue("printerip")
                        Dim sOrderChitId As String = jObj.GetValue("filename")
                        Dim sComCd As String = "TSKL"
                        Dim sProjCd As String = "TSKL"
                        If Not msServerUrl.EndsWith("/") Then msServerUrl = msServerUrl & "/"
                        AppendLog(sPrinterIp & "  " & sOrderChitId)
                        Dim remoteUri As String = msServerUrl & "ws/pos/printOrderChit?filename=" & sOrderChitId & "&ComCd=" & sComCd & "&ProjCd=" & sProjCd &
                                                         "&tenantId=" & sProjCd

                        Dim sFileName As String = Application.StartupPath & "\" & sOrderChitId & ".pdf"
                        AppendLog(remoteUri)
                        AppendLog(sFileName)

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
            AppendLog("Receive error: " & ex.Message)
        Finally
            AppendLog("Receive loop ended")
        End Try
    End Function


    ' ========== HELPERS ==========
    Private Sub UIReset()
        If Me.IsHandleCreated Then
            Me.BeginInvoke(Sub()
                               btnSend.Enabled = False
                               btnClose.Enabled = False
                               btnConnect.Enabled = True
                           End Sub)
        End If
    End Sub

    Private Sub AppendLog(msg As String)
        If lstLog.InvokeRequired Then
            lstLog.BeginInvoke(Sub() lstLog.Items.Add($"{DateTime.Now:HH:mm:ss}  {msg}"))
        Else
            lstLog.Items.Add($"{DateTime.Now:HH:mm:ss}  {msg}")
        End If
    End Sub

    Protected Overrides Sub OnFormClosing(e As FormClosingEventArgs)
        reconnectEnabled = False
        Try
            If cts IsNot Nothing Then cts.Cancel()
        Catch
        End Try
        MyBase.OnFormClosing(e)
    End Sub

    Private Sub btnClearLog_Click(sender As Object, e As EventArgs) Handles btnClearLog.Click
        lstLog.Clear()
    End Sub

    Private Sub frmWebsocketTest_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub
End Class