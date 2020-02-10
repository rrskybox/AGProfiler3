Public Class AGSample

    'Routines common to sampling and running Autoguide operation

    Public Shared Function SampleRun() As Double
        'Starts accumulating data for the number of samplecounts, and returns the averaged (mean) result
        '   currentdelayaftercorrect is in milliseconds
        '   currentdownloadtime is in milliseconds
        '   currentexposuretime is in milliseconds

        'Add a bit to the sampling time to deal with internal delays.  
        '   In a lossless system, 1.05 would drift the sampling over a single period for twenty samples

        Dim sampledelayaftercorrect As Double = AGAdaptiveTestForm.DACSet.Value
        Dim samplecount As Double = AGAdaptiveTestForm.SamplingNumber.Value
        Dim sampleexposure As Double = AGAdaptiveTestForm.ExposureSet.Value

        Dim sampleRMS As Double = 0
        Dim tsx_ag = CreateObject("TheSkyX.ccdsoftCamera")
        tsx_ag.Autoguider = 1

        'Wait for the autoguider isto be running, if not
        If Not AGOps.WaitAG() Then
            tsx_ag = Nothing
            Return (0)
        End If
    
        For repetitions = 1 To samplecount
            Application.DoEvents()
            If AGAdaptiveTestForm.agprofileabort Then
                'tsx_ag.Abort()
                tsx_ag = Nothing
                Return (0)
            End If
            System.Threading.Thread.Sleep(((sampleexposure + sampledelayaftercorrect) + AGAdaptiveTestForm.minimumcycletime) * 1000)
            sampleRMS += (tsx_ag.GuideErrorX ^ 2 + tsx_ag.GuideErrorY ^ 2)
            agMaxPixel = (tsx_ag.MaximumPixel)
            AGAdaptiveTestForm.MaxPixelLabel.Text = "Maximum ADU: " + Convert.ToString(agMaxPixel)
        Next
        tsx_ag = Nothing
        tsx_ag = Nothing
        Return Math.Sqrt(sampleRMS / samplecount)

    End Function

  
End Class
