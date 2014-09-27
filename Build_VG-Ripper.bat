@SET FrameworkDir=C:\Windows\Microsoft.NET\Framework\v4.0.30319
@SET FrameworkVersion=v4.0.30319
@SET FrameworkSDKDir=
@SET PATH=%FrameworkDir%;%FrameworkSDKDir%;%PATH%
@SET LANGDIR=EN

--- Build X Build
msbuild.exe VG-Ripper.sln /p:Configuration=RiPRipperX /t:Clean;Build /p:WarningLevel=0

--- Build Regular Build + Package 
msbuild.exe VG-Ripper.sln /p:Configuration=RiPRipper /t:Clean;Build /p:WarningLevel=0

