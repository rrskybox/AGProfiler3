Public Class AGData
    'This class encapulates properties and methods for storing and analyzing AG profile setting data

    Public Shared AGSampleData

    Public Class AGSettingGroup
        Property GExposure As Double
        Property GDAC As Double
        Property GMinMove As Double
        Property GMaxMove As Double
        Property GXplus As Integer
        Property GXminus As Integer
        Property GYplus As Integer
        Property GYminus As Integer
    End Class

    Public Class SettingSample
        Property GSettings As AGSettingGroup
        Property GCount As Integer
        Property GResult As Double
        Property GMaxPixel As Double
    End Class

    Public Sub New()
        'Create new instance of database arrays for the AGData --*** may have to do collection rather than array***
        'Each entry in the data array will contain the results of AG sampling in the following form
        '   Exposure, Delay After Correct, MinMove, MaxMove, Xpos, Xmin, Ypos, Ymin, SampleCnt, TotalRMS stored as strings
        '   Exposure (AGData(0,x...) will be 0 if no sample has been taken, -1 if skipped or invalidated for some reason
        'Each entry in the sampling array will contain the sampling sequence generated for the run

        'Data form for each entry is string, double, integer (sample settings, sample count, sample result)

        AGSampleData = New List(Of SettingSample)
        Return

    End Sub

    '* * * FINDSAMPLE:  Find the index of a sample, if any, for a given set of settings.  (Integer)
    Public Function FindEntry(tgtentry As AGSettingGroup) As SettingSample
        'Search the AGData for any entry matching the parameters    
        '   Return -1 if no sample found
        For Each entry In AGSampleData
            If String.Equals(StringSettings(tgtentry), StringSettings(entry.GSettings)) Then
                Return entry
            End If
        Next
        Return Nothing
    End Function

    '* * * LINESAMPLE: Create a string that contains the whole entry with sample result adjusted for count, laid out in lines
    Function LineSample(ags As SettingSample) As String
        Dim agro = ags.GSettings
        'Fix exposure and delay after correctto single decimal point 
        Dim adjresult As Double = ags.GResult / (ags.GCount / AGAdaptiveTestForm.SamplingNumber.Value)
        Dim exstr As String = _
            "   Exposure = " + Convert.ToString(Int(agro.GExposure * 10) / 10) + " sec  " + vbCrLf +
            "   Delay = " + Convert.ToString(Int(agro.GDAC * 10) / 10) + " sec  " + vbCrLf +
            "   Min Move = " + Str(agro.GMinMove) + vbCrLf +
            "   Max Move = " + Str(agro.GMaxMove) + vbCrLf +
            "     + X = " + Str(agro.GXplus) +
            "     - X = " + Str(agro.GXminus) + vbCrLf +
            "     + Y = " + Str(agro.GYplus) +
            "     - Y = " + Str(agro.GYminus) + vbCrLf +
            "   ->  " + Convert.ToString(Int(adjresult * 100) / 100) + " XY RMS for " + Str(ags.GCount) + " samples"
        Return (exstr)
    End Function

    '* * * STRINGSAMPLE: Create a string that contains the whole entry with sample result adjusted for count
    Function StringSample(ags As SettingSample) As String
        Dim agro = ags.GSettings
        'Fix exposure to single decimal point
        Dim adjresult As Double = ags.GResult / (ags.GCount / AGAdaptiveTestForm.SamplingNumber.Value)
        Dim exstr As String = "Last Sample -> Exposure = " + Convert.ToString(Int(agro.GExposure * 10) / 10) + " sec  " +
            "   Delay = " + Convert.ToString(Int(agro.GDAC * 10) / 10) + " sec  " +
            "   Min Move = " + Str(agro.GMinMove) +
            "   Max Move = " + Str(agro.GMaxMove) +
            "     + X = " + Str(agro.GXplus) +
            "     - X = " + Str(agro.GXminus) +
            "     + Y = " + Str(agro.GYplus) +
            "     - Y = " + Str(agro.GYminus)
        Return exstr
    End Function

    '* * * STRINGSETTINGS: Create a string that contains the setting parameters with result
    Function StringSettings(agro As AGSettingGroup) As String
        'Fix exposure to single decimal point
        Dim exstr As String = _
            Convert.ToString(Int(agro.GDAC * 10) / 10) + " " +
            Str(agro.GMinMove) + " " + Str(agro.GMaxMove) + " " +
            Str(agro.GXplus) + " " + Str(agro.GXminus) + " " + Str(agro.GYplus) + " " + Str(agro.GYminus)
        Return (exstr)
    End Function

    '* * * ADDENTRY: Add a new entry to the sample database
    Public Function AddEntry(samplesettings As AGSettingGroup, samplecount As Integer, result As Double, maxpixel As Double) As SettingSample
        'Assumes that FindSample has been run such that no other entry for the header exists
        Dim ad As New SettingSample
        ad.GSettings = samplesettings
        ad.GCount = samplecount
        ad.GResult = result
        ad.GMaxPixel = maxpixel
        AGSampleData.Add(ad)
        Return (ad)
    End Function

    '* * * UPDATEENTRY: Update entry in the sample database (return settingsample)
    Public Sub UpdateEntry(ByRef ss As SettingSample, samplecount As Integer, sampledata As Double, maxpixel As Double)
        'Assumes that FindSample has found this 
        ss.GCount = ss.GCount + samplecount
        ss.GResult = ss.GResult + sampledata
        ss.GMaxPixel = maxpixel
        Return
    End Sub

    Public Function MakeSampleSet() As AGSettingGroup
        'Parses a sample header for each of the settings
        Dim aggroup As New AGSettingGroup
        aggroup.GExposure = AGAdaptiveTestForm.ExposureSet.Value
        aggroup.GDAC = AGAdaptiveTestForm.DACSet.Value
        aggroup.GMinMove = AGAdaptiveTestForm.MinimumMoveSet.Value
        aggroup.GMaxMove = AGAdaptiveTestForm.MaximumMoveSet.Value
        aggroup.GXplus = AGAdaptiveTestForm.XplusAggressiveSet.Value
        aggroup.GXminus = AGAdaptiveTestForm.XminusAggressiveSet.Value
        aggroup.GYplus = AGAdaptiveTestForm.YplusAggressiveSet.Value
        aggroup.GYminus = AGAdaptiveTestForm.YminusAggressiveSet.Value
        Return (aggroup)
    End Function

    Public Function BestValues() As AGSettingGroup
        'Determines optimal configuration of settings based on current results
        Dim thisresult As Double
        Dim bestentry As SettingSample = Nothing
        Dim bestset = New AGSettingGroup
        Dim bestresult As Double = 1000
        For Each entry In AGSampleData
            thisresult = entry.Gresult / (entry.GCount / AGAdaptiveTestForm.SamplingNumber.Value)
            If thisresult < bestresult Then
                bestresult = thisresult
                bestset = entry.gsettings
                bestentry = entry
            End If
        Next
        AGAdaptiveTestForm.BestRMSLabel.Text = "Lowest XY Error RMS: " + vbCrLf + LineSample(bestentry)
        Return (bestset)
    End Function

End Class
