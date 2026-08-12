@echo off
setlocal EnableDelayedExpansion
title Time and Motion - setup
echo.
echo  =================================================
echo   Time and Motion - requirements check and run
echo  =================================================
echo.

set FAILED=0

rem ---------- .NET SDK 8 ----------
where dotnet >nul 2>&1
if errorlevel 1 (
    echo [MISSING] .NET SDK - install .NET SDK 8.0 from https://dotnet.microsoft.com/download/dotnet/8.0
    set FAILED=1
) else (
    set SDK8=
    for /f "tokens=1" %%v in ('dotnet --list-sdks 2^>nul') do (
        echo %%v | findstr /b /c:"8." >nul && set SDK8=%%v
    )
    if defined SDK8 (
        echo [OK]      .NET SDK !SDK8!
    ) else (
        echo [MISSING] .NET 8 SDK - install from https://dotnet.microsoft.com/download/dotnet/8.0
        set FAILED=1
    )
)

rem ---------- Node.js 20+ ----------
where node >nul 2>&1
if errorlevel 1 (
    echo [MISSING] Node.js - install the LTS from https://nodejs.org/ and reopen this window
    set FAILED=1
) else (
    for /f "tokens=1 delims=v." %%a in ('node --version') do set NODEMAJOR=%%a
    for /f %%v in ('node --version') do set NODEVER=%%v
    if !NODEMAJOR! GEQ 20 (
        echo [OK]      Node.js !NODEVER!
    ) else (
        echo [OLD]     Node.js !NODEVER! - 20+ required, install the LTS from https://nodejs.org/
        set FAILED=1
    )
)

rem ---------- npm ----------
where npm >nul 2>&1
if errorlevel 1 (
    echo [MISSING] npm ^(ships with Node.js^)
    set FAILED=1
) else (
    for /f %%v in ('npm --version') do echo [OK]      npm %%v
)

rem ---------- Visual Studio 2022 (optional, for F5) ----------
set "VSWHERE=%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe"
set VSVER=
if exist "%VSWHERE%" (
    for /f "usebackq tokens=*" %%v in (`"%VSWHERE%" -latest -products * -requires Microsoft.VisualStudio.Workload.NetWeb -property catalog_productDisplayVersion 2^>nul`) do set VSVER=%%v
)
if defined VSVER (
    echo [OK]      Visual Studio !VSVER! with the ASP.NET workload
) else (
    echo [INFO]    Visual Studio 2022 + ASP.NET workload not detected - optional, this script runs everything from the CLI
)

echo.
if not "%FAILED%"=="0" (
    echo One or more requirements are missing. Install them and run setup.bat again.
    pause
    exit /b 1
)

rem ---------- client packages ----------
if not exist "%~dp0gmvTM.Client\node_modules" (
    echo Installing client packages, this can take a minute...
    pushd "%~dp0gmvTM.Client"
    call npm install
    if errorlevel 1 (
        popd
        echo npm install failed.
        pause
        exit /b 1
    )
    popd
)

rem ---------- build ----------
echo Building the solution...
dotnet build "%~dp0gmvTM.sln" --nologo
if errorlevel 1 (
    echo Build failed.
    pause
    exit /b 1
)

rem ---------- run ----------
echo.
echo Starting the server. The Vite dev server is launched automatically by the SPA proxy.
echo   App:     http://localhost:5173/route/f
echo   Swagger: https://localhost:7080/swagger
echo Press Ctrl+C in this window to stop.
echo.
start "" http://localhost:5173/route/f
dotnet run --project "%~dp0gmvTM.Server" --launch-profile https

endlocal
