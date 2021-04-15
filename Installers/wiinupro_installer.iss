;Inno Setup 6
#define MyAppName "WiinUPro"
#define MyAppVersion "0.9.7"
#define MyAppPublisher "Justin Keys"
#define MyAppURL "https://github.com/KeyPuncher/WiinUPro/releases"
#define MyAppExeName "WiinUPro.exe"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{40F0DCB4-E81A-45CE-A596-F2D083E1D535}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
DisableDirPage=auto
AllowNoIcons=yes
OutputDir=.\
OutputBaseFilename=WiinUPro_{#MyAppVersion}_setup
Compression=lzma
SolidCompression=yes
ArchitecturesInstallIn64BitMode=x64
WizardStyle=modern
WizardSmallImageFile=WiinUPro.bmp
SetupIconFile=wup_install.ico

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
;Name: "brazilianportuguese"; MessagesFile: "compiler:Languages\BrazilianPortuguese.isl"
;Name: "catalan"; MessagesFile: "compiler:Languages\Catalan.isl"
;Name: "corsican"; MessagesFile: "compiler:Languages\Corsican.isl"
;Name: "czech"; MessagesFile: "compiler:Languages\Czech.isl"
;Name: "danish"; MessagesFile: "compiler:Languages\Danish.isl"
;Name: "dutch"; MessagesFile: "compiler:Languages\Dutch.isl"
;Name: "finnish"; MessagesFile: "compiler:Languages\Finnish.isl"
Name: "french"; MessagesFile: "compiler:Languages\French.isl"
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
Name: "spanish"; MessagesFile: "compiler:Languages\Spanish.isl"
;Name: "turkish"; MessagesFile: "compiler:Languages\Turkish.isl"
;Name: "ukrainian"; MessagesFile: "compiler:Languages\Ukrainian.isl"

[CustomMessages]
english.InstallAll=Install Everything
english.InstallSome=Pick and Choose
english.ComponentXbox=Xbox 360 Controller Driver (Required on Windows XP/Vista/7)
english.ComponentGCN=Game Cube Adapter Driver (Must be Plugged in)
french.InstallAll=Tout installer
french.InstallSome=Choisir
french.ComponentXbox=Xbox 360 Controller Driver (Requis sous Windows XP/Vista/and 7)
french.ComponentGCN=Game Cube Adapter Driver (Doit être branché)
spanish.InstallAll=Instalar todo
spanish.InstallSome=Escoger
spanish.ComponentXbox=Xbox 360 Controller Driver (Requerido para Windows XP/Vista/7)
spanish.ComponentGCN=Game Cube Adapter Driver (Debe estar enchufado)

[Types]
Name: "full"; Description: "{cm:InstallAll}";
Name: "custom"; Description: "{cm:InstallSome}"; Flags: iscustom

[Components]
Name: "main"; Description: "WiinUPro"; Types: full custom; Flags: fixed;
Name: "scp"; Description: "Scarlet Crush Production Driver"; Types: full
Name: "xbox"; Description: "{cm:ComponentXbox}"; Types: full
Name: "gcn"; Description: "{cm:ComponentGCN}"; Types: full

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 0,6.1

[Files]
Source: "..\WiinUPro\bin\Release\WiinUPro.exe"; DestDir: "{app}"; Components: main; Flags: ignoreversion
Source: "..\WiinUPro\bin\Release\lang.json"; DestDir: "{app}"; Components: main; Flags: ignoreversion
Source: "..\WiinUPro\bin\Release\Nintroller.dll"; DestDir: "{app}"; Components: main; Flags: ignoreversion
Source: "..\WiinUPro\bin\Release\Newtonsoft.Json.dll"; DestDir: "{app}"; Components: main; Flags: ignoreversion
Source: "..\WiinUPro\bin\Release\InputManager.dll"; DestDir: "{app}"; Components: main; Flags: ignoreversion
Source: "..\WiinUPro\bin\Release\ScpControl.dll"; DestDir: "{app}"; Components: main; Flags: ignoreversion
Source: "..\WiinUPro\bin\Release\LibUsbDotNet.dll"; DestDir: "{app}"; Components: main; Flags: ignoreversion
Source: "..\WiinUPro\bin\Release\SharpDX.dll"; DestDir: "{app}"; Components: main; Flags: ignoreversion
Source: "..\WiinUPro\bin\Release\SharpDX.DirectInput.dll"; DestDir: "{app}"; Components: main; Flags: ignoreversion
Source: "..\WiinUPro\bin\Release\SharpDX.RawInput.dll"; DestDir: "{app}"; Components: main; Flags: ignoreversion
Source: "..\WiinUPro\bin\Release\Hardcodet.Wpf.TaskbarNotification.dll"; DestDir: "{app}"; Components: main; Flags: ignoreversion
Source: "..\vJoy\x64\vJoyInterface.dll"; DestDir: "{app}"; Components: main; Check: Is64BitInstallMode; Flags: ignoreversion
Source: "..\vJoy\x64\vJoyInterfaceWrap.dll"; DestDir: "{app}"; Components: main; Check: Is64BitInstallMode; Flags: ignoreversion
Source: "..\vJoy\x86\vJoyInterface.dll"; DestDir: "{app}"; Components: main; Check: not Is64BitInstallMode; Flags: ignoreversion
Source: "..\vJoy\x86\vJoyInterfaceWrap.dll"; DestDir: "{app}"; Components: main; Check: not Is64BitInstallMode; Flags: ignoreversion
;Source: "..\WiinUPro\ReadMe.txt"; DestDir: "{app}"; Components: main; Flags: ignoreversion isreadme
Source: "Drivers\SCP_Driver\*"; DestDir: "{app}\SCP_Driver"; Components: scp; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "Drivers\Xbox360_32Eng_7.exe"; DestDir: "{app}"; DestName: "Xbox360Driver.exe"; Components: xbox; Check: IsWindows7OrAbove and not Is64BitInstallMode; Flags: ignoreversion
Source: "Drivers\Xbox360_64Eng_7.exe"; DestDir: "{app}"; DestName: "Xbox360Driver.exe"; Components: xbox; Check: IsWindows7OrAbove and Is64BitInstallMode; Flags: ignoreversion
Source: "Drivers\Xbox360_32Eng_Vista.exe"; DestDir: "{app}"; DestName: "Xbox360Driver.exe"; Components: xbox; Check: IsWindowsVista and not Is64BitInstallMode; Flags: ignoreversion
Source: "Drivers\Xbox360_64Eng_Vista.exe"; DestDir: "{app}"; DestName: "Xbox360Driver.exe"; Components: xbox; Check: IsWindowsVista and Is64BitInstallMode; Flags: ignoreversion
Source: "Drivers\Xbox360_32Eng_XPSP2.exe"; DestDir: "{app}"; DestName: "Xbox360Driver.exe"; Components: xbox; Check: IsWindowsXpSp2 and not Is64BitInstallMode; Flags: ignoreversion
Source: "Drivers\Xbox360_64Eng_XP.exe"; DestDir: "{app}"; DestName: "Xbox360Driver.exe"; Components: xbox; Check: IsWindowsXp and Is64BitInstallMode; Flags: ignoreversion
Source: "Drivers\zadig_2.1.2.gcn.exe"; DestDir: "{app}"; Components: gcn; Flags: ignoreversion
;TODO XInput Default Profiles

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
;Name: "{group}\ReadMe"; Filename: "{app}\ReadMe.txt"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: quicklaunchicon

[Run]
Filename: "{app}\SCP_Driver\ScpDriver.exe"; Components: scp;
Filename: "{app}\Xbox360Driver.exe"; Components: xbox;
Filename: "{app}\zadig_2.1.2.gcn.exe"; Components: gcn;
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[UninstallRun]
Filename: "{app}\SCP_Driver\ScpDriver.exe"; Components: scp

[Code]
/////////////////////////////////////////////////////////////////////
function GetUninstallString(): String;
var
  sUnInstPath: String;
  sUnInstallString: String;
begin
  sUnInstPath := ExpandConstant('Software\Microsoft\Windows\CurrentVersion\Uninstall\{#emit SetupSetting("AppId")}_is1');
  sUnInstallString := '';
  if not RegQueryStringValue(HKLM, sUnInstPath, 'UninstallString', sUnInstallString) then
    RegQueryStringValue(HKCU, sUnInstPath, 'UninstallString', sUnInstallString);
  Result := sUnInstallString;
end;

procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
var
  DirName: String;
begin
  if CurUninstallStep = usPostUninstall then
  begin
    DirName := ExpandConstant('{userappdata}') + '\WiinUPro';
    if DirExists(DirName) then
      begin
        if (GetUninstallString() = '') then
          begin
            if MsgBox('Do you want to delete your saved WiinUPro preferences ?',
              mbConfirmation, MB_YESNO) = IDYES
            then
              DelTree(DirName, True, True, True);
          end
        else
          DelTree(DirName, True, True, True);
      end;
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
  end;

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
  end;

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
  end;

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
  end;

  // Windows 8
  if Version.NTPlatform and
    (Version.Major = 6) and
    (Version.Minor = 2) then
  begin
    Result := True;
    Exit;
  end;

  // Windows 8.1
  if Version.NTPlatform and
    (Version.Major = 6) and
    (Version.Minor = 3) then
  begin
    Result := True;
    Exit;
  end;

  // Windows 10
  if Version.NTPlatform and
    (Version.Major = 10) then
  begin
    Result := True;
    Exit;
  end;

  Result := False;
end;

// code for uninstalling the previous version



/////////////////////////////////////////////////////////////////////
function IsUpgrade(): Boolean;
begin
  Result := False;
  if (GetUninstallString() <> '') then
  begin
    if MsgBox('There is another version of WiinUPro installed. Uninstall it?',
      mbConfirmation, MB_YESNO) = IDYES
    then
      Result := True;
  end;
end;


/////////////////////////////////////////////////////////////////////
function UnInstallOldVersion(): Integer;
var
  sUnInstallString: String;
  iResultCode: Integer;
begin
// Return Values:
// 1 - uninstall string is empty
// 2 - error executing the UnInstallString
// 3 - successfully executed the UnInstallString

  // default return value
  Result := 0;

  // get the uninstall string of the old app
  sUnInstallString := GetUninstallString();
  if sUnInstallString <> '' then begin
    sUnInstallString := RemoveQuotes(sUnInstallString);
    if Exec(sUnInstallString, '/SILENT /NORESTART /SUPPRESSMSGBOXES','', SW_HIDE, ewWaitUntilTerminated, iResultCode) then
      Result := 3
    else
      Result := 2;
  end else
    Result := 1;
end;

/////////////////////////////////////////////////////////////////////
procedure CurStepChanged(CurStep: TSetupStep);
begin
  if (CurStep=ssInstall) then
  begin
    if (IsUpgrade()) then
    begin
      UnInstallOldVersion();
    end;
  end;
end;