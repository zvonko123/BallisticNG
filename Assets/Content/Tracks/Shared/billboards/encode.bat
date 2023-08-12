set /p infile=Enter video file name with extension:%=%
ffmpeg2theora-0.29.exe %infile% -v 10
pause