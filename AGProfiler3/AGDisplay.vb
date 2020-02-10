Imports System.Windows.Forms.DataVisualization.Charting

Module AGDisplay
    'Methods for displaying autoguide information on graphs

    Public Sub ClearGraphs()
        AGAdaptiveTestForm.CorrectionChart.Series("CorrectionSeries").Points.Clear()
        AGAdaptiveTestForm.DelayChart.Series("DelaySeries").Points.Clear()
        AGAdaptiveTestForm.MinMoveChart.Series("MinMoveSeries").Points.Clear()
        AGAdaptiveTestForm.MaxMoveChart.Series("MaxMoveSeries").Points.Clear()
        AGAdaptiveTestForm.XPlusChart.Series("XPlusSeries").Points.Clear()
        AGAdaptiveTestForm.XMinusChart.Series("XMinusSeries").Points.Clear()
        AGAdaptiveTestForm.YPlusChart.Series("YPlusSeries").Points.Clear()
        AGAdaptiveTestForm.YMinusChart.Series("YMinusSeries").Points.Clear()
        Return
    End Sub

    Public Sub GraphAll()
        GraphDelay()
        GraphMinMove()
        GraphMaxMove()
        GraphXPlus()
        GraphXMinus()
        GraphYPlus()
        GraphYMinus()
        Return
    End Sub

    Public Sub GraphCorrection(result)
        'Append the result to the set of y data points assuming the x is incremental
        AGAdaptiveTestForm.CorrectionChart.Series("CorrectionSeries").Points.AddY(result)
        Application.DoEvents()
        AGAdaptiveTestForm.Show()
        Return
    End Sub

    Public Sub GraphDelay()
        Dim avgResult
        For Each entry In AGData.AGSampleData
            avgResult = entry.Gresult / (entry.GCount / AGAdaptiveTestForm.SamplingNumber.Value)
            Dim cycle = Int((entry.Gsettings.GDAC + entry.Gsettings.GExposure) * 10) / 10
            AGAdaptiveTestForm.DelayChart.Series("DelaySeries").Points.AddXY(cycle, avgResult)
        Next
        Application.DoEvents()
        AGAdaptiveTestForm.Show()
        Return
    End Sub

    Public Sub GraphMinMove()
        Dim avgResult
        For Each entry In AGData.AGSampleData
            avgResult = entry.Gresult / (entry.GCount / AGAdaptiveTestForm.SamplingNumber.Value)
            AGAdaptiveTestForm.MinMoveChart.Series("MinMoveSeries").Points.AddXY(entry.Gsettings.GMinMove, avgResult)
        Next

        Application.DoEvents()
        AGAdaptiveTestForm.Show()
        Return
    End Sub

    Public Sub GraphMaxMove()
        Dim avgResult
        For Each entry In AGData.AGSampleData
            avgResult = entry.Gresult / (entry.GCount / AGAdaptiveTestForm.SamplingNumber.Value)
            AGAdaptiveTestForm.MaxMoveChart.Series("MaxMoveSeries").Points.AddXY(entry.Gsettings.GMaxMove, avgResult)
        Next
        Application.DoEvents()
        AGAdaptiveTestForm.Show()
        Return
    End Sub

    Public Sub GraphXPlus()
        Dim avgResult
        For Each entry In AGData.AGSampleData
            avgResult = entry.Gresult / (entry.GCount / AGAdaptiveTestForm.SamplingNumber.Value)
            AGAdaptiveTestForm.XPlusChart.Series("XPlusSeries").Points.AddXY(entry.Gsettings.GXPlus, avgResult)
        Next
        Application.DoEvents()
        AGAdaptiveTestForm.Show()
        Return
    End Sub

    Public Sub GraphXMinus()
        Dim avgResult
        For Each entry In AGData.AGSampleData
            avgResult = entry.Gresult / (entry.GCount / AGAdaptiveTestForm.SamplingNumber.Value)
            AGAdaptiveTestForm.XMinusChart.Series("XMinusSeries").Points.AddXY(entry.Gsettings.GXMinus, avgResult)
        Next
        Application.DoEvents()
        AGAdaptiveTestForm.Show()
        Return
    End Sub

    Public Sub GraphYPlus()
        Dim avgResult
        For Each entry In AGData.AGSampleData
            avgResult = entry.Gresult / (entry.GCount / AGAdaptiveTestForm.SamplingNumber.Value)
            AGAdaptiveTestForm.YPlusChart.Series("YPlusSeries").Points.AddXY(entry.Gsettings.GYPlus, avgResult)
        Next
        Application.DoEvents()
        AGAdaptiveTestForm.Show()
        Return
    End Sub

    Public Sub GraphYMinus()
        Dim avgResult
        For Each entry In AGData.AGSampleData
            avgResult = entry.Gresult / (entry.GCount / AGAdaptiveTestForm.SamplingNumber.Value)
            AGAdaptiveTestForm.YMinusChart.Series("YMinusSeries").Points.AddXY(entry.Gsettings.GYMinus, avgResult)
        Next
        Application.DoEvents()
        AGAdaptiveTestForm.Show()
        Return
    End Sub


End Module