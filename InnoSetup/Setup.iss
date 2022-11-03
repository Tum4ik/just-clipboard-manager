﻿; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "Just Clipboard Manager"
#define MyAppPublisher "Yevheniy Tymchishin"
#define MyAppURL "https://github.com/Tum4ik/just-clipboard-manager"
#define MyAppExeName "JustClipboardManager.exe"
#define MyAppCopyright "© 2022-2023 Yevheniy Tymchishin. All rights reserved."
#include "version.iss"
#include "architecture.iss"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{CA7FB06C-6E28-4BC0-AF70-2C365C3C93A0}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={autopf}\{#MyAppName}
DisableProgramGroupPage=yes
LicenseFile=..\LICENSE
; Remove the following line to run in administrative install mode (install for all users.)
PrivilegesRequired=lowest
;PrivilegesRequiredOverridesAllowed=dialog
OutputDir=..\Tum4ik.JustClipboardManager\bin\setup
OutputBaseFilename=JustClipboardManager-{#MyAppVersion}-{#Architecture}
SetupIconFile=install.ico
UninstallDisplayIcon={app}\uninstall.ico
Compression=lzma
SolidCompression=yes
WizardStyle=modern
AppCopyright={#MyAppCopyright}
AppContact=timchishinevgeniy@gmail.com
VersionInfoVersion={#MyAppVersion}
VersionInfoTextVersion={#MyAppVersion}
VersionInfoProductVersion={#MyAppVersion}
VersionInfoCopyright={#MyAppCopyright}
VersionInfoCompany={#MyAppPublisher}
VersionInfoProductName={#MyAppName}
VersionInfoProductTextVersion={#MyAppVersion}
ArchitecturesInstallIn64BitMode=x64

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "ukrainian"; MessagesFile: "compiler:Languages\Ukrainian.isl"

[CustomMessages]
RunMyAppOnSystemStartup=Run %1 on system startup
DoYouWantRemoveAppSettingsAndClips=Do you also want to remove the application settings and "clips"?
YouHaveNewerVersion=You have newer version (%1) installed already
YouHaveThisVersion=You have this version (%1) installed already
ukrainian.RunMyAppOnSystemStartup=Запускати %1 під час запуску системи
ukrainian.DoYouWantRemoveAppSettingsAndClips=Чи бажаєте також видалити налаштування застосунку та "кліпи"?
ukrainian.YouHaveNewerVersion=У Вас вже встановлена новіша версія (%1)
ukrainian.YouHaveThisVersion=У Вас вже втановлена ця версія (%1)

[Tasks]
Name: "desktopicon"; \
  Description: "{cm:CreateDesktopIcon}"; \
  GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "autostart"; \
  Description: "{cm:RunMyAppOnSystemStartup,{#MyAppName}}"; \
  GroupDescription: "{cm:AutoStartProgramGroupDescription}"

[Files]
Source: "..\Tum4ik.JustClipboardManager\bin\publish\{#Architecture}\*"; \
  DestDir: "{app}"; \
  Flags: ignoreversion recursesubdirs createallsubdirs
; NOTE: Don't use "Flags: ignoreversion" on any shared system files
Source: "install.ico"; DestDir: "{app}"
Source: "uninstall.ico"; DestDir: "{app}"

[Icons]
Name: "{autoprograms}\{#MyAppName}"; \
  Filename: "{app}\{#MyAppExeName}"; \
  IconFilename: "{app}\install.ico"
Name: "{autodesktop}\{#MyAppName}"; \
  Filename: "{app}\{#MyAppExeName}"; \
  Tasks: desktopicon; \
  IconFilename: "{app}\install.ico"
Name: "{autostartup}\{#MyAppName}"; \
  Filename: "{app}\{#MyAppExeName}"; \
  Tasks: autostart; \
  IconFilename: "{app}\install.ico"

[Run]
Filename: "{app}\{#MyAppExeName}"; \
  Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; \
  Check: ShowPostinstallLaunchOption; \
  Flags: nowait postinstall skipifsilent

[UninstallRun]
Filename: "{sys}\taskkill.exe"; Parameters: "/f /im {#MyAppExeName}"; Flags: skipifdoesntexist runhidden


[Code]
var
  MustOpenAppAfterInstall: Boolean;
  IsUpgrade: Boolean;


function ShowPostinstallLaunchOption(): Boolean;
begin
  Result := not IsUpgrade;
end;


function StrSplit(Text: String; Separator: String): TArrayOfString;
var
  i, p: Integer;
  Dest: TArrayOfString;
begin
  i := 0;
  repeat
    SetArrayLength(Dest, i + 1);
    p := Pos(Separator, Text);
    if p > 0 then begin
      Dest[i] := Copy(Text, 1, p - 1);
      Text := Copy(Text, p + Length(Separator), Length(Text));
      i := i + 1;
    end else begin
      Dest[i] := Text;
      Text := '';
    end;
  until Length(Text) = 0;
  Result := Dest
end;


function GetPackedVersionComponents(Version: String): Int64;
var
  VersionArray: TArrayOfString;
  VersionArrayLength: Integer;
  i: Integer;
  Major, Minor, Build, Revision: LongInt;
begin
  Major := 0; Minor := 0; Build := 0; Revision := 0;
  VersionArray := StrSplit(Version, '.');
  VersionArrayLength := Length(VersionArray);
  for i := 0 to 3 do begin
    if i < VersionArrayLength then begin
      case i of
        0: Major := StrToInt(VersionArray[i]);
        1: Minor := StrToInt(VersionArray[i]);
        2: Build := StrToInt(VersionArray[i]);
        3: Revision := StrToInt(VersionArray[i]);
      end;
    end;
  end;
  Result := PackVersionComponents(Major, Minor, Build, Revision);
end;


function IsAppRunning(const FileName: String): Boolean;
var
  FSWbemLocator: Variant;
  FWMIService   : Variant;
  FWbemObjectSet: Variant;
begin
  Result := false;
  FSWbemLocator := CreateOleObject('WBEMScripting.SWBEMLocator');
  FWMIService := FSWbemLocator.ConnectServer('', 'root\CIMV2', '', '');
  FWbemObjectSet := FWMIService.ExecQuery(Format('SELECT Name FROM Win32_Process Where Name="%s"', [FileName]));
  Result := (FWbemObjectSet.Count > 0);
  FWbemObjectSet := Unassigned;
  FWMIService := Unassigned;
  FSWbemLocator := Unassigned;
end;


function InitializeSetup(): Boolean;
var
  RegistryUninstallPath, InstalledVersion: String;
  PackedPreviousVersion, PackedCurrentVersion: Int64;
  ResultCode: Integer;
begin
  MustOpenAppAfterInstall := False;
  Result := True;
  RegistryUninstallPath := ExpandConstant('SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{#emit SetupSetting("AppId")}_is1');
  if RegQueryStringValue(HKCU, RegistryUninstallPath, 'DisplayVersion', InstalledVersion) then begin
    PackedPreviousVersion := GetPackedVersionComponents(InstalledVersion);
    PackedCurrentVersion := GetPackedVersionComponents(ExpandConstant('{#MyAppVersion}'));
    if PackedPreviousVersion > PackedCurrentVersion then begin
      MsgBox(FmtMessage(CustomMessage('YouHaveNewerVersion'), [InstalledVersion]), mbInformation, MB_OK);
      Result := False;
    end else if PackedPreviousVersion = PackedCurrentVersion then begin
      MsgBox(FmtMessage(CustomMessage('YouHaveThisVersion'), [InstalledVersion]), mbInformation, MB_OK);
      Result := False;
    end else begin
      IsUpgrade := True;
    end;
  end;
  if Result and IsAppRunning(ExpandConstant('{#MyAppExeName}')) then begin
    MustOpenAppAfterInstall := True;
    Exec(ExpandConstant('{sys}\taskkill.exe'), ExpandConstant('/f /im {#MyAppExeName}'), '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
  end;
end;


procedure LaunchApplication();
var
  ResultCode: Integer;
begin
  Exec(ExpandConstant('{app}\{#MyAppExeName}'), '', '', SW_HIDE, ewNoWait, ResultCode);
end;


procedure CancelButtonClick(CurPageID: Integer; var Cancel, Confirm: Boolean);
begin
  if MustOpenAppAfterInstall then begin
    LaunchApplication();
  end;
end;


procedure CurStepChanged(CurStep: TSetupStep);

begin
  case CurStep of
    ssDone: begin
      if MustOpenAppAfterInstall then begin
        LaunchApplication();
      end;
    end;
  end;
end;


procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
var
  PublisherDir: String;
  DialogResult, CmdParamsCount, i: Integer;
  FoundRecord: TFindRec;
begin
  case CurUninstallStep of
    usPostUninstall: begin
      DialogResult := MsgBox(CustomMessage('DoYouWantRemoveAppSettingsAndClips'), mbConfirmation, MB_YESNO or MB_DEFBUTTON2);
      if DialogResult = IDYES then begin
        PublisherDir := ExpandConstant('{localappdata}\YTSoft\');
        if FindFirst(PublisherDir + 'JustClipboardManager*', FoundRecord) then begin
          try
            repeat
              if FoundRecord.Attributes and FILE_ATTRIBUTE_DIRECTORY <> 0 then begin
                DelTree(ExpandConstant(PublisherDir + FoundRecord.Name), True, True, True);
              end;
            until not FindNext(FoundRecord);
          finally
            FindClose(FoundRecord);
          end;
        end;
      end;
    end;
  end;
end;
