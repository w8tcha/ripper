"%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe" -nologo -latest -property installationPath > temp.txt
set /p $MSBUILDROOT=<temp.txt
del temp.txt

"%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe" -property installationVersion > temp.txt
set /p $MSBUILDVER=<temp.txt
del temp.txt

for /f "tokens=1 delims=." %%G in ("%$MSBUILDVER%") do set Current=%%G.0
Rem VS2017 => ~\MSBuild\15.0\Bin\MSBuild.exe
Rem VS2019 => ~\MsBuild\Current\Bin\MSBuild.exe
If "%Current%" NEQ "15.0" set Current=Current

@set $MSBUILDPATH="%$MSBUILDROOT%\MsBuild\%Current%\Bin\MSBuild.exe"

-- Build X Build
%$MSBUILDPATH% PG-Ripper.sln /p:Configuration=PG-RipperX /t:Clean;Build /p:WarningLevel=0

-- Build Regular Build + Package 
%$MSBUILDPATH% PG-Ripper.sln /p:Configuration=PG-Ripper /t:Clean;Build /p:WarningLevel=0