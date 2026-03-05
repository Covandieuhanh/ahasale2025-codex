@echo off
setlocal

set SITE_DIR=%~dp0..
for %%I in ("%SITE_DIR%") do set SITE_DIR=%%~fI

set IIS_EXPRESS=%ProgramFiles%\IIS Express\iisexpress.exe
if not exist "%IIS_EXPRESS%" set IIS_EXPRESS=%ProgramFiles(x86)%\IIS Express\iisexpress.exe

if not exist "%IIS_EXPRESS%" (
  echo [ERROR] Khong tim thay iisexpress.exe.
  echo Cai IIS Express hoac Visual Studio 2022 truoc khi chay.
  exit /b 1
)

echo Starting AhaSale local site at http://localhost:56445
"%IIS_EXPRESS%" /path:"%SITE_DIR%" /port:56445 /clr:v4.0
