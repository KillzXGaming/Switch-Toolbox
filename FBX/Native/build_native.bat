@echo off
SETLOCAL EnableDelayedExpansion

REM Find VS install path (Reused from main build.bat)
for /f "tokens=*" %%i in ('"%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe" -latest -products * -requires Microsoft.Component.MSBuild -property installationPath') do set VSINSTALL=%%i

if "%VSINSTALL%"=="" (
    echo ERROR: Visual Studio not found.
    exit /b 1
)

set MSBUILD="%VSINSTALL%\MSBuild\Current\Bin\MSBuild.exe"

if not exist %MSBUILD% (
    set MSBUILD="%VSINSTALL%\MSBuild\15.0\Bin\MSBuild.exe"
)

if not exist %MSBUILD% (
    echo ERROR: MSBuild not found.
    exit /b 1
)

echo Using MSBuild: %MSBUILD%
echo Building FBX Native Wrapper...

%MSBUILD% SwitchToolbox.FbxNative.vcxproj /t:Rebuild /p:Configuration=Release /p:Platform=x64

if errorlevel 1 (
    echo NATIVE BUILD FAILED!
    exit /b 1
)

echo Native Build Succeeded.

REM Copy to Toolbox bin/Release so it's picked up by the app and dist
xcopy /y "dist\SwitchToolbox.FbxNative.dll" "..\..\Toolbox\bin\Release\"

echo DLL Deployed to Toolbox/bin/Release.
