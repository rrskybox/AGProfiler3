Module AGTrain
    'Methods for training AG results from different settings

    Const MinMinMove = 0
    Const MaxMinMove = 1
    Const MinMoveInc = 0.1
    Const MinMaxMove = 0.1
    Const MaxMaxMove = 1
    Const MaxMoveInc = 0.1
    Const MinAggressiveness = 0
    Const MaxAggressiveness = 10

    Public commonexposure As Double = AGAdaptiveTestForm.ExposureSet.Value
    Public bestdelayaftercorrection As Double = AGAdaptiveTestForm.ShortestDACTime.Value
    Public bestminmove As Double = AGAdaptiveTestForm.MinimumMoveSet.Value
    Public bestmaxmove As Double = AGAdaptiveTestForm.MaximumMoveSet.Value
    Public bestxplus As Double = AGAdaptiveTestForm.XplusAggressiveSet.Value
    Public bestxminus As Double = AGAdaptiveTestForm.XminusAggressiveSet.Value
    Public bestyplus As Double = AGAdaptiveTestForm.YplusAggressiveSet.Value
    Public bestyminus As Double = AGAdaptiveTestForm.YminusAggressiveSet.Value

    Public agMaxPixel As Double

    Public SampleCount As Integer

    'Training:
    '   For each setting type,
    '       Increment up one (up) and test, then down one (center) and test, then down one more (down) and test
    '   Store the results in the data array as updates (not new data)

    Public Sub DelayTest()
        'Run delay testing
        '   Move up one notch (if possible) and test, then store result
        '   Move back to original positon and test, then store result
        '   Move down one notch (if possible) and test, then store result
        '   Reset to orginial position

        Dim lowresult As Double = -1
        Dim midresult As Double = -1
        Dim highresult As Double = -1

        Dim DACSet As Double = AGAdaptiveTestForm.DACSet.Value
        Dim DACmin As Double = AGAdaptiveTestForm.ShortestDACTime.Value
        Dim DACmax As Double = AGAdaptiveTestForm.LongestDACTime.Value
        Dim DACinc As Double = AGAdaptiveTestForm.DACStep.Value

        UploadSettings()
        midresult = RunTest()
        If DACSet < DACmax Then
            AGAdaptiveTestForm.DACSet.Value = (DACSet + DACinc)
            UploadSettings()
            highresult = RunTest()
        End If
        If DACSet > DACmin Then
            AGAdaptiveTestForm.DACSet.Value = (DACSet - DACinc)
            UploadSettings()
            lowresult = RunTest()
        End If
        AGAdaptiveTestForm.DACSet.Value = DACSet
        bestdelayaftercorrection = PickLowestPositiveValue(lowresult, midresult, highresult, DACSet - DACinc, DACSet, DACSet + DACinc)
        UploadSettings()
        Return
    End Sub

    Public Sub MinMoveTest()
        'Run mininum move testing
        '   Move up one notch (if possible) and test, then store result
        '   Move back to original positon and test, then store result
        '   Move down one notch (if possible) and test, then store result
        '   Reset to orginial position
        Dim lowresult As Double = -1
        Dim midresult As Double = -1
        Dim highresult As Double = -1

        Dim minmove = AGAdaptiveTestForm.MinimumMoveSet.Value
        UploadSettings()
        midresult = RunTest()
        If minmove < MaxMinMove Then
            AGAdaptiveTestForm.MinimumMoveSet.Value = minmove + MinMoveInc
            UploadSettings()
            highresult = RunTest()
        End If
        If minmove > MinMinMove Then
            AGAdaptiveTestForm.MinimumMoveSet.Value = minmove - MinMoveInc
            UploadSettings()
            lowresult = RunTest()
        End If
        AGAdaptiveTestForm.MinimumMoveSet.Value = minmove
        bestminmove = PickLowestPositiveValue(lowresult, midresult, highresult, minmove - MinMoveInc, minmove, minmove + MinMoveInc)
        Return
    End Sub

    Public Sub MaxMoveTest()
        'Run maximum move testing
        '   Move up one notch (if possible) and test, then store result
        '   Move back to original positon and test, then store result
        '   Move down one notch (if possible) and test, then store result
        '   Reset to orginial position
        Dim lowresult As Double = -1
        Dim midresult As Double = -1
        Dim highresult As Double = -1

        Dim maxmove = AGAdaptiveTestForm.MaximumMoveSet.Value
        UploadSettings()
        midresult = RunTest()
        If maxmove < MaxMaxMove Then
            AGAdaptiveTestForm.MaximumMoveSet.Value = maxmove + MaxMoveInc
            UploadSettings()
            highresult = RunTest()
        End If
        If maxmove > MinMaxMove Then
            AGAdaptiveTestForm.MaximumMoveSet.Value = maxmove - MaxMoveInc
            UploadSettings()
            lowresult = RunTest()
        End If
        AGAdaptiveTestForm.MaximumMoveSet.Value = maxmove
        bestmaxmove = PickLowestPositiveValue(lowresult, midresult, highresult, maxmove - MaxMoveInc, maxmove, maxmove + MaxMoveInc)
        Return
    End Sub

    Public Sub XPlusAggressivenessTest()
        'Run aggressiveness training
        '   Move up one notch (if possible) and test, then store result
        '   Move back to original positon and test, then store result
        '   Move down one notch (if possible) and test, then store result
        '   Reset to orginial position
        Dim lowresult As Double = -1
        Dim midresult As Double = -1
        Dim highresult As Double = -1

        Dim xpc = AGAdaptiveTestForm.XplusAggressiveSet.Value
        UploadSettings()
        midresult = RunTest()
        If xpc < MaxAggressiveness Then
            AGAdaptiveTestForm.XplusAggressiveSet.Value = xpc + 1
            UploadSettings()
            highresult = RunTest()
        End If
        If xpc > MinAggressiveness Then
            AGAdaptiveTestForm.XplusAggressiveSet.Value = xpc - 1
            UploadSettings()
            lowresult = RunTest()
        End If
        AGAdaptiveTestForm.XplusAggressiveSet.Value = xpc
        bestxplus = PickLowestPositiveValue(lowresult, midresult, highresult, xpc - 1, xpc, xpc + 1)
        Return
    End Sub

    Public Sub XMinusAggressivenessTest()
        'Run aggressiveness training
        'For each aggressiveness setting (X and Y),
        '   Move up one notch (if possible) and test, then store result
        '   Move back to original positon and test, then store result
        '   Move down one notch (if possible) and test, then store result
        '   Reset to orginial position
        Dim lowresult As Double = -1
        Dim midresult As Double = -1
        Dim highresult As Double = -1

        Dim xnc = AGAdaptiveTestForm.XminusAggressiveSet.Value
        UploadSettings()
        midresult = RunTest()

        If xnc < MaxAggressiveness Then
            AGAdaptiveTestForm.XminusAggressiveSet.Value = xnc + 1
            UploadSettings()
            highresult = RunTest()
        End If
        If xnc > MinAggressiveness Then
            AGAdaptiveTestForm.XminusAggressiveSet.Value = xnc - 1
            UploadSettings()
            lowresult = RunTest()
        End If
        AGAdaptiveTestForm.XminusAggressiveSet.Value = xnc
        bestxminus = PickLowestPositiveValue(lowresult, midresult, highresult, xnc - 1, xnc, xnc + 1)
        Return
    End Sub

    Public Sub YPlusAggressivenessTest()
        'Run aggressiveness training
        'For each aggressiveness setting (X and Y),
        '   Move up one notch (if possible) and test, then store result
        '   Move back to original positon and test, then store result
        '   Move down one notch (if possible) and test, then store result
        '   Reset to orginial position
        Dim lowresult As Double = -1
        Dim midresult As Double = -1
        Dim highresult As Double = -1

        Dim ypc = AGAdaptiveTestForm.YplusAggressiveSet.Value
        UploadSettings()
        midresult = RunTest()
        If ypc < MaxAggressiveness Then
            AGAdaptiveTestForm.YplusAggressiveSet.Value = ypc + 1
            UploadSettings()
            highresult = RunTest()
        End If
        If ypc > MinAggressiveness Then
            AGAdaptiveTestForm.YplusAggressiveSet.Value = ypc - 1
            UploadSettings()
            lowresult = RunTest()
        End If
        AGAdaptiveTestForm.YplusAggressiveSet.Value = ypc
        bestyplus = PickLowestPositiveValue(lowresult, midresult, highresult, ypc - 1, ypc, ypc + 1)
        Return
    End Sub

    Public Sub YMinusAggressivenessTest()
        'Run aggressiveness training
        'For each aggressiveness setting (X and Y),
        '   Move up one notch (if possible) and test, then store result
        '   Move back to original positon and test, then store result
        '   Move down one notch (if possible) and test, then store result
        '   Reset to orginial position
        Dim lowresult As Double = -1
        Dim midresult As Double = -1
        Dim highresult As Double = -1

        Dim ync = AGAdaptiveTestForm.YminusAggressiveSet.Value
        UploadSettings()
        midresult = RunTest()
        If ync < MaxAggressiveness Then
            AGAdaptiveTestForm.YminusAggressiveSet.Value = ync + 1
            UploadSettings()
            highresult = RunTest()
        End If
        If ync > MinAggressiveness Then
            AGAdaptiveTestForm.YminusAggressiveSet.Value = ync - 1
            UploadSettings()
            lowresult = RunTest()
        End If
        AGAdaptiveTestForm.YminusAggressiveSet.Value = ync
        bestyminus = PickLowestPositiveValue(lowresult, midresult, highresult, ync - 1, ync, ync + 1)
        Return
    End Sub

    Private Sub StopAG()
        'Abort autoguiding
        Dim tsx_ag = CreateObject("TheSky64.ccdsoftCamera")
        tsx_ag.AutoGuider = 1
        tsx_ag.Asynchronous = False
        tsx_ag.Abort()
        tsx_ag = Nothing
        Return
    End Sub

    Private Function RunTest() As Double
        Dim sampleresult = AGSample.SampleRun()
        'Check for abort, if so then exit
        If AGAdaptiveTestForm.agprofileabort Then
            Return (-1)
        Else
            'Store result here
            SaveResults(sampleresult, agMaxPixel)
        End If
        'Add the result to the Corrections Graph
        GraphCorrection(sampleresult)
        Return (sampleresult)
    End Function

    'Save sample results
    Private Sub SaveResults(sample, maxpixel)
        Dim sResult As String
        With AGAdaptiveTestForm.agsamples
            Dim aso = .MakeSampleSet()
            Dim ad = .FindEntry(aso)
            If ad Is Nothing Then
                ad = .AddEntry(aso, AGAdaptiveTestForm.SamplingNumber.Value, sample, maxpixel)
            Else
                .UpdateEntry(ad, AGAdaptiveTestForm.SamplingNumber.Value, sample, maxpixel)
            End If
            sResult = .StringSample(ad)
        End With
        AGAdaptiveTestForm.AGStatusStripLabel.Text = sResult
        AGAdaptiveTestForm.Show()
        Return
    End Sub

    Public Sub UploadSettings()
        'Set autoguider aggressiveness settings on the fly

        Dim tsx_ag = CreateObject("TheSky64.ccdsoftCamera")
        tsx_ag.AutoGuider = 1
        tsx_ag.Asynchronous = False

        tsx_ag.AutoguiderExposureTime = Convert.ToDouble(AGAdaptiveTestForm.ExposureSet.Value)
        tsx_ag.AutoguiderDelayAfterCorrection = Convert.ToDouble(AGAdaptiveTestForm.DACSet.Value)
        tsx_ag.AutoguiderMinimumMove = Convert.ToInt32(AGAdaptiveTestForm.MinimumMoveSet.Value * 1000)
        tsx_ag.AutoguiderMaximumMove = Convert.ToInt32(AGAdaptiveTestForm.MaximumMoveSet.Value * 1000)

        tsx_ag.SetPropLng("m_nXPlusAggressiveness", Convert.ToInt32(AGAdaptiveTestForm.XplusAggressiveSet.Value))
        tsx_ag.SetPropLng("m_nXMinusAggressiveness", Convert.ToInt32(AGAdaptiveTestForm.XminusAggressiveSet.Value))
        tsx_ag.SetPropLng("m_nYPlusAggressiveness", Convert.ToInt32(AGAdaptiveTestForm.YplusAggressiveSet.Value))
        tsx_ag.SetPropLng("m_nYMinusAggressiveness", Convert.ToInt32(AGAdaptiveTestForm.YminusAggressiveSet.Value))
        tsx_ag = Nothing
        Return

    End Sub

    Public Sub DownloadSettings()
        'Download autoguider settings from TSX
        Dim tsx_ag = CreateObject("TheSky64.ccdsoftCamera")
        tsx_ag.Autoguider = 1
        tsx_ag.Asynchronous = False

        AGAdaptiveTestForm.ExposureSet.Value = tsx_ag.AutoguiderExposureTime
        AGAdaptiveTestForm.DACSet.Value = tsx_ag.AutoguiderDelayAfterCorrection
        AGAdaptiveTestForm.MinimumMoveSet.Value = tsx_ag.AutoguiderMinimumMove / 1000
        AGAdaptiveTestForm.MaximumMoveSet.Value = tsx_ag.AutoguiderMaximumMove / 1000


        ' tsx_ag.PropDbl()  'double
        AGAdaptiveTestForm.XplusAggressiveSet.Value = tsx_ag.PropLng("m_nXPlusAggressiveness")  'integer
        AGAdaptiveTestForm.XminusAggressiveSet.Value = tsx_ag.PropLng("m_nXMinusAggressiveness")  'integer
        AGAdaptiveTestForm.YplusAggressiveSet.Value = tsx_ag.PropLng("m_nYPlusAggressiveness")  'integer
        AGAdaptiveTestForm.YminusAggressiveSet.Value = tsx_ag.PropLng("m_nYPlusAggressiveness")  'integer
        Return

    End Sub

    'Reset parameters to new optimal settings, if any
    Public Sub Optimize()
        'Get optimal values, set the new settings and display the best ones

        Dim bestset As AGData.AGSettingGroup = AGAdaptiveTestForm.agsamples.BestValues()

        AGAdaptiveTestForm.DACSet.Value = bestdelayaftercorrection
        AGAdaptiveTestForm.MinimumMoveSet.Value = bestminmove
        AGAdaptiveTestForm.MaximumMoveSet.Value = bestmaxmove
        AGAdaptiveTestForm.XplusAggressiveSet.Value = bestxplus
        AGAdaptiveTestForm.XminusAggressiveSet.Value = bestxminus
        AGAdaptiveTestForm.YplusAggressiveSet.Value = bestyplus
        AGAdaptiveTestForm.YminusAggressiveSet.Value = bestyminus

        AGAdaptiveTestForm.BestDelayLabel.Text = Str(bestdelayaftercorrection)
        AGAdaptiveTestForm.BestMinMoveLabel.Text = Str(bestminmove)
        AGAdaptiveTestForm.BestMaxMoveLabel.Text = Str(bestmaxmove)
        AGAdaptiveTestForm.BestPlusXLabel.Text = Str(bestxplus)
        AGAdaptiveTestForm.BestMinusXLabel.Text = Str(bestxminus)
        AGAdaptiveTestForm.BestPlusYLabel.Text = Str(bestyplus)
        AGAdaptiveTestForm.BestMinusYLabel.Text = Str(bestyminus)

        Return
    End Sub

    Private Function PickLowestPositiveValue(a, b, c, ar, br, cr) As Double
        'Determines lowest value (a,b,c) and returns the respective value (ar, br, cr
        '  Only a or c can be -1, not both, and b cannot be -1 (not verified in this routine)
        '  Returns br if no solution
        'Note that preference for "lowest" is given to a and c over b
        '

        If a < 0 Then 'both b and c must be positive
            If b <= c Then
                Return br
            Else
                Return cr
            End If
        End If
        If c < 0 Then 'both b and a must be positive
            If b <= a Then
                Return br
            Else
                Return ar
            End If
        End If
        'Neither a or c are less than zero (i.e. -1)
        If a <= b And a <= c Then
            Return ar
        End If
        If c <= a And c <= b Then
            Return cr
        End If
        Return br

    End Function

End Module