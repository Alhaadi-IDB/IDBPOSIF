<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
        Me.btnSystemSettings = New System.Windows.Forms.Button()
        Me.btnSaveSettings = New System.Windows.Forms.Button()
        Me.label2 = New System.Windows.Forms.Label()
        Me.label1 = New System.Windows.Forms.Label()
        Me.nudIP4 = New System.Windows.Forms.NumericUpDown()
        Me.nudIP3 = New System.Windows.Forms.NumericUpDown()
        Me.nudIP2 = New System.Windows.Forms.NumericUpDown()
        Me.nudIP1 = New System.Windows.Forms.NumericUpDown()
        Me.nudPort = New System.Windows.Forms.NumericUpDown()
        Me.NotifyIcon1 = New System.Windows.Forms.NotifyIcon(Me.components)
        Me.lblWebsocket = New System.Windows.Forms.Label()
        Me.lblWebsocketStatus = New System.Windows.Forms.Label()
        Me.tmWsHeartbeat = New System.Windows.Forms.Timer(Me.components)
        CType(Me.nudIP4, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudIP3, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudIP2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudIP1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudPort, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'btnSystemSettings
        '
        Me.btnSystemSettings.Location = New System.Drawing.Point(165, 79)
        Me.btnSystemSettings.Name = "btnSystemSettings"
        Me.btnSystemSettings.Size = New System.Drawing.Size(101, 27)
        Me.btnSystemSettings.TabIndex = 65
        Me.btnSystemSettings.Text = "System Settings"
        Me.btnSystemSettings.UseVisualStyleBackColor = True
        '
        'btnSaveSettings
        '
        Me.btnSaveSettings.Enabled = False
        Me.btnSaveSettings.Location = New System.Drawing.Point(272, 79)
        Me.btnSaveSettings.Name = "btnSaveSettings"
        Me.btnSaveSettings.Size = New System.Drawing.Size(101, 27)
        Me.btnSaveSettings.TabIndex = 64
        Me.btnSaveSettings.Text = "Save Settings"
        Me.btnSaveSettings.UseVisualStyleBackColor = True
        '
        'label2
        '
        Me.label2.AutoSize = True
        Me.label2.Location = New System.Drawing.Point(313, 9)
        Me.label2.Name = "label2"
        Me.label2.Size = New System.Drawing.Size(29, 13)
        Me.label2.TabIndex = 63
        Me.label2.Text = "Port:"
        '
        'label1
        '
        Me.label1.AutoSize = True
        Me.label1.Location = New System.Drawing.Point(12, 9)
        Me.label1.Name = "label1"
        Me.label1.Size = New System.Drawing.Size(105, 13)
        Me.label1.TabIndex = 62
        Me.label1.Text = "Interface IP address:"
        '
        'nudIP4
        '
        Me.nudIP4.Location = New System.Drawing.Point(238, 26)
        Me.nudIP4.Maximum = New Decimal(New Integer() {255, 0, 0, 0})
        Me.nudIP4.Name = "nudIP4"
        Me.nudIP4.Size = New System.Drawing.Size(67, 20)
        Me.nudIP4.TabIndex = 61
        '
        'nudIP3
        '
        Me.nudIP3.Location = New System.Drawing.Point(163, 26)
        Me.nudIP3.Maximum = New Decimal(New Integer() {255, 0, 0, 0})
        Me.nudIP3.Name = "nudIP3"
        Me.nudIP3.Size = New System.Drawing.Size(69, 20)
        Me.nudIP3.TabIndex = 60
        '
        'nudIP2
        '
        Me.nudIP2.Location = New System.Drawing.Point(88, 26)
        Me.nudIP2.Maximum = New Decimal(New Integer() {255, 0, 0, 0})
        Me.nudIP2.Name = "nudIP2"
        Me.nudIP2.Size = New System.Drawing.Size(69, 20)
        Me.nudIP2.TabIndex = 59
        '
        'nudIP1
        '
        Me.nudIP1.Location = New System.Drawing.Point(15, 26)
        Me.nudIP1.Maximum = New Decimal(New Integer() {255, 0, 0, 0})
        Me.nudIP1.Name = "nudIP1"
        Me.nudIP1.Size = New System.Drawing.Size(67, 20)
        Me.nudIP1.TabIndex = 58
        '
        'nudPort
        '
        Me.nudPort.Location = New System.Drawing.Point(316, 26)
        Me.nudPort.Maximum = New Decimal(New Integer() {65535, 0, 0, 0})
        Me.nudPort.Name = "nudPort"
        Me.nudPort.Size = New System.Drawing.Size(57, 20)
        Me.nudPort.TabIndex = 57
        '
        'NotifyIcon1
        '
        Me.NotifyIcon1.BalloonTipText = "IDBPOS Interface Server"
        Me.NotifyIcon1.Icon = CType(resources.GetObject("NotifyIcon1.Icon"), System.Drawing.Icon)
        Me.NotifyIcon1.Text = "IDBPOS Interface Server"
        Me.NotifyIcon1.Visible = True
        '
        'lblWebsocket
        '
        Me.lblWebsocket.AutoSize = True
        Me.lblWebsocket.Location = New System.Drawing.Point(12, 68)
        Me.lblWebsocket.Name = "lblWebsocket"
        Me.lblWebsocket.Size = New System.Drawing.Size(98, 13)
        Me.lblWebsocket.TabIndex = 66
        Me.lblWebsocket.Text = "Websocket Status:"
        '
        'lblWebsocketStatus
        '
        Me.lblWebsocketStatus.AutoSize = True
        Me.lblWebsocketStatus.BackColor = System.Drawing.Color.Lime
        Me.lblWebsocketStatus.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(254, Byte))
        Me.lblWebsocketStatus.Location = New System.Drawing.Point(11, 81)
        Me.lblWebsocketStatus.Name = "lblWebsocketStatus"
        Me.lblWebsocketStatus.Size = New System.Drawing.Size(112, 20)
        Me.lblWebsocketStatus.TabIndex = 67
        Me.lblWebsocketStatus.Text = "Online / Offline"
        '
        'tmWsHeartbeat
        '
        Me.tmWsHeartbeat.Interval = 10000
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.MediumTurquoise
        Me.ClientSize = New System.Drawing.Size(375, 118)
        Me.Controls.Add(Me.lblWebsocketStatus)
        Me.Controls.Add(Me.lblWebsocket)
        Me.Controls.Add(Me.btnSystemSettings)
        Me.Controls.Add(Me.btnSaveSettings)
        Me.Controls.Add(Me.label2)
        Me.Controls.Add(Me.label1)
        Me.Controls.Add(Me.nudIP4)
        Me.Controls.Add(Me.nudIP3)
        Me.Controls.Add(Me.nudIP2)
        Me.Controls.Add(Me.nudIP1)
        Me.Controls.Add(Me.nudPort)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "Form1"
        Me.Text = "IDBPOSIf V1.0.14"
        CType(Me.nudIP4, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudIP3, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudIP2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudIP1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudPort, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Private WithEvents btnSystemSettings As Button
    Private WithEvents btnSaveSettings As Button
    Private WithEvents label2 As Label
    Private WithEvents label1 As Label
    Private WithEvents nudIP4 As NumericUpDown
    Private WithEvents nudIP3 As NumericUpDown
    Private WithEvents nudIP2 As NumericUpDown
    Private WithEvents nudIP1 As NumericUpDown
    Private WithEvents nudPort As NumericUpDown
    Friend WithEvents NotifyIcon1 As NotifyIcon
    Private WithEvents lblWebsocket As Label
    Friend WithEvents lblWebsocketStatus As Label
    Friend WithEvents tmWsHeartbeat As Timer
End Class
