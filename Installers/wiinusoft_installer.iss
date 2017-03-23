#define MyAppName "WiinUSoft"
#define MyAppVersion "3.0"
#define MyAppPublisher "Justin Keys"
#define MyAppURL "http://www.wiinupro.com/"
#define MyAppExeName "WiinUSoft.exe"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{1BFC4F9F-BB85-4CE3-AC22-0CBFF78D5EE4}
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
OutputDir=.\
OutputBaseFilename=wiinusoft_setup
Compression=lzma
SolidCompression=yes
ArchitecturesInstallIn64BitMode=x64

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
;Name: "brazilianportuguese"; MessagesFile: "compiler:Languages\BrazilianPortuguese.isl"
;Name: "catalan"; MessagesFile: "compiler:Languages\Catalan.isl"
;Name: "corsican"; MessagesFile: "compiler:Languages\Corsican.isl"
;Name: "czech"; MessagesFile: "compiler:Languages\Czech.isl"
;Name: "danish"; MessagesFile: "compiler:Languages\Danish.isl"
;Name: "dutch"; MessagesFile: "compiler:Languages\Dutch.isl"
;Name: "finnish"; MessagesFile: "compiler:Languages\Finnish.isl"
;Name: "french"; MessagesFile: "compiler:Languages\French.isl"
;Name: "german"; MessagesFile: "compiler:Languages\German.isl"
;Name: "greek"; MessagesFile: "compiler:Languages\Greek.isl"
;Name: "hebrew"; MessagesFile: "compiler:Languages\Hebrew.isl"
;Name: "hungarian"; MessagesFile: "compiler:Languages\Hungarian.isl"
;Name: "italian"; MessagesFile: "compiler:Languages\Italian.isl"
;Name: "japanese"; MessagesFile: "compiler:Languages\Japanese.isl"
;Name: "norwegian"; MessagesFile: "compiler:Languages\Norwegian.isl"
;Name: "polish"; MessagesFile: "compiler:Languages\Polish.isl"
;Name: "portuguese"; MessagesFile: "compiler:Languages\Portuguese.isl"
;Name: "russian"; MessagesFile: "compiler:Languages\Russian.isl"
;Name: "scottishgaelic"; MessagesFile: "compiler:Languages\ScottishGaelic.isl"
;Name: "serbiancyrillic"; MessagesFile: "compiler:Languages\SerbianCyrillic.isl"
;Name: "serbianlatin"; MessagesFile: "compiler:Languages\SerbianLatin.isl"
;Name: "slovenian"; MessagesFile: "compiler:Languages\Slovenian.isl"
;Name: "spanish"; MessagesFile: "compiler:Languages\Spanish.isl"
;Name: "turkish"; MessagesFile: "compiler:Languages\Turkish.isl"
;Name: "ukrainian"; MessagesFile: "compiler:Languages\Ukrainian.isl"

[Types]
Name: "full"; Description: "Install Everything";
Name: "custom"; Description: "Pick and Choose"; Flags: iscustom

[Components]
Name: "main"; Description: "WiinUSoft"; Types: full custom; Flags: fixed;
Name: "scp"; Description: "Scarlet Crush Production Driver"; Types: full
Name: "xbox"; Description: "Xbox 360 Controller Driver (Required on Windows XP, Vista, and 7)"; Types: full

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 0,6.1

