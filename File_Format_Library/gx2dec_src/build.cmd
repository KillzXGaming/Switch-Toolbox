@echo off
rem Build gx2dec.exe: the CLI driver (src/) against Cemu v2.6's legacy Latte shader
rem decompiler (MPL-2.0, https://github.com/cemu-project/Cemu tag v2.6), MSVC C++20.
rem
rem Fetch the dependencies next to this script first (header-only / source-only):
rem   Cemu-2.6\             Cemu v2.6 source tree (only src\Cafe\HW\Latte\** is compiled)
rem   fmt-10.2.1\           https://github.com/fmtlib/fmt (header-only via FMT_HEADER_ONLY)
rem   boost_1_85_0\         boost headers (BOOST_ALL_NO_LIB; nothing is linked)
rem   glm-1.0.1\            https://github.com/g-truc/glm (header-only)
rem   Vulkan-Headers-1.3.290\  https://github.com/KhronosGroup/Vulkan-Headers (VK_NO_PROTOTYPES)
rem shim\ provides stub openssl/wx headers and a no-op Renderer so the decompiler
rem translation units build without Cemu's full dependency set; the GLSL generation
rem code itself is unmodified Cemu source.
setlocal enabledelayedexpansion
rem Set VSDEVCMD to a VsDevCmd.bat to pick a specific Visual Studio; otherwise vswhere finds one.
set "VSWHERE=%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe"
if not defined VSDEVCMD if exist "%VSWHERE%" (
  for /f "usebackq delims=" %%i in (`"%VSWHERE%" -latest -products * -requires Microsoft.VisualStudio.Component.VC.Tools.x86.x64 -property installationPath`) do set "VSDEVCMD=%%i\Common7\Tools\VsDevCmd.bat"
)
if not exist "%VSDEVCMD%" (
  echo No Visual Studio with the C++ toolchain was found. Install the "Desktop development with C++" workload, or set VSDEVCMD to the full path of VsDevCmd.bat.
  exit /b 1
)
call "%VSDEVCMD%" -arch=amd64 >nul || exit /b 1
set ROOT=%~dp0
set CEMU=%ROOT%Cemu-2.6\src
set DRIVER=%ROOT%src
set INC=/I "%CEMU%" /I "%ROOT%fmt-10.2.1\include" /I "%ROOT%boost_1_85_0" /I "%ROOT%glm-1.0.1" /I "%ROOT%shim" /I "%ROOT%Vulkan-Headers-1.3.290\include"
set FLAGS=/nologo /std:c++20 /O2 /EHsc /MP /DENABLE_OPENGL /DVK_NO_PROTOTYPES /DBOOST_ALL_NO_LIB /DNOMINMAX /DWIN32_LEAN_AND_MEAN /FI"Common/precompiled.h" /c
mkdir "%ROOT%build" 2>nul
cd /d "%ROOT%build"

cl %FLAGS% %INC% ^
  "%CEMU%\Cafe\HW\Latte\LegacyShaderDecompiler\LatteDecompiler.cpp" ^
  "%CEMU%\Cafe\HW\Latte\LegacyShaderDecompiler\LatteDecompilerAnalyzer.cpp" ^
  "%CEMU%\Cafe\HW\Latte\LegacyShaderDecompiler\LatteDecompilerEmitGLSL.cpp" ^
  "%CEMU%\Cafe\HW\Latte\LegacyShaderDecompiler\LatteDecompilerEmitGLSLAttrDecoder.cpp" ^
  "%CEMU%\Cafe\HW\Latte\LegacyShaderDecompiler\LatteDecompilerRegisterDataTypeTracker.cpp" ^
  "%CEMU%\Cafe\HW\Latte\Core\FetchShader.cpp" ^
  "%CEMU%\Cafe\HW\Latte\Core\LatteGSCopyShaderParser.cpp" ^
  "%DRIVER%\globals.cpp" ^
  "%DRIVER%\latteshader_impl.cpp" ^
  "%DRIVER%\main.cpp"
if errorlevel 1 exit /b 1
cl /nologo /Fe:"%ROOT%..\gx2dec.exe" *.obj
