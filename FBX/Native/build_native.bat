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

REM Copy native dependencies.
set FBXSDK_DLL=..\FBX SDK\2017.0.1\lib\vs2015\x64\release\libfbxsdk.dll
if exist "%FBXSDK_DLL%" (
    xcopy /y "%FBXSDK_DLL%" "dist\"
) else (
    echo WARNING: libfbxsdk.dll not found at "%FBXSDK_DLL%"
)

REM Copy to Toolbox bin/Release so it's picked up by the app and dist
xcopy /y "dist\SwitchToolbox.FbxNative.dll" "..\..\Toolbox\bin\Release\"
if exist "dist\libfbxsdk.dll" (
    xcopy /y "dist\libfbxsdk.dll" "..\..\Toolbox\bin\Release\"
)

echo DLL Deployed to Toolbox/bin/Release.
