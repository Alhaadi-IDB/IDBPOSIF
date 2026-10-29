' Set Option Strict on as this really speeds
' Up performance in vb.net and helps you create
' good code
'Option Strict On

Imports System.Net.Sockets
Imports System.IO
Imports System.Text
Imports Newtonsoft.Json.Linq
Imports System.Net
Imports System.Web

Public Class Server

    Private sMyWebServerRoot As String
    Private mySocket As Socket
    Private _DefaultPage As String

    Public Sub New(ByVal s As Socket, ByVal Location As String, ByVal DefaultPage As String)
        mySocket = s
        If Microsoft.VisualBasic.Right(Location, 1) = "\" Then
            Location = Mid(Location, 1, Len(Location) - 1)

        End If
        sMyWebServerRoot = Location & "\"
        _DefaultPage = DefaultPage
    End Sub
    Public Function GetMimeType(ByVal filename As String) As String
        Dim ext As String
        If InStr(filename, ".") > 0 And InStr(filename, ".") < Len(filename) Then
            ext = Mid(filename, InStrRev(filename, ".") + 1)
        Else
            ext = ""
        End If
        ext = LCase(ext)
        Select Case ext
            Case Is = "htm"
                Return "text/html"
            Case Is = "js"
                Return "text/html"
            Case Is = "html"
                Return "text/html"
            Case Is = "txt"
                Return "text/html"
            Case Is = "jpg"
                Return "image/jpeg"
            Case Is = "jpeg"
                Return "image/jpeg"
            Case Is = "gif"
                Return "image/gif"
            Case Else
                Return ""
        End Select
    End Function

    Public Sub HandleConnection()
        Dim iStartPos As Integer = 0
        Dim sErrorMessage As String
        Dim sReturnData As String = ""

        Dim sPhysicalFilePath As String = ""
        Dim bReceive As Byte()
        ReDim bReceive(2024)
        'Receive the request into a byte array
        Dim i As Integer = mySocket.Receive(bReceive, bReceive.Length, 0)
        ' Convert the byte array to a string
        Dim sbuffer As String = Encoding.ASCII.GetString(bReceive).Trim

        iStartPos = sbuffer.IndexOf("HTTP", 1)

        If (iStartPos = -1) Then
            mySocket.Close()
            Return
        End If

        '  Get the HTTP text and version e.g. it will return "HTTP/1.1"
        Dim sHttpVersion As String = sbuffer.Substring(iStartPos, 8)

        If (sbuffer.Contains("Access-Control-Request-Private-Network: true")) Then
            SendHeaderForAccessControlRequestPrivateNetwork(sHttpVersion, "text/plain;charset=UTF-8", sReturnData.Length, " 200 OK")
            SendToBrowser(sReturnData)
            Exit Sub
        End If

        Try
            ' the web server only accepts get requests.
            If Mid(LCase(sbuffer), 1, 3) <> "get" Then
                'f not GET request then close socket and exit
                mySocket.Close()
                Return
            End If

            Dim iStartAction As Integer = sbuffer.IndexOf("ActionType")
            If (iStartAction > -1) Then
                Dim iEndAction As Integer = sbuffer.IndexOf("&", iStartAction + 1)
                msActionType = sbuffer.Substring(iStartAction, iEndAction - iStartAction)
                msActionType = msActionType.Replace("ActionType=", "")

                Dim sData As String = sbuffer.Substring(iEndAction + 1, sbuffer.IndexOf(sHttpVersion) - iEndAction - 1)
                sData = sData.Replace(sHttpVersion, "").Replace(vbCrLf, "").Replace(vbCr, "").Trim()

                Dim sBillId As String = ""
                Dim sOutletId As String = ""
                Dim sShiftId As String = ""
                Dim sPrinterIp As String = ""
                Dim sOrderChitId As String = ""
                Dim sComCd As String = ""
                Dim sProjCd As String = ""
                Dim sUserId As String = ""
                Dim sMode As String = ""
                Dim sProductCategory As String = ""
                Dim sLanguageId As String = ""
                Dim sBillOrderId As String = ""
                Dim bGroup = ""

                Dim jObj = New JObject()

                If (msActionType = "PrintReceipt") Then
                    '1&1000&PANI&PANI&Cashier&false
                    Dim aData As String() = sData.Split("&")
                    sOutletId = HttpUtility.UrlDecode(aData(0))
                    sBillId = HttpUtility.UrlDecode(aData(1))
                    sComCd = HttpUtility.UrlDecode(aData(2))
                    sProjCd = HttpUtility.UrlDecode(aData(3))
                    sUserId = HttpUtility.UrlDecode(aData(4))
                    bGroup = HttpUtility.UrlDecode(aData(5))

                    Dim remoteUri As String = msServerUrl & "/ws/pos/printReceipt?outletId=" & sOutletId &
                        "&selected=" & sBillId & "&ComCd=" & sComCd & "&ProjCd=" & sProjCd & "&UserId=" & sUserId &
                        "&tenantId=" & sProjCd & "&bGroup=" & bGroup
                    ' Log(remoteUri)

                    Dim sFileName As String = Application.StartupPath & "\" & sBillId & "_" & "receipt.pdf"
                    'Log(sFileName)

                    Try
                        Using client As New WebClient()
                            client.DownloadFile(remoteUri, sFileName)
                        End Using
                    Catch ex As Exception
                        sErrorMessage = ex.Message
                        jObj("status") = 0
                        jObj("responseMsg") = sErrorMessage.ToString
                        sData = "fnPrintReceiptIDBPOSIfReturn(" & jObj.ToString & ")"
                        SendHeader(sHttpVersion, "text/plain;charset=UTF-8", sData.Length, " 200 OK")
                        SendToBrowser(sData)
                        mySocket.Close()
                        Return
                    End Try

                    sErrorMessage = ""
                    If fnCallGhostScript(msCashierPrinter, sFileName, sErrorMessage) Then
                        jObj("status") = 1
                        sErrorMessage = "OK"
                    Else
                        jObj("status") = 0
                        sErrorMessage = "KO " + sErrorMessage
                    End If

                    jObj("responseMsg") = sErrorMessage.ToString
                    sData = "fnPrintReceiptIDBPOSIfReturn(" & jObj.ToString & ")"
                    SendHeader(sHttpVersion, "text/plain;charset=UTF-8", sData.Length, " 200 OK")
                    SendToBrowser(sData)
                    mySocket.Close()
                    Return

                ElseIf (msActionType = "PrintOrderChit") Then

                    Dim aData As String() = sData.Split("&")

                    sPrinterIp = HttpUtility.UrlDecode(aData(0))
                    sOrderChitId = HttpUtility.UrlDecode(aData(1))
                    sComCd = HttpUtility.UrlDecode(aData(2))
                    sProjCd = HttpUtility.UrlDecode(aData(3))

                    Dim remoteUri As String = msServerUrl & "/ws/pos/printOrderChit?filename=" & sOrderChitId & "&ComCd=" & sComCd & "&ProjCd=" & sProjCd &
                                                 "&tenantId=" & sProjCd
                    Dim sFileName As String = Application.StartupPath & "\" & sOrderChitId & ".pdf"

                    Try
                        Using client As New WebClient()
                            client.DownloadFile(remoteUri, sFileName)
                        End Using
                    Catch ex As Exception
                        sErrorMessage = "KO " & ex.Message
                        jObj("status") = 0
                        jObj("responseMsg") = sErrorMessage.ToString
                        sData = "fnOrderChitIDBPOSIfReturn(" & jObj.ToString & ")"
                        SendHeader(sHttpVersion, "text/plain;charset=UTF-8", sData.Length, " 200 OK")
                        SendToBrowser(sData)
                        mySocket.Close()
                        Return
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

                    jObj("responseMsg") = sErrorMessage.ToString
                    sData = "fnOrderChitIDBPOSIfReturn(" & jObj.ToString & ")"
                    SendHeader(sHttpVersion, "text/plain;charset=UTF-8", sData.Length, " 200 OK")
                    SendToBrowser(sData)
                    mySocket.Close()
                    Return

                ElseIf (msActionType = "PrintDetails") Then
                    'PANI&PANI&admin&3020&6&ShiftEnd
                    Dim aData As String() = sData.Split("&")
                    sComCd = HttpUtility.UrlDecode(aData(0))
                    sProjCd = HttpUtility.UrlDecode(aData(1))
                    sUserId = HttpUtility.UrlDecode(aData(2))
                    sShiftId = HttpUtility.UrlDecode(aData(3))
                    sOutletId = HttpUtility.UrlDecode(aData(4))
                    sMode = HttpUtility.UrlDecode(aData(5))

                    Dim remoteUri As String = msServerUrl & "/ws/pos/printReportToCashierPrinter/" & msActionType & "?ComCd=" & sComCd &
                        "&ProjCd=" & sProjCd & "&UserId=" & sUserId & "&shiftId=" & sShiftId & "&outletId=" & sOutletId & "&mode=" & sMode &
                        "&tenantId=" & sProjCd
                    Dim sFileName As String = Application.StartupPath & "\" & sShiftId & "_" & sMode & ".pdf"

                    Try
                        Using client As New WebClient()
                            client.DownloadFile(remoteUri, sFileName)
                        End Using
                    Catch ex As Exception
                        sErrorMessage = ex.Message
                        jObj("status") = 0
                        jObj("responseMsg") = sErrorMessage.ToString
                        sData = "fnPrintShiftIDBPOSIfReturn(" & jObj.ToString & ")"
                        SendHeader(sHttpVersion, "text/plain;charset=UTF-8", sData.Length, " 200 OK")
                        SendToBrowser(sData)
                        mySocket.Close()
                        Return
                    End Try

                    sErrorMessage = ""
                    If fnCallGhostScript(msCashierPrinter, sFileName, sErrorMessage) Then
                        jObj("status") = 1
                        sErrorMessage = "OK"
                    Else
                        jObj("status") = 0
                        sErrorMessage = "KO"
                    End If

                    jObj("responseMsg") = sErrorMessage.ToString
                    sData = "fnPrintShiftIDBPOSIfReturn(" & jObj.ToString & ")"
                    SendHeader(sHttpVersion, "text/plain;charset=UTF-8", sData.Length, " 200 OK")
                    SendToBrowser(sData)
                    mySocket.Close()
                    Return

                ElseIf (msActionType = "PrintCategory") Then
                    'PANI&PANI&admin&3035&2&ShiftEnd&23
                    Dim aData As String() = sData.Split("&")
                    sComCd = HttpUtility.UrlDecode(aData(0))
                    sProjCd = HttpUtility.UrlDecode(aData(1))
                    sUserId = HttpUtility.UrlDecode(aData(2))
                    sShiftId = HttpUtility.UrlDecode(aData(3))
                    sOutletId = HttpUtility.UrlDecode(aData(4))
                    sMode = HttpUtility.UrlDecode(aData(5))
                    sProductCategory = HttpUtility.UrlDecode(aData(6))

                    Dim remoteUri As String = msServerUrl & "/ws/pos/printReportToCashierPrinter/" & msActionType & "?ComCd=" & sComCd &
                        "&ProjCd=" & sProjCd & "&UserId=" & sUserId & "&shiftId=" & sShiftId & "&outletId=" & sOutletId & "&mode=" & sMode &
                        "&ProductCategory=" & sProductCategory & "&tenantId=" & sProjCd

                    Dim sFileName As String = Application.StartupPath & "\" & sShiftId & "_" & sProductCategory & "_" & sMode & ".pdf"

                    Try
                        Using client As New WebClient()
                            client.DownloadFile(remoteUri, sFileName)
                        End Using
                    Catch ex As Exception
                        sErrorMessage = ex.Message
                        jObj("status") = 0
                        jObj("responseMsg") = sErrorMessage.ToString
                        sData = "fnPrintCategoryIDBPOSIfReturn(" & jObj.ToString & ")"
                        SendHeader(sHttpVersion, "text/plain;charset=UTF-8", sData.Length, " 200 OK")
                        SendToBrowser(sData)
                        mySocket.Close()
                        Return
                    End Try

                    sErrorMessage = ""
                    If fnCallGhostScript(msCashierPrinter, sFileName, sErrorMessage) Then
                        jObj("status") = 1
                        sErrorMessage = "OK"
                    Else
                        jObj("status") = 0
                        sErrorMessage = "KO"
                    End If

                    jObj("responseMsg") = sErrorMessage.ToString
                    sData = "fnPrintCategoryIDBPOSIfReturn(" & jObj.ToString & ")"
                    SendHeader(sHttpVersion, "text/plain;charset=UTF-8", sData.Length, " 200 OK")
                    SendToBrowser(sData)
                    mySocket.Close()
                    Return

                ElseIf (msActionType = "PrintKitchenDisplay") Then
                    'PANI&PANI&1&2551,2552,2553
                    Dim aData As String() = sData.Split("&")
                    sComCd = HttpUtility.UrlDecode(aData(0))
                    sProjCd = HttpUtility.UrlDecode(aData(1))
                    sLanguageId = HttpUtility.UrlDecode(aData(2))
                    sBillOrderId = HttpUtility.UrlDecode(aData(3))

                    Dim remoteUri As String = msServerUrl & "/ws/kitchenOrder/generateOrderChit?ComCd=" & sComCd &
                        "&ProjCd=" & sProjCd & "&languageId=" & sLanguageId & "&billOrderId=" & sBillOrderId & "&tenantId=" & sProjCd

                    Dim sFileName As String = Application.StartupPath & "\orderChit_" & TimeOfDay.ToString("hhmmss") & ".pdf"

                    Try
                        Using client As New WebClient()
                            client.DownloadFile(remoteUri, sFileName)
                        End Using
                    Catch ex As Exception
                        sErrorMessage = ex.Message
                        jObj("status") = 0
                        jObj("responseMsg") = sErrorMessage.ToString
                        sData = "fnPrintKitchenDisplayIDBPOSIfReturn(" & jObj.ToString & ")"
                        SendHeader(sHttpVersion, "text/plain;charset=UTF-8", sData.Length, " 200 OK")
                        SendToBrowser(sData)
                        mySocket.Close()
                        Return
                    End Try

                    sErrorMessage = ""
                    If fnCallGhostScript(msCashierPrinter, sFileName, sErrorMessage) Then
                        jObj("status") = 1
                        sErrorMessage = "OK"
                    Else
                        jObj("status") = 0
                        sErrorMessage = "KO"
                    End If


                    jObj("responseMsg") = sErrorMessage.ToString
                    sData = "fnPrintKitchenDisplayIDBPOSIfReturn(" & jObj.ToString & ")"
                    SendHeader(sHttpVersion, "text/plain;charset=UTF-8", sData.Length, " 200 OK")
                    SendToBrowser(sData)
                    mySocket.Close()
                    Return

                ElseIf (msActionType = "PrintQROrder") Then
                    'PANI&PANI&admin&2551
                    Dim aData As String() = sData.Split("&")
                    sComCd = HttpUtility.UrlDecode(aData(0))
                    sProjCd = HttpUtility.UrlDecode(aData(1))
                    sUserId = HttpUtility.UrlDecode(aData(2))
                    sBillId = HttpUtility.UrlDecode(aData(3))

                    Dim remoteUri As String = msServerUrl & "/ws/pos/PrintQROrder?ComCd=" & sComCd &
                        "&ProjCd=" & sProjCd & "&UserId=" & sUserId & "&BillId=" & sBillId & "&tenantId=" & sProjCd

                    Dim sFileName As String = Application.StartupPath & "\qrOrder_" & TimeOfDay.ToString("hhmmss") & ".pdf"

                    Try
                        Using client As New WebClient()
                            client.DownloadFile(remoteUri, sFileName)
                        End Using
                    Catch ex As Exception
                        sErrorMessage = ex.Message
                        jObj("status") = 0
                        jObj("responseMsg") = sErrorMessage.ToString
                        sData = "fnPrintQROrderIDBPOSIfReturn(" & jObj.ToString & ")"
                        SendHeader(sHttpVersion, "text/plain;charset=UTF-8", sData.Length, " 200 OK")
                        SendToBrowser(sData)
                        mySocket.Close()
                        Return
                    End Try

                    sErrorMessage = ""
                    If fnCallGhostScript(msCashierPrinter, sFileName, sErrorMessage) Then
                        jObj("status") = 1
                        sErrorMessage = "OK"
                    Else
                        jObj("status") = 0
                        sErrorMessage = "KO"
                    End If


                    jObj("responseMsg") = sErrorMessage.ToString
                    sData = "fnPrintQROrderIDBPOSIfReturn(" & jObj.ToString & ")"
                    SendHeader(sHttpVersion, "text/plain;charset=UTF-8", sData.Length, " 200 OK")
                    SendToBrowser(sData)
                    mySocket.Close()
                    Return

                ElseIf (msActionType = "GenerateAndPrintOrderChit") Then
                    'PANI&PANI&???
                    Dim aData As String() = sData.Split("&")
                    sComCd = HttpUtility.UrlDecode(aData(0))
                    sProjCd = HttpUtility.UrlDecode(aData(1))
                    sBillOrderId = HttpUtility.UrlDecode(aData(2))

                    Dim remoteUri As String = msServerUrl & "ws/pos/ols/fnGenerateOrderChitForOLS?ComCd=" & sComCd &
                       "&ProjCd=" & sProjCd & "&jaBillOrderId=" & sBillOrderId & "&tenantId=" & sProjCd

                    Dim sDownloadedData As String = ""

                    Try
                        Using client As New WebClient()
                            sDownloadedData = client.DownloadString(remoteUri)
                        End Using
                    Catch ex As Exception
                        sErrorMessage = ex.Message
                        jObj("status") = 0
                        jObj("responseMsg") = sErrorMessage.ToString
                        sData = "fnPrintQROrderIDBPOSIfReturn(" & jObj.ToString & ")"
                        SendHeader(sHttpVersion, "text/plain;charset=UTF-8", sData.Length, " 200 OK")
                        SendToBrowser(sData)
                        mySocket.Close()
                        Return
                    End Try

                    'sErrorMessage = ""
                    'If fnCallGhostScript(msCashierPrinter, sFileName, sErrorMessage) Then
                    'jObj("status") = 1
                    'sErrorMessage = "OK"
                    'Else
                    'jObj("status") = 0
                    'sErrorMessage = "KO"
                    'End If

                    jObj("responseMsg") = sErrorMessage.ToString
                    sData = "fnPrintQROrderIDBPOSIfReturn(" & jObj.ToString & ")"
                    SendHeader(sHttpVersion, "text/plain;charset=UTF-8", sData.Length, " 200 OK")
                    SendToBrowser(sData)
                    mySocket.Close()
                    Return

                ElseIf (msActionType = "PrintTest") Then
                    'PANI&PANI&admin
                    Dim aData As String() = sData.Split("&")
                    sComCd = HttpUtility.UrlDecode(aData(0))
                    sProjCd = HttpUtility.UrlDecode(aData(1))
                    sUserId = HttpUtility.UrlDecode(aData(2))

                    Dim remoteUri As String = msServerUrl & "/ws/pos/" & msActionType & "?ComCd=" & sComCd &
                        "&ProjCd=" & sProjCd & "&UserId=" & sUserId & "&tenantId=" & sProjCd

                    Dim sFileName As String = Application.StartupPath & "\TestPrint.pdf"

                    Try
                        Using client As New WebClient()
                            client.DownloadFile(remoteUri, sFileName)
                        End Using
                    Catch ex As Exception
                        sErrorMessage = ex.Message
                        jObj("status") = 0
                        jObj("responseMsg") = sErrorMessage.ToString
                        sData = "fnPrintTestIDBPOSIfReturn(" & jObj.ToString & ")"
                        SendHeader(sHttpVersion, "text/plain;charset=UTF-8", sData.Length, " 200 OK")
                        SendToBrowser(sData)
                        mySocket.Close()
                        Return
                    End Try

                    sErrorMessage = ""
                    If fnCallGhostScript(msCashierPrinter, sFileName, sErrorMessage) Then
                        jObj("status") = 1
                        sErrorMessage = "OK"
                    Else
                        jObj("status") = 0
                        sErrorMessage = "KO"
                    End If


                    jObj("responseMsg") = sErrorMessage.ToString
                    sData = "fnPrintTestIDBPOSIfReturn(" & jObj.ToString & ")"
                    SendHeader(sHttpVersion, "text/plain;charset=UTF-8", sData.Length, " 200 OK")
                    SendToBrowser(sData)
                    mySocket.Close()
                    Return

                ElseIf (msActionType = "KickDrawer") Then
                    sErrorMessage = ""

                    If fnKickDrawer("", sErrorMessage) Then
                        jObj("status") = 1
                        sErrorMessage = "OK"
                    Else
                        jObj("status") = 0
                        sErrorMessage = "KO " + sErrorMessage
                    End If

                    jObj("responseMsg") = sErrorMessage.ToString
                    sData = "fnKickDrawerIDBPOSIfReturn(" & jObj.ToString & ")"
                    SendHeader(sHttpVersion, "text/plain;charset=UTF-8", sData.Length, " 200 OK")
                    SendToBrowser(sData)
                    mySocket.Close()


                Else

                    SendHeader(sHttpVersion, "text/plain;charset=UTF-8", sReturnData.Length, " 200 OK")
                    SendToBrowser(sReturnData)
                    mySocket.Close()
                    Return
                End If



            Else
                sErrorMessage = "<H2>IDBPOSIF Server<BR>"
                SendHeader(sHttpVersion, "", sErrorMessage.Length, " 404 Not Found")
                SendToBrowser(sErrorMessage)
                mySocket.Close()
                Return

            End If

        Catch ex As Exception
            sErrorMessage = "<H2>IDBPOSIF Server<BR>"
            sErrorMessage = sErrorMessage & "<BR>No default page was found."
            sErrorMessage = sErrorMessage & "<BR><BR>Please contact vendor</H2>"
            sErrorMessage = sErrorMessage & "<BR>" & ex.Message
            SendHeader(sHttpVersion, "", sErrorMessage.Length, " 404 Not Found")
            SendToBrowser(sErrorMessage)
            mySocket.Close()
            Return
        End Try

    End Sub

    Private Sub SendHeader(ByVal sHttpVersion As String, ByVal sMIMEHeader As String, ByVal iTotBytes As Integer, ByVal sStatusCode As String)

        Dim sBuffer As String = ""

        ' if Mime type is not provided set default to text/html
        If (sMIMEHeader.Length = 0) Then sMIMEHeader = "text/html" ' // Default Mime Type is text/html

        sBuffer = sBuffer & sHttpVersion & sStatusCode & vbNewLine
        sBuffer = sBuffer & "Server: IDBPOSIf WebServer" & vbNewLine
        sBuffer = sBuffer & "Content-Type: " & sMIMEHeader & vbNewLine
        sBuffer = sBuffer & "Accept-Ranges: bytes" & vbNewLine
        sBuffer = sBuffer & "Content-Length: " & iTotBytes & vbNewLine & vbNewLine
        Dim bSendData As Byte() = Encoding.ASCII.GetBytes(sBuffer)
        SendToBrowser(bSendData)

    End Sub

    Private Sub SendToBrowser(ByVal sData As String)
        SendToBrowser(Encoding.ASCII.GetBytes(sData))
    End Sub


    Private Sub SendToBrowser(ByVal bSendData As Byte())

        If (mySocket.Connected) Then
            Dim numbytes As Integer
            numbytes = mySocket.Send(bSendData, bSendData.Length, 0)
            If numbytes = -1 Then

            Else

            End If

        Else

        End If

    End Sub


    Private Sub SendHeaderForAccessControlRequestPrivateNetwork(ByVal sHttpVersion As String, ByVal sMIMEHeader As String, ByVal iTotBytes As Integer, ByVal sStatusCode As String)

        Dim sBuffer As String = ""

        ' if Mime type is not provided set default to text/html
        If (sMIMEHeader.Length = 0) Then sMIMEHeader = "text/html" ' // Default Mime Type is text/html

        sBuffer = sBuffer & sHttpVersion & sStatusCode & vbNewLine
        sBuffer = sBuffer & "Access-Control-Request-Private-Network: true" & vbNewLine
        sBuffer = sBuffer & "Access-Control-Allow-Origin: *" & vbNewLine
        sBuffer = sBuffer & "Access-Control-Allow-Methods: GET" & vbNewLine
        sBuffer = sBuffer & "Access-Control-Max-Age: 10" & vbNewLine
        sBuffer = sBuffer & "Server: VB.net WebServer" & vbNewLine
        sBuffer = sBuffer & "Content-Type: " & sMIMEHeader & vbNewLine
        sBuffer = sBuffer & "Accept-Ranges: bytes" & vbNewLine
        sBuffer = sBuffer & "Content-Length: " & iTotBytes & vbNewLine & vbNewLine

        Dim bSendData As Byte() = Encoding.ASCII.GetBytes(sBuffer)
        SendToBrowser(bSendData)

    End Sub

End Class
