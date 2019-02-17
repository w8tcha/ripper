@SET FrameworkSDKDir=
@SET PATH=%FrameworkDir%;%FrameworkSDKDir%;%PATH%
@SET LANGDIR=EN
@SET MSBUILDPATH="C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\MsBuild.exe"

--- Build X Build
%MSBUILDPATH% VG-Ripper.sln /p:Configuration=RiPRipperX /t:Clean;Build /p:WarningLevel=0
--- Build Regular Build + Package 
%MSBUILDPATH% VG-Ripper.sln /p:Configuration=RiPRipper /t:Clean;Build /p:WarningLevel=0

