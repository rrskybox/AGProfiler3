Public Class AGScanTestForm
    'Windows Visual Basic Forms Application: AGPReTest
    '
    'Windows application to run a range of autoguidng parameters and display/save results for analysis
    '
    ' ------------------------------------------------------------------------
    '
    ' Author: R.McAlister (2015)
    ' Version 1.0 
    ' Version 1.1
    '   Added controls to set initial and final exposure times
    '   Improved backlash estimation algorithm
    ' Version 1.2
    ' Version 1.3 Added some tool tips and published as installable application
    ' Version 2.0 Enhanced:
    '       1. Added tests for individual aggressiveness settings (xpos, xneg, ypos, yneg)
    '       2. Testing from center outwards on settings with cut-off
    '       3. Changed default sample count to 15
    '       4. Added ability to start test at specific settings
    '       5. Added heuristic for on-the-fly monitoring and optimization (this was a hard one)
    '       6. Generally cleaned up the code
    ' Version 3.0
    '       1. Integrated with AGPRofiler2 to make AGProfiler3 - uploads results to AGAnalysisForm
    '       2. Modified "exposure" tests.  Exposure remains constant, but delay after correction
    '           stepped to change overall cycle time of exposure
    '       3. Added function to adjust the exposure time based on ADU target.
    '       4. Moved the controls around a bit.
    '
    ' ------------------------------------------------------------------------
    'This form encapsulates the AGPreTest user interface
    '
    Private agprofileabort As Boolean       'Common abort flag

    Private bestDelay As Integer
    Private bestAggressiveness As Integer
    Private bestMminmove As Double

    Dim alertmsg = vbCrLf + "For this autoguide profiling to work correctly:" + vbCrLf + vbCrLf +
        vbTab + "1. The Sky X Pro V 10.3.0 (DB 9051 or greater) with Camera Add On must be running," + vbCrLf +
        vbTab + "2. Telescope and Autoguide Camera must be connected," + vbCrLf +
    vbTab + "3. Autoguide must have been calibrated successfully, " + vbCrLf +
        vbTab + "4. A decent guide star must have been selected." + vbCrLf + vbCrLf +
        "Press Start when ready, the profiling takes around an hour, so go get some coffee." + vbCrLf + vbCrLf +
        "Upon completion or Stop, the data can be Saved to a file AGProfile.txt in a folder of your choosing." + vbCrLf +
        "The results will also be uploaded into AGProfiler3."

    Private Sub AGAnalysisForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'Initializes the form and creates an AutoGuideProfile object to run the profiling

        AGProfileData.Text = alertmsg
        ' Set up the delays for the ToolTip.
        AGPToolTips.AutoPopDelay = 5000
        AGPToolTips.InitialDelay = 1000
        AGPToolTips.ReshowDelay = 500
        ' Force the ToolTip text to be displayed whether or not the form is active.
        AGPToolTips.ShowAlways = True

        Me.BackColor = Color.LightBlue

        ScanExposureSet.Value = AGAdaptiveTestForm.ExposureSet.Value

        Return
    End Sub

    Private Sub SetExposureButton_Click(sender As Object, e As EventArgs) Handles SetExposureButton.Click
        'Get the best exposure time based on the target ADU
        ScanExposureSet.Value = AGOps.ManageExposure(AGAdaptiveTestForm.TargetADUSet.Value, ScanExposureSet.Value)
        AGAdaptiveTestForm.ExposureSet.Value = ScanExposureSet.Value
        Return
    End Sub

  
    Private Sub StartButton_Click(sender As Object, e As EventArgs) Handles StartButton.Click
        'Initiates a profile run
        'Note: Current TSX configuration settings are saved, then restored after the run is completed or aborted
        'Run the profile tests
        '
        Dim tag = CreateObject("TheSky64.ccdsoftCamera")
        tag.Autoguider = 1

        samplesize = SetSampleCount.Value
        AnalysisTextBox.Text = EstimateBacklash()
        TTUtility.SaveTSXState()
        agprofileabort = False
        Profile()
        AGProfileData.Text = AGProfileData.Text + vbCrLf + vbCrLf + "Profiling Finished" + vbCrLf
        TTUtility.RestoreTSXState(False)
        'If this hasn't been stopped by an abort, then update Training Form values
        If agprofileabort <> True Then
            UploadResults()
        End If
        Return
    End Sub

    Private Sub StopButton_Click(sender As Object, e As EventArgs) Handles StopButton.Click
        'Signals the profile run to abort
        agprofileabort = True
        Return
    End Sub

    Private Sub SaveButton_Click(sender As Object, e As EventArgs) Handles SaveButton.Click
        'User picks a directory and all the data set text is saved as the text file "AGScanProfile.txt"

        Dim sddo = SaveFolderDialog.ShowDialog()
        Dim sddof = SaveFolderDialog.SelectedPath
        If sddof <> "" Then
            Dim sdata = AGProfileData.Text + vbCrLf + BestTextBox.Text + vbCrLf + AnalysisTextBox.Text + vbCrLf
            System.IO.File.WriteAllText((sddof + "\AGScanProfile.txt"), sdata)
        End If
        'Now upload the results to the adaptive test, whether it wants it or not
        TTUtility.LogIt("Wrote results to AGScanProfile.txt")
        UploadResults()

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
    Const DelayIncrement = 1
    Const AggressivenessIncrement = 1
    Const MinMoveIncrements = 0.1
    Const MaxMoves = 5
    Const MaxMoveIncrement = 0.1

    Const RMSDecimalAccuracy = 2
    Const ExposureDecimalAccuracy = 1
    Const DelayDecimalAccuracy = 0
    Const AggressivenessDecimalAccuracy = 0
    Const MinMoveDecimalAccuracy = 1

    Const SiderealRate = 15 'arcsec/second
    Const GuideRate = 7.5 'arcsec/second

    'Common variables and arrays
    Dim StartDelay As Integer
    Dim EndDelay As Integer
    Dim DelayPoints As Integer
    Dim StartAggressiveness As Integer
    Dim EndAggressiveness As Integer
    Dim AggressivenessPoints As Integer
    Dim StartMinMove As Double
    Dim EndMinMove As Double
    Dim MinMovePoints As Integer

    Dim samplesize As Integer

    Dim currentexposure As Double
    Dim currentdelay As Double
    Dim currentaggressiveness As Integer
    Dim currentminmove As Double
    Dim leasterror As Double


    Private Sub Profile()
        'This routine runs the autoguider for a fixed number of samples at cycle settings for each of 10 agressiveness settings,
        '    then compares the results in terms of RMS error.
        'Rev 1 assumes that 
        '   1. a calibration performed
        '   2. an appropriate guide star has been selected
        '   3. a frame size selected (either sub or full)

        'Prepare array to store results of trials (indexed delay, aggressiveness) -> RMS 
        'Determine number and end points for guider delay after correction settings
        StartDelay = Int(SetMinDelay.Value)
        EndDelay = Int(SetMaxDelay.Value)
        DelayPoints = EndDelay - StartDelay + 1
        If DelayPoints < 0 Then
            EndDelay = StartDelay
            DelayPoints = 0
        End If

        'Determin number and end points for aggressiveness settings
        StartAggressiveness = Int(SetMinAggressiveness.Value)
        EndAggressiveness = Int(SetMaxAggressiveness.Value)
        AggressivenessPoints = EndAggressiveness - StartAggressiveness + 1

        'Determin number and end points for minimum move settings
        StartMinMove = SetMinMinMove.Value
        EndMinMove = SetMaxMInMove.Value
        MinMovePoints = (EndMinMove / MinMoveIncrements) - (StartMinMove / MinMoveIncrements) + 1

        Dim fRMSex(DelayPoints, AggressivenessPoints) As Double
        Dim fRMSmin(MinMovePoints) As Double

        Dim agstat As Integer
        Dim trialRMS As Double
        Dim trialcount As Integer

        Dim tsx_ag = CreateObject("TheSky64.ccdsoftCamera")
        tsx_ag.Autoguider = 1

        tsx_ag.Abort()
        'Wait for the autoguider to stop, if needed
        System.Threading.Thread.Sleep(2000)
        tsx_ag.ShowAutoguider = True

        tsx_ag.AutoSaveOn = False                                                   'Turn off autosave to save time and space
        tsx_ag.AutoguiderExposureTime = Convert.ToDouble(ScanExposureSet.Value)

        'Measure how long an autoguide exposure and download actually take, throw away one to clear the path. Two more and average.
        Dim cycletime = (AGOps.TurnAround())
        cycletime = (AGOps.TurnAround() + AGOps.TurnAround()) / 2

        tsx_ag.Asynchronous = True
        'Turn on autoguiding
        Try
            agstat = tsx_ag.Autoguide()
        Catch ex As Exception
            MsgBox("Autoguiding error -- check start up instructions")
            tsx_ag = Nothing
            Return
        End Try

        'Wait 5 exposure+delays for the autoguider to get going
        System.Threading.Thread.Sleep((cycletime + currentdelay) * 3000)

        For DelayCount = 1 To DelayPoints
            For AggressivenessCount = 1 To AggressivenessPoints
                trialRMS = 0
                trialcount = 0
                tsx_ag.AutoguiderDelayAfterCorrection = Convert.ToDouble(DelayCount + StartDelay - 1) * 1000
                Dim test = tsx_ag.AutoguiderDelayAfterCorrection
                tsx_ag.AutoguiderAggressiveness = Convert.ToDouble(AggressivenessCount + StartAggressiveness - 1)
                tsx_ag.AutoguiderMinimumMove = StartMinMove * 1000                  'Autoguider Min Move in 1ms (undocumented)
                currentdelay = tsx_ag.AutoguiderDelayAfterCorrection / 1000
                currentexposure = tsx_ag.AutoguiderExposureTime
                currentaggressiveness = tsx_ag.AutoguiderAggressiveness
                currentminmove = tsx_ag.AutoguiderMinimumMove / 1000                'Autoguider Min Move in 1ms (undocumented)
                DisplayData(fRMSex, fRMSmin)
                'Wait for a few seconds for everything to clear, then set for async operation
                tsx_ag.Asynchronous = True
                System.Threading.Thread.Sleep(3000)

                For repetitions = 1 To samplesize
                    Application.DoEvents()
                    If agprofileabort Then
                        tsx_ag.Abort()
                        Return
                    End If
                    System.Threading.Thread.Sleep((cycletime * 1000) + (currentdelay * 1000))
                    trialRMS += (tsx_ag.GuideErrorX ^ 2 + tsx_ag.GuideErrorY ^ 2)
                    trialcount += 1
                Next
                fRMSex((DelayCount - 1), (AggressivenessCount - 1)) = Math.Sqrt(trialRMS / trialcount)
            Next
        Next

        'Minimum Move profile
        DisplayData(fRMSex, fRMSmin) 'Sets best delay and aggressiveness
        For minmove = 1 To MinMovePoints
            trialRMS = 0
            trialcount = 0
            tsx_ag.AutoguiderDelayAfterCorrection = bestDelay
            tsx_ag.AutoguiderAggressiveness = bestAggressiveness
            tsx_ag.AutoguiderMinimumMove = ((minmove - 1) * MinMoveIncrements) * 1000  'Set in increments of 1 msec (undocumented)
            currentexposure = tsx_ag.AutoguiderExposureTime
            currentdelay = tsx_ag.AutoguiderDelayAfterCorrection / 1000
            currentaggressiveness = tsx_ag.AutoguiderAggressiveness
            currentminmove = tsx_ag.AutoguiderMinimumMove / 1000                        'Set in increments of 1 msec (undocumented)
            DisplayData(fRMSex, fRMSmin)
            'Wait for a few seconds for everything to clear
            System.Threading.Thread.Sleep(3000)

            For repetitions = 0 To samplesize
                Application.DoEvents()
                If agprofileabort Then
                    tsx_ag.Abort()
                    tsx_ag = Nothing
                    Return
                End If
                System.Threading.Thread.Sleep((currentexposure * 1000) + (currentdelay * 1000))
                trialRMS += (tsx_ag.GuideErrorX ^ 2 + tsx_ag.GuideErrorY ^ 2)
                trialcount += 1
            Next
            fRMSmin(minmove - 1) = Math.Sqrt(trialRMS / trialcount)
        Next
        DisplayData(fRMSex, fRMSmin)
        tsx_ag = Nothing
        Return

    End Sub

    Private Sub DisplayData(fRMSex, fRMSmin)
        AGProfileData.Text = ""
        DisplayDelayAggrData(fRMSex)
        DisplayMinMoveData(fRMSmin)
        DisplayBestData(fRMSex, fRMSmin)
        Return
    End Sub

    Private Sub DisplayDelayAggrData(FRMSex)

        'Display data and analyze results
        Dim rawdata = AGProfileData.Text
        rawdata = rawdata + vbCrLf + "RMS(z) at Minimum Autoguider Move = " + TTUtility.DoubleClip(currentminmove, MinMoveDecimalAccuracy) + vbCrLf + "   Aggressiveness:"
        For i = 1 To AggressivenessPoints
            rawdata = rawdata + vbTab + TTUtility.DoubleClip(((i * AggressivenessIncrement) + (StartAggressiveness - 1)), AggressivenessDecimalAccuracy)
        Next
        rawdata = rawdata + vbCrLf + "Delay After Correction(sec)" + vbCrLf
        For i = 1 To DelayPoints
            rawdata = rawdata + "  " + TTUtility.DoubleClip((i + StartDelay - 1), 0) + vbTab + vbTab
            For j = 1 To AggressivenessPoints
                rawdata = rawdata + TTUtility.DoubleClip(FRMSex(i - 1, j - 1), RMSDecimalAccuracy) + vbTab
            Next
            rawdata = rawdata + vbCrLf
        Next
        AGProfileData.Text = rawdata
        Me.Show()
        Return
    End Sub

    Private Sub DisplayMinMoveData(fRMSmin)
        'Display data and analyze results
        Dim rawdata = AGProfileData.Text
        rawdata = rawdata + vbCrLf + "RMS(z) at Delay = " + TTUtility.DoubleClip(currentdelay, ExposureDecimalAccuracy) +
                "  Aggressiveness = " + TTUtility.DoubleClip(currentaggressiveness, AggressivenessDecimalAccuracy) + vbCrLf + "   Minimum Move:"
        For i = 0 To MinMovePoints - 1
            rawdata = rawdata + vbTab + TTUtility.DoubleClip((MinMoveIncrements * i + StartMinMove), MinMoveDecimalAccuracy)
        Next
        rawdata = rawdata + vbCrLf + vbTab + vbTab
        For i = 1 To MinMovePoints
            rawdata = rawdata + TTUtility.DoubleClip(fRMSmin(i - 1), RMSDecimalAccuracy) + vbTab
        Next
        AGProfileData.Text = rawdata
        Me.Show()
        Return
    End Sub

    Private Sub DisplayBestData(fRMSex, fRMSmin)
        'Scans the profiler data array to determine lowest value and set AG profile properties accordinly
        Dim rawdata = AGProfileData.Text

        bestDelay = StartDelay
        bestAggressiveness = StartAggressiveness
        bestminmove = StartMinMove

        leasterror = fRMSex(0, 0)
        For i = 1 To DelayPoints
            For j = 1 To AggressivenessPoints
                If (fRMSex(i - 1, j - 1) < leasterror) And (fRMSex(i - 1, j - 1) > 0) Then
                    leasterror = fRMSex(i - 1, j - 1)
                    bestDelay = (i + StartDelay - 1) * DelayIncrement
                    bestAggressiveness = j * AggressivenessIncrement
                End If
            Next
        Next
        leasterror = fRMSmin(0)
        For i = 1 To MinMovePoints
            If (fRMSmin(i - 1) < leasterror) And (fRMSmin(i - 1) > 0) Then
                leasterror = fRMSmin(i - 1)
                bestminmove = i * MinMoveIncrements
            End If
        Next
        BestTextBox.Text = "Best combination: Exposure = " + TTUtility.DoubleClip(ScanExposureSet.Value, ExposureDecimalAccuracy) +
                            " (sec), Delay After Correction = " + TTUtility.DoubleClip(bestDelay, AggressivenessDecimalAccuracy) +
                            " (sec), Aggressiveness = " + TTUtility.DoubleClip(bestAggressiveness, AggressivenessDecimalAccuracy) +
                            ", Minimum Move = " + TTUtility.DoubleClip(bestminmove, MinMoveDecimalAccuracy)
        Return

    End Sub


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

    Private Sub FindStarButton_Click(sender As Object, e As EventArgs) Handles FindStarButton.Click
        'Fronts for call to find guide star
        AGOps.ManageFindStar()
        Return
    End Sub

    Private Sub UploadResults()
        'Copy the results of the scan to the Adaptive Test settings

        AGAdaptiveTestForm.MinimumMoveSet.Value = bestminmove
        AGAdaptiveTestForm.XplusAggressiveSet.Value = bestAggressiveness
        AGAdaptiveTestForm.XminusAggressiveSet.Value = bestAggressiveness
        AGAdaptiveTestForm.YplusAggressiveSet.Value = bestAggressiveness
        AGAdaptiveTestForm.YminusAggressiveSet.Value = bestAggressiveness
        If bestDelay <= 0 Then
            AGAdaptiveTestForm.ShortestDACTime.Value = 0
        Else
            AGAdaptiveTestForm.ShortestDACTime.Value = bestDelay - 1
        End If
        AGAdaptiveTestForm.LongestDACTime.Value = bestDelay + 1
        Return

    End Sub

End Class







