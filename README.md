# AGProfiler3
Autoguiding performance profiling using TheSkyX

AutoGuide Profiler 3						Rick McAlister, 11/26/16

Overview: 

AutoGuide Profiler3 presents a two-step process for optimizing the performance of astronomical autoguiding under TSX.  The first step is a comprehensive Scan Mode for autoguiding performance profiling.   In scan mode, the application exercises most combinations of basic autoguider settings through their full ranges.  The result is a comprehensive map of autoguiding accuracy with respect to the primary TSX settings.  In the second step, Adaptive Mode, the application runs an adaptive algorithm in which autoguiding accuracy is tested and settings continuously adjusted for improvement. This mode starts with initial setting values, then tests autoguiding accuracy as those values are incrementally and repetitively adjusted towards an optimum.  The initial settings are normally, but not necessarily, acquired via the Scan Mode.  The application can continue to run in Adaptive Mode concurrently with most other imaging applications to adjust the autoguiding settings for changes in seeing, etc. 
 
One major enhancement in this version is in the image exposure testing.  The previous versions tested incremental changes to the length of exposure to see the effect of changing the autoguiding cycle time on tracking behavior.   Cycle time is an important factor when dealing with poor seeing for instance.  Too rapid a cycle merely chases the scintillation and can result in a marked worsening in performance.  However, changing the exposure time can also result in saturation or loss of guide star (too dim).  AutoProfiler3 no longer varies the exposure time.  Instead, the Delay After Correction setting is incrementally varied to test the effect of longer cycle times.  On other outcome of using Delay After Correction is that Autoguide Exposure Time cannot be changed on the fly, whereas Delay After Correction can.  Previous versions of the test application had to abort and restart autoguiding to set a new exposure length.  This meant that AutoGuide Profiler could interfere with other control applications.  In this version, Adaptive Mode testing is entirely passive with respect to other programs.  That is, autoguiding can be turned on and off without interference.  Adaptive Mode merely waits until Autoguiding is running to measure and modify settings.

Lastly, AutoGuide Profiler 3 adds two new functions:   automatic selection of a guide star, and automatic generation of a suitable exposure for imaging during the tests, static or adaptive, based on target ADU.

Scan Mode Testing

The Autoguider Scan Testing runs the TSX autoguider through a set of defined settings and compares the results in terms of average guiding error.  The user can set minimum and maximum constraints on delay, aggressiveness and minimum movement settings.  All combinations of aggressiveness and delay settings are run for a fixed number of samples, then the minimum move setting values are run against the best aggressiveness and delay settings.  The scan results for best exposure, aggressiveness, delay and minimum move are set as starting points for the main program.  Note that the aggressiveness setting is applied to all X and Y aggressiveness settings equally.  The main routine, described above, will start with those equal values then refine them individually.

Adaptive Mode Testing

The Autoguider Adaptive Testing exercises TSX autoguiding through a range of delay-after-correction, aggressiveness, minimum and maximum move settings.  The user sets the initial autoguiding parameters, either directly, downloaded from TSX, and/or determined by running the Autoguider Scan Test.  Once started, the program turns on autoguiding and samples X and Y tracking errors.  After each sampling period, the mean result is saved and each setting, one at a time, is adjusted up then down by one increment and the XY error resampled.  After all settings have been adjusted and sampled in this way, the autoguider is reset to the best of the three positions for each, then sampling tests repeated.  

For instance, if a parameter was set to 5, the program tests that parameter for values 4, 5 and 6.  If the value of 4 is determined to provide the most precise autoguiding of the three, then in the next loop, the program will try 3, 4 and 5.  If 3 has the best results of the three settings, then the program will test 2, 3 and 4, and so on.  If the seeing remains consistent over the tests, then (theoretically) the routine will eventually end up looping at one set of values.  The testing will simply remain there (or close) while continuing to test that value and both sides, one higher and one lower, all night.  If the conditions improve or deteriorate, then the routine should follow over time.  

Results are graphed for each setting as the tests take place.  Each graph is colored red as that specific setting is being tested.  A status line on the bottom indicates the parameters and results of the most recently completed test.  Information as to the value of the maximum ADU for the selected target and current overall setting group and XY error average are displayed to the left of the graphs.  Note that the current autoguider settings, as displayed in the setting boxes, may or may not reflect this current overall best as adjustment to individual settings may still be converging on an improved set of values.

Over many sampling loops, the setting group will evolve towards the values that produce the minimum tracking errors.  Not all combinations may be exercised, but not all combinations work well anyway.  If there is a sweet spot, then this profiler should find it.  The progress can be monitor visually on the Avg Guide Error Over Time graph on the upper left.  This graph consecutively displays the result of each successive test.  Under perfect conditions, this plot should converge, then hover on a sub-pixel reading.  The user can thereby judge when the evaluation is essentially finished.

Optionally, the profiler can continue to run during an imaging session and help adapt to changing seeing conditions.  Theoretically, all settings are changed on the fly without the need to interrupt Autoguiding.  However, if a control program starts and stops autoguiding periodically then there is a chance for some confusion and unpredictable results.

Adaptive Mode Commands:

Scan Test:  Open the autoguider scan window to completely scan a full spectrum of aggressiveness, minimum move and delay settings.  

Get Settings from TSX and Send Settings to TSX: Download and upload settings to TSX.

Find Star:  Picks an appropriate guide star for the profiling.   The algorithm uses TSX light source inventorying to avoid hot pixels, saturated stars, near-border stars and multiple stars in a subframe.  Alternatively, the guide star can be set within TSX prior to training using TSX’s built-in capability.

Set Exposure:  Adjusts the exposure time of the autoguider such that the guide star is imaged at a level near the target ADU.

Start AG: Starts up TSX Autoguiding function.

Adaptive Test: Starts the sampling and adjustment process, assuming Autoguiding has been started either by Start AG, in TSX, or by another control application, e.g. CCDAP.

Reset:  The sampling database can be cleared and profiling restarted without exiting the application.

Save: Stores the data as in a text file named AGAdaptiveProfile.txt in a folder of the user’s choosing.  If the user cancels out of the folder selection, no file is saved.

Close: The application is terminated.

Scan Mode Commands:

Find Star:  See Find Star above.

Set Exposure:  See Set Exposure above.

Start Scan:  Initiates the scan profiling

Stop:  Halts the scan profiling.

Save: Stores the data as in a text file named AGScanProfile.txt in a folder of the user’s choosing and the set of parameters that have produced the best result are uploaded to the Adaptive Profiler.  If the user cancels out of the folder selection, no file is saved.

Done:  Updates the initial settings in the Adaptive Test window and closes the window.

Requirements: 

 Auto Guide Profile 3 is a Windows Forms executable, written in Visual Basic.  The app requires TheSkyX Pro (Build 10229 or later) with the TSX Camera Add On option. The application may also require installation of Microsoft Powerpack 3.0 for charting (can be found at https://www.microsoft.com/en-us/download/details.aspx?id=25169).   The application runs as an uncertified, standalone application under Windows 7, 8 and 10.  

Installation:  

Open the AGProfiler3/publish folder. Download AGProfiler3_64Buildxx.zip. Extract all files and run setup.exe. This is an unsigned application so manually enable installation, if required. Upon completion, an application icon will have been added to the start menu under "TSXToolKit" with the name "AutoGuide Profiler 3". This application can be pinned to the Start if desired.

Support:  

This application was written for the public domain and as such is unsupported. The developer wishes you his best and hopes everything works out, but recommends learning Visual Basic (it's really not hard and the tools are free from Microsoft) if you find a problem or want to add features.  The source is supplied as a Visual Studio project.
