; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "PG-Ripper"
#define MyAppVersion "1.4.0.9"
#define MyAppPublisher "The Watcher"
#define MyAppURL "http://ripper.codeplex.com"
#define MyAppExeName "PGRipper.exe"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{494069BB-BB57-4B73-8A96-C46B040208E2}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={pf}\{#MyAppName}
DefaultGroupName={#MyAppName}
AllowNoIcons=yes
LicenseFile=C:\Users\Watcher\Documents\Projects\Visual Studio 2012\Projects\rip-ripper\PG-Ripper\bin\license.txt
InfoBeforeFile=C:\Users\Watcher\Documents\Projects\Visual Studio 2012\Projects\rip-ripper\PG-Ripper\bin\readme.txt
OutputBaseFilename=RiP-Ripper-Setup
SetupIconFile=C:\Users\Watcher\Documents\Projects\Visual Studio 2012\Projects\rip-ripper\PG-Ripper\App.ico
Compression=lzma/ultra64
SolidCompression=yes

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 0,6.1

[Files]
Source: "C:\Users\Watcher\Documents\Projects\Visual Studio 2012\Projects\rip-ripper\PG-Ripper\bin\PG-Ripper\PGRipper.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Watcher\Documents\Projects\Visual Studio 2012\Projects\rip-ripper\PG-Ripper\bin\PG-Ripper\ICSharpCode.SharpZipLib.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Watcher\Documents\Projects\Visual Studio 2012\Projects\rip-ripper\PG-Ripper\bin\PG-Ripper\Microsoft.WindowsAPICodePack.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Watcher\Documents\Projects\Visual Studio 2012\Projects\rip-ripper\PG-Ripper\bin\PG-Ripper\Microsoft.WindowsAPICodePack.Shell.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Watcher\Documents\Projects\Visual Studio 2012\Projects\rip-ripper\PG-Ripper\bin\license.txt"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Watcher\Documents\Projects\Visual Studio 2012\Projects\rip-ripper\PG-Ripper\bin\readme.txt"; DestDir: "{app}"; Flags: ignoreversion
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: quicklaunchicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

