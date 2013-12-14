@SET FrameworkDir=C:\Windows\Microsoft.NET\Framework\v4.0.30319
@SET FrameworkVersion=v4.0.30319
@SET FrameworkSDKDir=
@SET PATH=%FrameworkDir%;%FrameworkSDKDir%;%PATH%
@SET LANGDIR=EN

-- Build X64 Build
msbuild.exe PG-Ripper.sln /p:Configuration=PG-Ripper64 /t:Clean;Build /p:WarningLevel=0 /p:Platform="x64"

-- Build X Build
msbuild.exe PG-Ripper.sln /p:Configuration=PG-RipperX /t:Clean;Build /p:WarningLevel=0

-- Build Regular Build + Package 
msbuild.exe PG-Ripper.sln /p:Configuration=PG-Ripper /t:Clean;Build /p:WarningLevel=0