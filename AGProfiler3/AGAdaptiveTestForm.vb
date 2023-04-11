Imports System.IO
Imports System.Reflection


Public Class AGAdaptiveTestForm
    'Windows Visual Basic Forms Application: AGAnalysis
    '
    'Standalone application to run a range of autoguidng parameters and display/save results for analysis
    '
    ' ------------------------------------------------------------------------
    '
    ' Author: R.McAlister (2015, 2016)
    ' Version 1.0 (Superceded by 1.1)
    ' Version 1.1
    '   Added controls to set initial and final exposure times
    '   Improved backlash estimation algorithm
    ' Version 1.2
    ' Version 1.3 Added some tool tips and published as installable application
    '
    ' Version 2.0 Major Changes:
    '       1. Added tests for individual aggressiveness settings (xpos, xneg, ypos, yneg)
    '       2. Testing from center outwards on settings with cut-off
    '       3. Changed default sample count to 15
    '       4. Added ability to start test at specific settings
    '       5. Added heuristic for on-the-fly monitoring and optimization (this was the big one)
    '       6. Added graphical representation of data (this was also a big one)
    '       7. Added database for samples
    '       8. Added continuous sampling with optimizing (on-the-fly)
    '       9. Added option to download and/or upload settings to TSX
    '       10. Added ability to pick specific settings to profile/adjust
    '       11. Added upload and download of settings from TSX
    '       23. Generally cleaned up the code
    '
    ' ------------------------------------------------------------------------
    'This form encapsulates the AutoGuideProfile user interface
    'Three data results boxes
    'Four button controls:  Start, Stop, Save and Cancel -- should be self-explanatory
    '
    Public Shared agprofileabort As Boolean       'Common abort flag
    Public Shared agsamples
    Public Shared minimumcycletime

    Private Sub AGAnalysisForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'Initializes the form and creates an AutoGuideProfile object to run the profiling

        ' Set up the delays for the ToolTip.
        AGPToolTips.AutoPopDelay = 5000
        AGPToolTips.InitialDelay = 1000
        AGPToolTips.ReshowDelay = 500
        ' Force the ToolTip text to be displayed whether or not the form is active.
        AGPToolTips.ShowAlways = True

        Me.BackColor = Color.LightBlue
        minimumcycletime = 0
        agsamples = New AGData

        Return
    End Sub

    Private Sub FindStarButton_Click(sender As Object, e As EventArgs) Handles FindStarButton.Click
          'Fronts for call to find guide star
        AGOps.ManageFindStar()
        Return
    End Sub

    Private Sub StopButton_Click(sender As Object, e As EventArgs) Handles StopButton.Click
        'Signals the profile run to abort
        agprofileabort = True
        Return
    End Sub

    Private Sub SaveButton_Click(sender As Object, e As EventArgs) Handles SaveButton.Click
        'User picks a directory and all the data set text is saved as the text file "AGAdaptiveProfile.txt"

        Dim sdata As String = ""
        Dim sddo = SaveFolderDialog.ShowDialog()
        Dim sddof = SaveFolderDialog.SelectedPath
        If sddof <> "" Then
            For Each entry In agsamples.AGSampleData
                sdata = sdata + agsamples.StringSample(entry) + vbCrLf
            Next
            System.IO.File.WriteAllText((sddof + "\AGAdaptiveProfile.txt"), sdata)
            TTUtility.LogIt("Wrote results to AGAdaptiveProfile.txt")
        End If
        Return
    End Sub

    Private Sub CancelButton_Click(sender As Object, e As EventArgs) Handles CloseButton.Click, CloseButton.Click
        Close()
        Return
    End Sub

    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '    AutoGuideProfile routines
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

    'Configuration constants 

    Const ExposureIncrement = 1
    Const Aggressivenesses = 10
    Const AggressivenessIncrement = 1
    Const MinMoves = 11
    Const MinMoveIncrements = 0.05
    Const MaxMoves = 5
    Const MaxMoveIncrement = 0.5


    Private Function EstimateBacklash() As String
        'Compare calibration values to estimate how much backlash might have been
        '   The assumption is that if there is no backlash then the vectors should be equal and opposite.
        '   Otherwise the difference is the backlash in pixels per second
        '
        Dim tsx_ag = CreateObject("TheSky64.ccdsoftCamera")
        tsx_ag.Autoguider = 1

        Dim caltimeX = tsx_ag.SavedCalibrationTimeX / 100
        Dim caltimeY = tsx_ag.SavedCalibrationTimeY / 100

        'Calibration magnitudes in pixels
        'Have to two step this process to get to 2 significant digits (compiler "feature")
        Const dplaces = 3

        Dim dPXx = Math.Abs(Math.Round(tsx_ag.CalibrationVectorXPositiveXComponent, dplaces))
        Dim dPXy = Math.Abs(Math.Round(tsx_ag.CalibrationVectorXPositiveYComponent, dplaces))
        Dim dNXx = Math.Abs(Math.Round(tsx_ag.CalibrationVectorXNegativeXComponent, dplaces))
        Dim dNXy = Math.Abs(Math.Round(tsx_ag.CalibrationVectorXNegativeYComponent, dplaces))
        Dim dPYx = Math.Abs(Math.Round(tsx_ag.CalibrationVectorYPositiveXComponent, dplaces))
        Dim dPYy = Math.Abs(Math.Round(tsx_ag.CalibrationVectorYPositiveYComponent, dplaces))
        Dim dNYx = Math.Abs(Math.Round(tsx_ag.CalibrationVectorYNegativeXComponent, dplaces))
        Dim dNYy = Math.Abs(Math.Round(tsx_ag.CalibrationVectorYNegativeYComponent, dplaces))

        'Average calibration distances in pixels
        Dim blXx = Backlash(dPXx, dNXx, caltimeX)
        Dim blYx = Backlash(dPYx, dNYx, caltimeY) 'Not really needed, assume smallish
        Dim blXy = Backlash(dPXy, dNXy, caltimeX) 'Not really needed, assume smallish
        Dim blYy = Backlash(dPYy, dNYy, caltimeY)

        'Calibration backlash in pixels/guidesec
        'Average the two directions

        Dim dXbacklash = blXx
        Dim dYbacklash = blYy

        Dim currentXcompensation = tsx_ag.AutoguiderBacklashXAxis
        Dim currentYcompensation = tsx_ag.AutoguiderBacklashYAxis

        'Calibration backlash in guidesec

        tsx_ag = Nothing

        Return ("Calculated Backlash = " + TTUtility.DoubleClip(dXbacklash, 2) + " (sec) in X and " + TTUtility.DoubleClip(dYbacklash, 2) + " (sec) in Y" + vbCrLf +
                "Current Backlash Compensation is " + TTUtility.DoubleClip(currentXcompensation / 1000, 2) + " (sec) in X and " +
                TTUtility.DoubleClip(currentYcompensation / 1000, 2) + " (sec) in Y")

    End Function

    Private Function Backlash(dA, dB, duration) As Double
        'Calculates backlash as a function of two distances (in pixels) and a time duration for the longest
        'Backlash = calibration time / (positive pixel distance - negative pixel distance)
        '
        If dA > dB Then
            Return (duration * (1 - dB / dA))
        ElseIf dA = dB Then
            Return (0)
        Else
            Return (duration * (1 - dA / dB))
        End If

    End Function

    Private Sub Instructions_Click(sender As Object, e As EventArgs)
        'Write out the instructions in a window
        Const InstPath = "AGProfiler3.Instructions.txt"

        Dim instReader As StreamReader
        Dim AG2assembly As [Assembly]
        AG2assembly = [Assembly].GetExecutingAssembly()
        instReader = New StreamReader(AG2assembly.GetManifestResourceStream(InstPath))
        Try
            AG2assembly = [Assembly].GetExecutingAssembly()
            instReader = New StreamReader(AG2assembly.GetManifestResourceStream(InstPath))
        Catch ex As Exception
            MessageBox.Show("Instructions weren't found!" + ex.Message)
        End Try
        Try
            If instReader.Peek() <> -1 Then
                MsgBox(instReader.ReadToEnd())
            End If
        Catch ex As Exception
            MessageBox.Show("Error reading stream!" + ex.Message)
        End Try

        Return

    End Sub

    Private Sub AdaptiveTestButton_Click(sender As Object, e As EventArgs) Handles AdaptiveTestButton.Click
        'Starts sample tests and changes


        TTUtility.LogIt("Starting Autoguider Training...")
        agprofileabort = False
        Dim runcolor = Color.LightPink
        Dim lastcolor
        Do While Not agprofileabort
            If DelayCheckBox.Checked And Not agprofileabort Then
                lastcolor = DelayChart.BackColor
                DelayChart.BackColor = runcolor
                DelayTest()
                DelayChart.BackColor = lastcolor
                GraphAll()
            End If
            If MinMoveCheckBox.Checked And Not agprofileabort Then
                lastcolor = MinMoveChart.BackColor
                MinMoveChart.BackColor = runcolor
                MinMoveTest()
                MinMoveChart.BackColor = lastcolor
                GraphAll()
            End If
            If MaxMoveCheckBox.Checked And Not agprofileabort Then
                lastcolor = MaxMoveChart.BackColor
                MaxMoveChart.BackColor = runcolor
                MaxMoveTest()
                MaxMoveChart.BackColor = lastcolor
                GraphAll()
            End If
            If AggressivenessCheckBox.Checked And Not agprofileabort Then
                lastcolor = XPlusChart.BackColor
                XPlusChart.BackColor = runcolor
                XPlusAggressivenessTest()
                XPlusChart.BackColor = lastcolor
                GraphAll()
                '
                lastcolor = XMinusChart.BackColor
                XMinusChart.BackColor = runcolor
                XMinusAggressivenessTest()
                XMinusChart.BackColor = lastcolor
                GraphAll()
                '
                lastcolor = YPlusChart.BackColor
                YPlusChart.BackColor = runcolor
                YPlusAggressivenessTest()
                YPlusChart.BackColor = lastcolor
                GraphAll()
                '
                lastcolor = YMinusChart.BackColor
                YMinusChart.BackColor = runcolor
                YMinusAggressivenessTest()
                YMinusChart.BackColor = lastcolor
                GraphAll()
                '
            End If
            Optimize()
        Loop
        Return
    End Sub

    Private Sub GetTSXSettingsButton_Click(sender As Object, e As EventArgs) Handles GetTSXSettingsButton.Click
        'Gets autoguider setup parameters from TSX
        DownloadSettings()
        Return
    End Sub


    Private Sub SendTSXSettingsButton_Click(sender As Object, e As EventArgs) Handles SendTSXSettingsButton.Click
        'Sends autoguider setup parameters to TSX
        UploadSettings()
        Return
    End Sub

    Private Sub ResetSamplesButton_Click(sender As Object, e As EventArgs) Handles ResetSamplesButton.Click
        'wipes out any sample data and start a new database
        agsamples = Nothing
        agsamples = New AGData
        ClearGraphs()
        Return
    End Sub

    Private Sub TestScanButton_Click(sender As Object, e As EventArgs) Handles TestScanButton.Click
        'Opens PreTest form window
        Dim agpre = New AGScanTestForm
        agpre.Show()
        Return

    End Sub

    Private Sub SetExposureButton_Click(sender As Object, e As EventArgs) Handles SetExposureButton.Click
        'Get the best exposure time based on the target ADU
        ExposureSet.Value = AGOps.ManageExposure(TargetADUSet.Value, ExposureSet.Value)
        Return
    End Sub

    Private Sub StartAGButton_Click(sender As Object, e As EventArgs) Handles StartAGButton.Click
        'Fire off autoguiding: connect scope, autoguider and turn on autoguiding

        'Connect telescope if not already connected. 
        TTUtility.LogIt("Connecting telescope")
        Dim mtstat
        Dim tsx_mt = CreateObject("TheSky64.sky6RASCOMtele")
        Try
            mtstat = tsx_mt.Connect()
        Catch ex As Exception
            MsgBox("Mount connect error: " + ex.Message)
            tsx_mt = Nothing
            Return
        End Try
        tsx_mt = Nothing

        'Measure how long an autoguide exposure and download actually take, throw away one to clear the path. Two more and average.
        TTUtility.LogIt("Estimating minimum cycle time")
        minimumcycletime = AGOps.TurnAround()
        minimumcycletime = (AGOps.TurnAround() + AGOps.TurnAround()) / 2

        'if autoguider not already running, then set the exposure and turn it on.
        TTUtility.LogIt("Starting autoguider (minimum cycle time is " + TTUtility.DoubleClip(minimumcycletime, 2) + " sec")
        AGOps.StartAG(ExposureSet.Value)
        Return

    End Sub
End Class







