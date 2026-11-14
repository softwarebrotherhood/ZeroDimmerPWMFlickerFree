@echo off
if exist "%~dp0bin\Release\net9.0-windows\publish\AntiFlickerDimmer.exe" (
    start "" "%~dp0bin\Release\net9.0-windows\publish\AntiFlickerDimmer.exe"
    exit
)
if exist "%~dp0bin\Release\net9.0-windows\AntiFlickerDimmer.exe" (
    start "" "%~dp0bin\Release\net9.0-windows\AntiFlickerDimmer.exe"
    exit
)
dotnet run -c Release --project "%~dp0AntiFlickerDimmer.csproj"
