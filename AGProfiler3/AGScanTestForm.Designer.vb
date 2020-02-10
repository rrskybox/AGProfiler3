<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class AGScanTestForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(AGScanTestForm))
        Me.AGProfileData = New System.Windows.Forms.TextBox()
        Me.SaveButton = New System.Windows.Forms.Button()
        Me.CloseButton = New System.Windows.Forms.Button()
        Me.StopButton = New System.Windows.Forms.Button()
        Me.SaveFolderDialog = New System.Windows.Forms.FolderBrowserDialog()
        Me.StartButton = New System.Windows.Forms.Button()
        Me.AnalysisTextBox = New System.Windows.Forms.TextBox()
        Me.BestTextBox = New System.Windows.Forms.TextBox()
        Me.SetSampleCount = New System.Windows.Forms.NumericUpDown()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.AGPToolTips = New System.Windows.Forms.ToolTip(Me.components)
        Me.SetMinAggressiveness = New System.Windows.Forms.NumericUpDown()
        Me.SetMinMinMove = New System.Windows.Forms.NumericUpDown()
        Me.SetMaxMInMove = New System.Windows.Forms.NumericUpDown()
        Me.SetMaxAggressiveness = New System.Windows.Forms.NumericUpDown()
        Me.SetMaxDelay = New System.Windows.Forms.NumericUpDown()
        Me.SetMinDelay = New System.Windows.Forms.NumericUpDown()
        Me.ScanExposureSet = New System.Windows.Forms.NumericUpDown()
        Me.NumericUpDown1 = New System.Windows.Forms.NumericUpDown()
        Me.NumericUpDown2 = New System.Windows.Forms.NumericUpDown()
        Me.SetExposureButton = New System.Windows.Forms.Button()
        Me.FindStarButton = New System.Windows.Forms.Button()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.Label5 = New System.Windows.Forms.Label()
        CType(Me.SetSampleCount, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.SetMinAggressiveness, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.SetMinMinMove, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.SetMaxMInMove, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.SetMaxAggressiveness, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.SetMaxDelay, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.SetMinDelay, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ScanExposureSet, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.NumericUpDown1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.NumericUpDown2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.SuspendLayout()
        '
        'AGProfileData
        '
        Me.AGProfileData.Enabled = False
        Me.AGProfileData.Location = New System.Drawing.Point(13, 145)
        Me.AGProfileData.Multiline = True
        Me.AGProfileData.Name = "AGProfileData"
        Me.AGProfileData.Size = New System.Drawing.Size(632, 205)
        Me.AGProfileData.TabIndex = 0
        '
        'SaveButton
        '
        Me.SaveButton.Location = New System.Drawing.Point(489, 455)
        Me.SaveButton.Name = "SaveButton"
        Me.SaveButton.Size = New System.Drawing.Size(75, 33)
        Me.SaveButton.TabIndex = 3
        Me.SaveButton.Text = "Save"
        Me.AGPToolTips.SetToolTip(Me.SaveButton, "Saves the sampling data to a text file named" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "AGScanProfiler.txt in a user specif" & _
        "ied folder." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Uploads scan test results to Adaptive Profiler." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10))
        Me.SaveButton.UseVisualStyleBackColor = True
        '
        'CloseButton
        '
        Me.CloseButton.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.CloseButton.Location = New System.Drawing.Point(570, 455)
        Me.CloseButton.Name = "CloseButton"
        Me.CloseButton.Size = New System.Drawing.Size(75, 33)
        Me.CloseButton.TabIndex = 4
        Me.CloseButton.Text = "Done"
        Me.AGPToolTips.SetToolTip(Me.CloseButton, "Closes the Scan Profiler.  Results are uploaded" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "to the Adaptive Profiler.")
        Me.CloseButton.UseVisualStyleBackColor = True
        '
        'StopButton
        '
        Me.StopButton.Location = New System.Drawing.Point(323, 455)
        Me.StopButton.Name = "StopButton"
        Me.StopButton.Size = New System.Drawing.Size(75, 33)
        Me.StopButton.TabIndex = 2
        Me.StopButton.Text = "Stop"
        Me.AGPToolTips.SetToolTip(Me.StopButton, "Halts the scan.")
        Me.StopButton.UseVisualStyleBackColor = True
        '
        'StartButton
        '
        Me.StartButton.Location = New System.Drawing.Point(242, 455)
        Me.StartButton.Name = "StartButton"
        Me.StartButton.Size = New System.Drawing.Size(75, 33)
        Me.StartButton.TabIndex = 1
        Me.StartButton.Text = "Start Scan"
        Me.AGPToolTips.SetToolTip(Me.StartButton, "Initiates profile scanning of the autoguider settings." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "A guide star must be firs" & _
        "t set by ""Find Star"" or TSX to" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "run correctly." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10))
        Me.StartButton.UseVisualStyleBackColor = True
        '
        'AnalysisTextBox
        '
        Me.AnalysisTextBox.Enabled = False
        Me.AnalysisTextBox.Location = New System.Drawing.Point(12, 400)
        Me.AnalysisTextBox.Multiline = True
        Me.AnalysisTextBox.Name = "AnalysisTextBox"
        Me.AnalysisTextBox.Size = New System.Drawing.Size(632, 38)
        Me.AnalysisTextBox.TabIndex = 5
        '
        'BestTextBox
        '
        Me.BestTextBox.Enabled = False
        Me.BestTextBox.Location = New System.Drawing.Point(12, 356)
        Me.BestTextBox.Multiline = True
        Me.BestTextBox.Name = "BestTextBox"
        Me.BestTextBox.Size = New System.Drawing.Size(632, 38)
        Me.BestTextBox.TabIndex = 6
        '
        'SetSampleCount
        '
        Me.SetSampleCount.Location = New System.Drawing.Point(136, 14)
        Me.SetSampleCount.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.SetSampleCount.Name = "SetSampleCount"
        Me.SetSampleCount.Size = New System.Drawing.Size(41, 20)
        Me.SetSampleCount.TabIndex = 7
        Me.AGPToolTips.SetToolTip(Me.SetSampleCount, resources.GetString("SetSampleCount.ToolTip"))
        Me.SetSampleCount.Value = New Decimal(New Integer() {16, 0, 0, 0})
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(20, 16)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(110, 13)
        Me.Label1.TabIndex = 8
        Me.Label1.Text = "Samples/Combination"
        '
        'SetMinAggressiveness
        '
        Me.SetMinAggressiveness.Location = New System.Drawing.Point(28, 50)
        Me.SetMinAggressiveness.Maximum = New Decimal(New Integer() {10, 0, 0, 0})
        Me.SetMinAggressiveness.Name = "SetMinAggressiveness"
        Me.SetMinAggressiveness.Size = New System.Drawing.Size(41, 20)
        Me.SetMinAggressiveness.TabIndex = 17
        Me.AGPToolTips.SetToolTip(Me.SetMinAggressiveness, "Least Autoguider Aggressiveness setting to be used during this profiling scan." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10))
        Me.SetMinAggressiveness.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        'SetMinMinMove
        '
        Me.SetMinMinMove.DecimalPlaces = 1
        Me.SetMinMinMove.Increment = New Decimal(New Integer() {1, 0, 0, 65536})
        Me.SetMinMinMove.Location = New System.Drawing.Point(20, 50)
        Me.SetMinMinMove.Maximum = New Decimal(New Integer() {10, 0, 0, 0})
        Me.SetMinMinMove.Name = "SetMinMinMove"
        Me.SetMinMinMove.Size = New System.Drawing.Size(41, 20)
        Me.SetMinMinMove.TabIndex = 25
        Me.AGPToolTips.SetToolTip(Me.SetMinMinMove, "Number of guiding corrections to be made for each" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "set of parameters during the p" & _
        "rofiling.  The duration" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "of the profiling run wil be approximately this number" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & _
        "times the average exposure time.")
        Me.SetMinMinMove.Value = New Decimal(New Integer() {1, 0, 0, 65536})
        '
        'SetMaxMInMove
        '
        Me.SetMaxMInMove.DecimalPlaces = 1
        Me.SetMaxMInMove.Increment = New Decimal(New Integer() {1, 0, 0, 65536})
        Me.SetMaxMInMove.Location = New System.Drawing.Point(99, 50)
        Me.SetMaxMInMove.Maximum = New Decimal(New Integer() {10, 0, 0, 0})
        Me.SetMaxMInMove.Name = "SetMaxMInMove"
        Me.SetMaxMInMove.Size = New System.Drawing.Size(41, 20)
        Me.SetMaxMInMove.TabIndex = 27
        Me.AGPToolTips.SetToolTip(Me.SetMaxMInMove, "Number of guiding corrections to be made for each" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "set of parameters during the p" & _
        "rofiling.  The duration" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "of the profiling run wil be approximately this number" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & _
        "times the average exposure time.")
        Me.SetMaxMInMove.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        'SetMaxAggressiveness
        '
        Me.SetMaxAggressiveness.Location = New System.Drawing.Point(102, 50)
        Me.SetMaxAggressiveness.Maximum = New Decimal(New Integer() {10, 0, 0, 0})
        Me.SetMaxAggressiveness.Name = "SetMaxAggressiveness"
        Me.SetMaxAggressiveness.Size = New System.Drawing.Size(41, 20)
        Me.SetMaxAggressiveness.TabIndex = 36
        Me.AGPToolTips.SetToolTip(Me.SetMaxAggressiveness, "Greatest Autoguider Aggressiveness setting to be used during this profiling scan." & _
        "" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10))
        Me.SetMaxAggressiveness.Value = New Decimal(New Integer() {10, 0, 0, 0})
        '
        'SetMaxDelay
        '
        Me.SetMaxDelay.DecimalPlaces = 2
        Me.SetMaxDelay.Location = New System.Drawing.Point(107, 50)
        Me.SetMaxDelay.Maximum = New Decimal(New Integer() {60, 0, 0, 0})
        Me.SetMaxDelay.Name = "SetMaxDelay"
        Me.SetMaxDelay.Size = New System.Drawing.Size(47, 20)
        Me.SetMaxDelay.TabIndex = 11
        Me.AGPToolTips.SetToolTip(Me.SetMaxDelay, "Longest Autoguider Delay After Corretion Time for evaluation.")
        Me.SetMaxDelay.Value = New Decimal(New Integer() {3, 0, 0, 0})
        '
        'SetMinDelay
        '
        Me.SetMinDelay.DecimalPlaces = 2
        Me.SetMinDelay.Location = New System.Drawing.Point(29, 50)
        Me.SetMinDelay.Maximum = New Decimal(New Integer() {60, 0, 0, 0})
        Me.SetMinDelay.Name = "SetMinDelay"
        Me.SetMinDelay.Size = New System.Drawing.Size(45, 20)
        Me.SetMinDelay.TabIndex = 12
        Me.AGPToolTips.SetToolTip(Me.SetMinDelay, "Shortest Autoguider Delay After Corretion Time for evaluation.")
        '
        'ScanExposureSet
        '
        Me.ScanExposureSet.DecimalPlaces = 1
        Me.ScanExposureSet.Location = New System.Drawing.Point(345, 12)
        Me.ScanExposureSet.Name = "ScanExposureSet"
        Me.ScanExposureSet.Size = New System.Drawing.Size(47, 20)
        Me.ScanExposureSet.TabIndex = 57
        Me.AGPToolTips.SetToolTip(Me.ScanExposureSet, "Autoguider Exposure Time (in seconds).  Use" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & """Set Exposure"" to calibrate this tim" & _
        "e to a target" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "ADU value for the guide star.")
        Me.ScanExposureSet.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        'NumericUpDown1
        '
        Me.NumericUpDown1.DecimalPlaces = 1
        Me.NumericUpDown1.Increment = New Decimal(New Integer() {1, 0, 0, 65536})
        Me.NumericUpDown1.Location = New System.Drawing.Point(20, 50)
        Me.NumericUpDown1.Maximum = New Decimal(New Integer() {10, 0, 0, 0})
        Me.NumericUpDown1.Name = "NumericUpDown1"
        Me.NumericUpDown1.Size = New System.Drawing.Size(41, 20)
        Me.NumericUpDown1.TabIndex = 25
        Me.AGPToolTips.SetToolTip(Me.NumericUpDown1, "Shortest Autoguider Minimum Movement setting" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "to beused during this profiling sca" & _
        "n." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10))
        Me.NumericUpDown1.Value = New Decimal(New Integer() {1, 0, 0, 65536})
        '
        'NumericUpDown2
        '
        Me.NumericUpDown2.DecimalPlaces = 1
        Me.NumericUpDown2.Increment = New Decimal(New Integer() {1, 0, 0, 65536})
        Me.NumericUpDown2.Location = New System.Drawing.Point(99, 50)
        Me.NumericUpDown2.Maximum = New Decimal(New Integer() {10, 0, 0, 0})
        Me.NumericUpDown2.Name = "NumericUpDown2"
        Me.NumericUpDown2.Size = New System.Drawing.Size(41, 20)
        Me.NumericUpDown2.TabIndex = 27
        Me.AGPToolTips.SetToolTip(Me.NumericUpDown2, "Longest Minimum Movement setting to be used" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "during this profiling scan." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10))
        Me.NumericUpDown2.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        'SetExposureButton
        '
        Me.SetExposureButton.Location = New System.Drawing.Point(93, 455)
        Me.SetExposureButton.Name = "SetExposureButton"
        Me.SetExposureButton.Size = New System.Drawing.Size(97, 32)
        Me.SetExposureButton.TabIndex = 55
        Me.SetExposureButton.Text = "Set Exposure"
        Me.AGPToolTips.SetToolTip(Me.SetExposureButton, "Adjusts the exposure time to meet the target ADU" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "as set in the Adaptive Test win" & _
        "dow.")
        Me.SetExposureButton.UseVisualStyleBackColor = True
        '
        'FindStarButton
        '
        Me.FindStarButton.Location = New System.Drawing.Point(12, 455)
        Me.FindStarButton.Name = "FindStarButton"
        Me.FindStarButton.Size = New System.Drawing.Size(75, 32)
        Me.FindStarButton.TabIndex = 56
        Me.FindStarButton.Text = "Find Star"
        Me.AGPToolTips.SetToolTip(Me.FindStarButton, "Takes an autoguider image and locates a suitable star" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "for autoguiding, i.e. good" & _
        " brightness, no neighbors and" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "away from the edge of the image.")
        Me.FindStarButton.UseVisualStyleBackColor = True
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(247, 14)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(77, 13)
        Me.Label2.TabIndex = 49
        Me.Label2.Text = "Exposure (sec)"
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.Label3)
        Me.GroupBox1.Controls.Add(Me.SetMaxAggressiveness)
        Me.GroupBox1.Controls.Add(Me.Label4)
        Me.GroupBox1.Controls.Add(Me.SetMinAggressiveness)
        Me.GroupBox1.Location = New System.Drawing.Point(242, 49)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(168, 79)
        Me.GroupBox1.TabIndex = 52
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Aggressiveness (+X,-X,+Y,-Y)"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(103, 25)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(51, 13)
        Me.Label3.TabIndex = 43
        Me.Label3.Text = "Maximum"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(25, 25)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(48, 13)
        Me.Label4.TabIndex = 44
        Me.Label4.Text = "Minimum"
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.Label6)
        Me.GroupBox2.Controls.Add(Me.NumericUpDown2)
        Me.GroupBox2.Controls.Add(Me.Label7)
        Me.GroupBox2.Controls.Add(Me.NumericUpDown1)
        Me.GroupBox2.Controls.Add(Me.SetMaxMInMove)
        Me.GroupBox2.Controls.Add(Me.SetMinMinMove)
        Me.GroupBox2.Location = New System.Drawing.Point(461, 49)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(160, 79)
        Me.GroupBox2.TabIndex = 53
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Minimum Movement (sec)"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(91, 25)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(51, 13)
        Me.Label6.TabIndex = 45
        Me.Label6.Text = "Maximum"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(13, 25)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(48, 13)
        Me.Label7.TabIndex = 46
        Me.Label7.Text = "Minimum"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(26, 25)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(48, 13)
        Me.Label9.TabIndex = 42
        Me.Label9.Text = "Minimum"
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.Label5)
        Me.GroupBox3.Controls.Add(Me.Label9)
        Me.GroupBox3.Controls.Add(Me.SetMinDelay)
        Me.GroupBox3.Controls.Add(Me.SetMaxDelay)
        Me.GroupBox3.Location = New System.Drawing.Point(15, 49)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(175, 79)
        Me.GroupBox3.TabIndex = 54
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "Delay After Correction (sec)"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(104, 25)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(51, 13)
        Me.Label5.TabIndex = 41
        Me.Label5.Text = "Maximum"
        '
        'AGScanTestForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.CloseButton
        Me.ClientSize = New System.Drawing.Size(656, 499)
        Me.Controls.Add(Me.ScanExposureSet)
        Me.Controls.Add(Me.FindStarButton)
        Me.Controls.Add(Me.SetExposureButton)
        Me.Controls.Add(Me.GroupBox3)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.SetSampleCount)
        Me.Controls.Add(Me.BestTextBox)
        Me.Controls.Add(Me.AnalysisTextBox)
        Me.Controls.Add(Me.StartButton)
        Me.Controls.Add(Me.StopButton)
        Me.Controls.Add(Me.CloseButton)
        Me.Controls.Add(Me.SaveButton)
        Me.Controls.Add(Me.AGProfileData)
        Me.Name = "AGScanTestForm"
        Me.Text = "AutoGuide Profiler3 Scan Mode Testing"
        CType(Me.SetSampleCount, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.SetMinAggressiveness, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.SetMinMinMove, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.SetMaxMInMove, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.SetMaxAggressiveness, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.SetMaxDelay, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.SetMinDelay, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.ScanExposureSet, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.NumericUpDown1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.NumericUpDown2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents AGProfileData As System.Windows.Forms.TextBox
    Friend WithEvents SaveButton As System.Windows.Forms.Button
    Friend WithEvents CloseButton As System.Windows.Forms.Button
    Friend WithEvents StopButton As System.Windows.Forms.Button
    Friend WithEvents SaveFolderDialog As System.Windows.Forms.FolderBrowserDialog
    Friend WithEvents StartButton As System.Windows.Forms.Button
    Friend WithEvents AnalysisTextBox As System.Windows.Forms.TextBox
    Friend WithEvents BestTextBox As System.Windows.Forms.TextBox
    Friend WithEvents SetSampleCount As System.Windows.Forms.NumericUpDown
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents AGPToolTips As System.Windows.Forms.ToolTip
    Friend WithEvents SetMinAggressiveness As System.Windows.Forms.NumericUpDown
    Friend WithEvents SetMinMinMove As System.Windows.Forms.NumericUpDown
    Friend WithEvents SetMaxMInMove As System.Windows.Forms.NumericUpDown
    Friend WithEvents SetMaxAggressiveness As System.Windows.Forms.NumericUpDown
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents SetMaxDelay As System.Windows.Forms.NumericUpDown
    Friend WithEvents SetMinDelay As System.Windows.Forms.NumericUpDown
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
    Friend WithEvents SetExposureButton As System.Windows.Forms.Button
    Friend WithEvents FindStarButton As System.Windows.Forms.Button
    Friend WithEvents ScanExposureSet As System.Windows.Forms.NumericUpDown
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents NumericUpDown2 As System.Windows.Forms.NumericUpDown
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents NumericUpDown1 As System.Windows.Forms.NumericUpDown
End Class