[Files]
Source: "..\WiinUSoft\bin\Release\WiinUSoft.exe"; DestDir: "{app}"; Components: main; Flags: ignoreversion
Source: "..\WiinUSoft\bin\Release\Nintroller.dll"; DestDir: "{app}"; Components: main; Flags: ignoreversion
Source: "..\WiinUSoft\bin\Release\RestSharp.dll"; DestDir: "{app}"; Components: main; Flags: ignoreversion
Source: "..\WiinUSoft\bin\Release\ScpControl.dll"; DestDir: "{app}"; Components: main; Flags: ignoreversion
Source: "..\WiinUSoft\bin\Release\Hardcodet.Wpf.TaskbarNotification.dll"; Components: main; DestDir: "{app}"; Flags: ignoreversion
Source: "..\WiinUSoft\ReadMe.txt"; DestDir: "{app}"; Components: main; Flags: ignoreversion isreadme
Source: "Drivers\SCP_Driver\*"; DestDir: "{app}\SCP_Driver"; Components: scp; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "Drivers\Xbox360_32Eng_7.exe"; DestDir: "{app}"; DestName: "Xbox360Driver.exe"; Components: xbox; Check: IsWindows7OrAbove and not Is64BitInstallMode; Flags: ignoreversion
Source: "Drivers\Xbox360_64Eng_7.exe"; DestDir: "{app}"; DestName: "Xbox360Driver.exe"; Components: xbox; Check: IsWindows7OrAbove and Is64BitInstallMode; Flags: ignoreversion
Source: "Drivers\Xbox360_32Eng_Vista.exe"; DestDir: "{app}"; DestName: "Xbox360Driver.exe"; Components: xbox; Check: IsWindowsVista and not Is64BitInstallMode; Flags: ignoreversion
Source: "Drivers\Xbox360_64Eng_Vista.exe"; DestDir: "{app}"; DestName: "Xbox360Driver.exe"; Components: xbox; Check: IsWindowsVista and Is64BitInstallMode; Flags: ignoreversion
Source: "Drivers\Xbox360_32Eng_XPSP2.exe"; DestDir: "{app}"; DestName: "Xbox360Driver.exe"; Components: xbox; Check: IsWindowsXpSp2 and not Is64BitInstallMode; Flags: ignoreversion
Source: "Drivers\Xbox360_64Eng_XP.exe"; DestDir: "{app}"; DestName: "Xbox360Driver.exe"; Components: xbox; Check: IsWindowsXp and Is64BitInstallMode; Flags: ignoreversion

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\ReadMe"; Filename: "{app}\ReadMe.txt"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: quicklaunchicon

[Run]
Filename: "{app}\SCP_Driver\ScpDriver.exe"; Components: scp
Filename: "{app}\Xbox360Driver.exe"; Components: xbox;
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[UninstallRun]
Filename: "{app}\SCP_Driver\ScpDriver.exe"; Components: scp

[Code]
procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
var
  FileName: String;
begin
  if CurUninstallStep = usPostUninstall then
  begin
    FileName := ExpandConstant('{userappdata}') + '\WiinUSoft_prefs.config';
    if FileExists(FileName) then
      if MsgBox('Do you want to delete your saved WiinUSoft preferences ?',
        mbConfirmation, MB_YESNO) = IDYES
      then
        DeleteFile(FileName);
  end;
end;

function IsWindowsXp: Boolean;
var
  Version: TWindowsVersion;
begin
  GetWindowsVersionEx(Version);

  if Version.NTPlatform and
    (Version.Major = 5) and
    (Version.Minor > 0) then
  begin
    Result := True;
    Exit;
  end

  Result := False;
end;

function IsWindowsXpSp2: Boolean;
var
  Version: TWindowsVersion;
begin
  GetWindowsVersionEx(Version);

  if Version.NTPlatform and
    (Version.Major = 5) and
    (Version.Minor > 0) and
    (Version.ServicePackMajor > 1) then
  begin
    Result := True;
    Exit;
  end

  Result := False;
end;

function IsWindowsVista: Boolean;
var
  Version: TWindowsVersion;
begin
  GetWindowsVersionEx(Version);

  if Version.NTPlatform and
    (Version.Major = 6) and
    (Version.Minor = 0) then
  begin
    Result := True;
    Exit;
  end

  Result := False;
end;

function IsWindows7OrAbove: Boolean;
var
  Version: TWindowsVersion;
begin
  GetWindowsVersionEx(Version);

  // Windows 7
  if Version.NTPlatform and
    (Version.Major = 6) and
    (Version.Minor = 1) then
  begin
    Result := True;
    Exit;
  end

  // Windows 8
  if Version.NTPlatform and
    (Version.Major = 6) and
    (Version.Minor = 2) then
  begin
    Result := True;
    Exit;
  end

  // Windows 8.1
  if Version.NTPlatform and
    (Version.Major = 6) and
    (Version.Minor = 3) then
  begin
    Result := True;
    Exit;
  end

  // Windows 10
  if Version.NTPlatform and
    (Version.Major = 10) then
  begin
    Result := True;
    Exit;
  end

  Result := False;
end;