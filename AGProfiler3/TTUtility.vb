Imports System.IO
'Windows Visual Basic Class: AGUtility
'
'Culled down verion of TTUtilities module for use with the AutoGuiding Profile Application
'
' ------------------------------------------------------------------------
'
' Author: R.McAlister (2015)
' Version 1.0
'
' ------------------------------------------------------------------------
Public Class TTUtility
    'Common utilities for TSX connections - Culled for AGAnalysisSA (StandAlone)
    '
    '
    Public Shared Target_Name
    Public Shared Cam_Delay
    Public Shared Cam_Reduction
    Public Shared Cam_Filter
    Public Shared Cam_Exposure
    Public Shared AG_Reduction
    Public Shared AG_Exposure
    Public Shared AG_Delay
    Public Shared AG_MinMove
    Public Shared AG_Aggressiveness

    Public Shared Sub SaveTSXState()

        'Saves the current target and camera configuration information in TTutility public variables
        '   Target Name (for TSX Find)
        '   Camera Delay
        '   Camera Exposure
        '   Camera Image Reduction
        '   Camera Filter

        Dim tsx_oi = CreateObject("TheSkyX.Sky6ObjectInformation")
        tsx_oi.Property(theskyxLib.Sk6ObjectInformationProperty.sk6ObjInfoProp_NAME1)
        Target_Name = tsx_oi.ObjInfoPropOut

        Dim tsx_cc = CreateObject("TheSkyX.ccdsoftCamera")
        Cam_Delay = tsx_cc.Delay
        Cam_Reduction = tsx_cc.ImageReduction
        Cam_Filter = tsx_cc.FilterIndexZeroBased
        Cam_Exposure = tsx_cc.ExposureTime

        Dim tsx_ag = CreateObject("TheSkyX.ccdsoftCamera")
        tsx_ag.Autoguider = 1
        AG_Delay = tsx_ag.Delay
        AG_Reduction = tsx_ag.ImageReduction
        AG_Exposure = tsx_ag.ExposureTime
        AG_MinMove = tsx_ag.AutoguiderMinimumMove
        AG_Aggressiveness = tsx_ag.AutoguiderAggressiveness

        tsx_oi = Nothing
        tsx_cc = Nothing
        tsx_ag = Nothing

        Return
    End Sub


    Public Shared Sub RestoreTSXState(CLS_Flag As Boolean)

        'Clears the observing list, restores the current target and camera configuration information in TTutility public variables,
        '   Camera Delay
        '   Camera Exposure
        '   Camera Image Reduction
        '   Camera Filter

        'Open camera object
        Dim tsx_cc = CreateObject("TheSkyX.ccdsoftCamera")

        'Restore camera settings
        tsx_cc.ImageReduction = Cam_Reduction
        tsx_cc.FilterIndexZeroBased = Cam_Filter
        tsx_cc.ExposureTime = Cam_Exposure
        tsx_cc.Delay = Cam_Delay

        Dim tsx_ag = CreateObject("TheSkyX.ccdsoftCamera")
        tsx_ag.Autoguider = 1
        tsx_ag.Delay = AG_Delay
        tsx_ag.ImageReduction = AG_Reduction
        tsx_ag.ExposureTime = AG_Exposure
        tsx_ag.AutoguiderMinimumMove = AG_MinMove
        tsx_ag.AutoguiderAggressiveness = AG_Aggressiveness

        'Clean up
        tsx_cc = Nothing
        tsx_ag = Nothing
        Return

    End Sub

    Public Shared Sub LogIt(logline As String)
        'Write string to status strip
        AGAdaptiveTestForm.AGStatusStripLabel.Text = logline
        Return
    End Sub

    Public Shared Function DoubleClip(ByVal dvalue As Double, ByVal decimals As Integer) As String
        'Converts a double value (dvalue) to a string truncated to the specified number (decimals) of decimal points, adds a leading 0 if necessary
        Dim ss As String
        Dim decpos As Integer

        'Convert the value to a simple string without leading spaces
        ss = LTrim(Str(dvalue))
        decpos = InStr(ss, ".")
        Select decpos
            Case 0
                'No decimal point is found in string, e.g. xxxxx
                'Use all the characters
                'If the dvalue is 0, then add a decimal point and zero's accordingly
                If dvalue = 0 Then
                    Select Case decimals
                        Case 0
                            ss = "0"
                        Case Else
                            ss = "0."
                            For i = 1 To decimals
                                ss = ss + "0"
                            Next
                    End Select
                End If
            Case 1
                'Decimal point is found in numeric string at first character, e.g. .xxxx
                'Put a zero in front of the leftmost decimals characters + 1
                ss = "0" + Strings.Left(ss, decimals + 1)
            Case Else
                'Decimal point is found in numeric string after first character
                'Save everything to the left of the decimal point + number of decimals unless
                '  the decimals is 0, then move the decpos to the left one before 
                If decimals = 0 Then
                    decpos = decpos - 1
                End If
                ss = Strings.Left(ss, decpos + decimals)

        End Select
        Return ss

    End Function



End Class
