Public Class AGOps
    'Routines for determining appropriate exposure level for autoguide star, returns derived exposure time

    Public Shared Function ManageExposure(tgtADU As Double, exposure As Double) As Double
        'Get the best exposure time based on the target ADU
        '
        'Subrountine loops up to 10 times, taking an image, and calulating a new exposure that would meet
        '   the ADU goal
        'If the returned exposure is very small, the star was probably saturated, so reduce by 1/2 and do again
        'If the returned exposure is very large, the star was probably severely underexposed, or lost, so increase by 2 and do again
        'If just right, then update the exposure settings and return
        '
        Dim trycount As Integer = 0
        TTUtility.LogIt("Determining correct guide star exposure")
        Do
            Dim exp = AGOps.CalibrateExposure(tgtADU, exposure, 0)
            If exp < 0.1 Then
                AGAdaptiveTestForm.ExposureSet.Value = AGAdaptiveTestForm.ExposureSet.Value / 2
                trycount += 1
            ElseIf exp > 10 Then
                AGAdaptiveTestForm.ExposureSet.Value = AGAdaptiveTestForm.ExposureSet.Value * 2
                trycount += 1
            Else
                TTUtility.LogIt("Guide Star Target Exposure = " + TTUtility.DoubleClip(exp, 2) + " secs")
                Return exp
            End If
        Loop Until trycount >= 10
        TTUtility.LogIt("Guide Star Exposure Calibration Failed")
        Return exposure
    End Function

    '*** CalibrateExposure determines exposure level that returns star maximum pixels at target ADU
    Public Shared Function CalibrateExposure(targetADU As Double, exposure As Double, delay As Double) As Double
        'Take AG image
        'Get current exposure level
        'Get maximum pixels ADU
        'Calculate correct exposure to get target max pixels

        Dim tag = CreateObject("TheSkyX.ccdsoftCamera")
        tag.Autoguider = 1
        tag.Subframe = True

        AGSingleImage(exposure, delay)
        Dim targetExposure = targetADU * (tag.ExposureTime / tag.MaximumPixel)
        tag = Nothing
        Return targetExposure

    End Function

    '*** SingleImage takes one autoguider image with whatever settings are already set
    Public Shared Sub AGSingleImage(exposure As Double, delay As Double)
        Dim tag = CreateObject("TheSkyX.ccdsoftCamera")
        tag.Autoguider = 1
        tag.Abort()
        'Wait a couple of seconds for it to clear
        System.Threading.Thread.Sleep(2000)
        Dim iasave = tag.Asynchronous
        Dim iesave = tag.ExposureTime

        tag.Asynchronous = False
        tag.ExposureTime = exposure

        Dim ierr = tag.TakeImage()
        'Save error processing for some other rev

        'Return image asynchronous to whatever it was
        tag.Asynchronous = iasave
        tag.ExposureTime = iesave

        tag = Nothing
        Return
    End Sub

    Public Shared Sub ManageFindStar()
        Dim mtstat
        Dim tsx_mt = CreateObject("TheSkyX.sky6RASCOMtele")
        Try
            mtstat = tsx_mt.Connect()
        Catch ex As Exception
            MsgBox("Mount connect error: " + ex.Message)
            tsx_mt = Nothing
            Return
        End Try
        'Select guide star
        Dim setstarstat = AGOps.SetAutoGuideStar()

        tsx_mt = Nothing
        Return
    End Sub

    Public Shared Function SetAutoGuideStar() As Boolean
        ' Subroutine picks a guide star, computes a subframe to put around it,
        ' then loads the location and subframe into the autoguider
        '
        Const MagWeight = 1
        Const FWHMWeight = 1
        Const ElpWeight = -1
        Const ClsWeight = 1


        'Algorithm:
        '
        '   Compute optimality and normalizaton values (see below) 
        '   Eliminate all points near edge and with neighbors
        '   Compute optimality differential and normalize, and add
        '   Select best (least sum) point
        '
        'Normalized deviation from optimal where optimal is the best value for each of the four catagories:
        '   Magnitude optimal is lowest magnitude
        '   FWHM optimal is average FWHM
        '   Ellipticity optimal is lowest ellipticity
        '   Class optimal is lowest class
        '
        'Normalized means adjusted against the range of highest to lowest becomes 1 to 0, unless there is only one datapoint
        '
        '
        TTUtility.LogIt("Autoguider: Picking guide star")
        Dim tsx_img = CreateObject("TheSkyX.ccdsoftImage")
        Dim tsx_agr = CreateObject("TheSkyX.ccdsoftCamera")
        tsx_agr.Autoguider = 1

        'take image
        tsx_agr.Asynchronous = False
        tsx_agr.Abort()
        'Wait a second, things don't always get cleared before an "Abort" return
        System.Threading.Thread.Sleep(2000)
        '.AutoguiderExposureTime = Convert.ToDouble(AGAnalysisForm.ExposureSet.Value)
        tsx_agr.ExposureTime = Convert.ToDouble(AGAdaptiveTestForm.ExposureSet.Value)

        tsx_agr.Subframe = False
        tsx_agr.AutosaveOn = True
        Dim ierr = tsx_agr.TakeImage()
        tsx_agr.AutosaveOn = False

        Dim TrackBoxSize = tsx_agr.TrackBoxX

        'Open the active image, if any
        Dim imgerr As Integer = 0

        Try
            imgerr = tsx_img.AttachToActiveAutoGuider()
        Catch ex As Exception
            'Just close up, TSX will spawn error window
            TTUtility.LogIt("Some problem with guider image: " + ex.Message)
            tsx_img = Nothing
            Return False
        End Try

        Try
            imgerr = tsx_img.ShowInventory()
        Catch ex As Exception
            TTUtility.LogIt("Astrometric error: " + ex.Message)
            tsx_img = Nothing
            Return False
        End Try

        Dim Xsize = tsx_img.WidthInPixels()
        Dim Ysize = tsx_img.HeightInPixels()

        'Collect astrometric light source data from the image linking into single index arrays: 
        '  magnitude, fmhm, ellipsicity, x and y positionc
        Dim MagArr As Object() = tsx_img.InventoryArray(ccdsoftInventoryIndex.cdInventoryMagnitude)
        Dim FWHMArr As Object() = tsx_img.InventoryArray(ccdsoftInventoryIndex.cdInventoryFWHM)
        Dim XPosArr As Object() = tsx_img.InventoryArray(ccdsoftInventoryIndex.cdInventoryX)
        Dim YPosArr As Object() = tsx_img.InventoryArray(ccdsoftInventoryIndex.cdInventoryY)
        Dim ElpArr As Object() = tsx_img.InventoryArray(ccdsoftInventoryIndex.cdInventoryEllipticity)
        Dim ClsArr As Object() = tsx_img.InventoryArray(ccdsoftInventoryIndex.cdInventoryClass)

        If MagArr.Length = 0 Then
            TTUtility.LogIt("No astrometric sources found")
            tsx_img = Nothing
            Return False
        End If

        'Get some useful statistics
        ' Max and min magnitude
        ' Max and min FWHM
        ' Max and min ellipticity
        ' max and min class
        ' Average FWHM

        Dim maxMag As Double = MagArr(0)
        Dim minMag As Double = MagArr(0)
        Dim maxFWHM As Double = FWHMArr(0)
        Dim minFWHM As Double = FWHMArr(0)
        Dim maxElp As Double = ElpArr(0)
        Dim minElp As Double = ElpArr(0)
        Dim maxCls As Double = ClsArr(0)
        Dim minCls As Double = ClsArr(0)

        Dim AvgFWHM As Double = 0

        For i = 0 To MagArr.Length - 1
            If MagArr(i) < minMag Then
                minMag = MagArr(i)
            End If
            If MagArr(i) > maxMag Then
                maxMag = MagArr(i)
            End If
            AvgFWHM += FWHMArr(i)
            If FWHMArr(i) < minFWHM Then
                minFWHM = FWHMArr(i)
            End If
            If FWHMArr(i) > maxFWHM Then
                maxFWHM = FWHMArr(i)
            End If
            If ElpArr(i) < minElp Then
                minElp = ElpArr(i)
            End If
            If ElpArr(i) > maxElp Then
                maxElp = ElpArr(i)
            End If
            If ClsArr(i) < minCls Then
                minCls = ClsArr(i)
            End If
            If ClsArr(i) > maxCls Then
                maxCls = ClsArr(i)
            End If
        Next

        AvgFWHM = AvgFWHM / FWHMArr.Length

        'Create a set of "best" values
        Dim optMag = minMag         'Magnitudes increase with negative values
        Dim optFWHM = AvgFWHM       'Looking for the closest to average FWHM
        Dim optElp = minElp         'Want the minimum amount of elongation
        Dim optCls = maxCls         '1 = star,0 = galaxy
        'Create a set of ranges
        Dim rangeMag = maxMag - minMag
        Dim rangeFWHM = maxFWHM - minFWHM
        Dim rangeElp = maxElp - minElp
        Dim rangeCls = maxCls - minCls
        'Create interrum variables for weights
        Dim normMag As Double
        Dim normFWHM As Double
        Dim normElp As Double
        Dim normCls As Double
        'Count keepers for statistics
        Dim SourceCount As Integer = 0
        Dim EdgeCount As Integer = 0
        Dim NeighborCount As Integer = 0

        'Create a selection array to store normilized and summed difference values
        Dim SelectArr(MagArr.Length - 1)

        'Convert all points to normalized differences, checking for zero ranges (e.g.single or identical data points)
        For i = 0 To MagArr.Length - 1
            If rangeMag <> 0 Then
                normMag = 1 - Math.Abs(optMag - MagArr(i)) / rangeMag
            Else
                normMag = 0
            End If
            If rangeFWHM <> 0 Then
                normFWHM = 1 - Math.Abs(optFWHM - FWHMArr(i)) / rangeFWHM
            Else
                normFWHM = 0
            End If
            If rangeElp <> 0 Then
                normElp = 1 - Math.Abs(optElp - ElpArr(i)) / rangeElp
            Else
                normElp = 0
            End If
            If rangeCls <> 0 Then
                normCls = 1 - Math.Abs(optCls - ClsArr(i)) / rangeCls
            Else
                normCls = 0
            End If

            'Sum the normalized points, weight and store value
            SelectArr(i) = normMag * MagWeight + normFWHM * FWHMWeight + normElp * ElpWeight + normCls * ClsWeight
            SourceCount += 1

            'Remove neighbors and edge liers
            If IsOnEdge(XPosArr(i), YPosArr(i), Xsize, Ysize, TrackBoxSize) Then
                SelectArr(i) = -1
            Else
                For j = i + 1 To SelectArr.Length - 2
                    If IsNeighbor(XPosArr(i), YPosArr(i), XPosArr(j), YPosArr(j), TrackBoxSize) Then
                        SelectArr(i) = -2
                    End If
                Next
            End If
        Next

        'Now find the best remaining entry

        Dim bestOne As Integer = 0
        For i = 0 To SelectArr.Length - 1
            If SelectArr(i) > SelectArr(bestOne) Then
                bestOne = i
            End If
        Next

        tsx_agr.GuideStarX = XPosArr(bestOne) * tsx_agr.BinX
        tsx_agr.GuideStarY = YPosArr(bestOne) * tsx_agr.BinY

        tsx_agr.SubframeLeft = (XPosArr(bestOne) - (TrackBoxSize / 2)) * tsx_agr.BinX
        tsx_agr.SubframeRight = (XPosArr(bestOne) + (TrackBoxSize / 2)) * tsx_agr.BinX
        tsx_agr.SubframeTop = (YPosArr(bestOne) - (TrackBoxSize / 2)) * tsx_agr.BinY
        tsx_agr.SubframeBottom = (YPosArr(bestOne) + (TrackBoxSize / 2)) * tsx_agr.BinY

        If SelectArr(bestOne) <> -1 Then
            tsx_agr.Subframe = True
            TTUtility.LogIt("Autoguider: Guide star set")
            tsx_img = Nothing
            tsx_agr = Nothing
            Return True
        Else
            'run statistics -- only if total failure
            For i = 0 To SourceCount - 1
                If SelectArr(i) = -1 Then
                    EdgeCount += 1
                End If
                If SelectArr(i) = -2 Then
                    NeighborCount += 1
                End If
            Next
            TTUtility.LogIt("Autoguider: No Guide star found out of " +
                            Str(SourceCount) + " stars, " +
                            Str(NeighborCount) + " had neighbors and " +
                            Str(EdgeCount) + " were on the edge.")
            tsx_img = Nothing
            tsx_agr = Nothing
            Return False
        End If


    End Function

    Private Shared Function IsOnEdge(Xpos As Integer, Ypos As Integer, Xsize As Integer, Ysize As Integer, border As Integer) As Boolean
        If (Xpos - border > 0) And _
            (Xpos + border < Xsize) And _
            (Ypos - border > 0) And _
            (Ypos + border) < Ysize Then
            Return False
        Else
            Return True
        End If
    End Function

    Private Shared Function IsNeighbor(Xpos1 As Integer, Ypos1 As Integer, Xpos2 As Integer, Ypos2 As Integer, subsize As Integer) As Boolean
        Dim limit As Integer = subsize / 2
        If (Math.Abs(Xpos1 - Xpos2) >= limit) Or _
            (Math.Abs(Ypos1 - Ypos2) >= limit) Then
            Return False
        Else
            Return True
        End If
    End Function

    'TurnAround:  Determines minimum amount of time for autoguider to take and process image at current exposure, binning and subframe -> seconds
    Public Shared Function TurnAround() As Double
        Dim starttime As DateTime
        Dim endtime As DateTime
        Dim duration As TimeSpan

        Dim tag = CreateObject("TheSkyX.ccdsoftCamera")
        tag.Autoguider = 1
        tag.Abort()
        'wait two seconds for the abort to clear
        System.Threading.Thread.Sleep(2000)
        Dim esave = tag.ExposureTime
        Dim iasave = tag.Asynchronous
        tag.Asynchronous = False
        tag.ExposureTime = 1 / 1000 '10 milliseconds
        starttime = Now()
        Dim ierr = tag.TakeImage()
        endtime = Now()
        duration = endtime - starttime
        tag.ExposureTime = esave
        tag.Asynchronous = iasave
        tag = Nothing
        Return (duration.TotalSeconds)
  
    End Function

    Public Shared Sub StartAG(exposure As Double)
        'Starts autoguiding if autoguiding isn't currently underway.  Exposure in milliseconds.
        Dim tsx_ag = CreateObject("TheSkyX.ccdsoftCamera")
        tsx_ag.AutoGuider = 1
        tsx_ag.Asynchronous = True
        If tsx_ag.State <> TheSkyXLib.ccdsoftCameraState.cdStateAutoGuide Then
            tsx_ag.AutoguiderExposureTime = exposure
            tsx_ag.Autoguide()
            'Wait 3 exposure+delays for the autoguider to get up and going
            System.Threading.Thread.Sleep((tsx_ag.AutoguiderExposureTime + tsx_ag.AutoguiderDelayAfterCorrection) * 3 * 1000)
            TTUtility.LogIt("Autoguiding started")
        Else
            TTUtility.LogIt("Autoguiding already running")
        End If
        tsx_ag = Nothing
        Return
    End Sub

    Public Shared Function WaitAG() As Boolean
        'Checks to see if Autoguide is running, if not sleep for a second and check again
        'If the abortflag gets set than return false
        'Otherwise return true
        Dim tsx_ag = CreateObject("TheSkyX.ccdsoftCamera")
        tsx_ag.AutoGuider = 1

        Do While tsx_ag.State <> TheSkyXLib.ccdsoftCameraState.cdStateAutoGuide
            If Not AGAdaptiveTestForm.agprofileabort Then
                Application.DoEvents()
                TTUtility.LogIt("Waiting for autoguiding to be turned on.")
                System.Threading.Thread.Sleep(1000)
            Else
                tsx_ag = Nothing
                Return False
            End If
        Loop
        tsx_ag = Nothing
        Return True

    End Function



End Class
