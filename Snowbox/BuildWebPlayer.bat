@echo off

tasklist /fi "imagename eq Unity.exe" | find "Unity" >nul
if errorlevel 1 goto locate_unity

echo ABORTING: The Unity Editor is already running, please close it and restart this build script.
if not "%1"=="nopause" pause
exit

:locate_unity

reg query "HKCU\Software\Unity Technologies\Unity Editor" /v "Location" 1>nul 2>&1 || (goto guess_unity)

for /f "tokens=2,*" %%a in ('reg query "HKCU\Software\Unity Technologies\Unity Editor" /v "Location" ^| findstr /c:"Location"') do (
	call :found_unity %%b %1
)

exit

:guess_unity

if exist "%ProgramFiles(x86)%\Unity\Editor\Unity.exe" (
	call :found_unity "%ProgramFiles(x86)%\Unity\Editor\Unity.exe" %1
	exit
)

if exist "%ProgramFiles%\Unity\Editor\Unity.exe" (
	call :found_unity "%ProgramFiles%\Unity\Editor\Unity.exe" %1
	exit
)

echo ABORTING: This build script needs the Unity Editor to run, but it does not seem to be installed on your system.
if not "%1"=="nopause" pause
exit

:found_unity

echo Automatically building WebPlayer... (please wait)

%1 -batchmode -projectPath "%CD%" -executeMethod AutoBuild.WebPlayer -quit

if not errorlevel 1 (
	echo You can now open the WebPlayer folder
) else (
	echo ERROR: Something went wrong while building
)

if not "%2"=="nopause" pause
exit
