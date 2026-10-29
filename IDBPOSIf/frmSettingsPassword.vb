Public Class frmSettingsPassword
    Private Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOK.Click
        Dim sPassword As String = Now.Year + Now.Month + Now.Day

        If (sPassword = Me.txtPassword.Text) Then
            Me.Close()
            Form1.EnableSaveSettingsbutton()
            frmSettings.ShowDialog(Me)
        Else
            MsgBox("Password Error!  Please Try Again.")
        End If
    End Sub
End Class