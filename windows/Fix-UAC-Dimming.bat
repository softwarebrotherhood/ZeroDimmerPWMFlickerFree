@echo off
title Dim Run-As-Admin Popups
net session >nul 2>&1
if %errorLevel% neq 0 (
    echo Requesting Administrator privileges to adjust UAC desktop policy...
    powershell -Command "Start-Process cmd -ArgumentList '/c \"\"%~f0\"\"' -Verb RunAs"
    exit /b
)
reg add "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System" /v PromptOnSecureDesktop /t REG_DWORD /d 0 /f
echo.
echo =========================================================================
echo  [SUCCESS] PromptOnSecureDesktop is now set to 0.
echo  "Run as administrator" (UAC) prompts will now stay on your desktop
echo  and will be 100%% DIMMED by FlickerFree Dimmer!
echo =========================================================================
echo.
pause
