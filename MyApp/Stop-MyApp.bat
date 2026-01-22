@echo off
REM Quick batch script to stop MyApp.exe

echo Stopping MyApp.exe processes...
taskkill /F /IM MyApp.exe 2>nul

if %ERRORLEVEL% == 0 (
    echo ? MyApp.exe stopped successfully!
) else (
    echo ? No MyApp.exe process running.
)

timeout /t 1 /nobreak >nul
echo Ready to build!
